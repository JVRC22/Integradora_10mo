using System;
using System.IO;
using SQLite;
using SQLitePCL;
using UnityEngine;

namespace _Project.Scripts.Services.Data
{
    /// <summary>
    /// Servicio central para interactuar con game.db (SQLite).
    /// Se apoya en SystemLoader.SetupDatabase, que copia game_template.db
    /// desde StreamingAssets a Application.persistentDataPath/Database/game.db.
    /// 
    /// Este servicio:
    /// - Inicializa SQLitePCL (Batteries) una sola vez.
    /// - Abre la conexión a game.db al iniciar.
    /// - Asume que el esquema YA existe en game_template.db.
    /// - Expone métodos para guardar y cargar el JSON de progreso del jugador.
    /// </summary>
    public class DatabaseService : MonoBehaviour
    {
        public static DatabaseService Instance { get; private set; }

        private SQLiteConnection _connection;
        private string _dbPath;

        /// <summary>
        /// True si la conexión está abierta y lista para usarse.
        /// </summary>
        public bool IsReady => _connection != null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeSQLitePCL();
            OpenConnection();
        }

        /// <summary>
        /// Inicializa SQLitePCL Batteries (obligatorio para sqlite-net-pcl).
        /// Intentamos primero Batteries_V2 y, si no existe, Batteries normal.
        /// </summary>
        private void InitializeSQLitePCL()
        {
            try
            {
                // Para los paquetes SQLitePCLRaw.bundle_* modernos
                Batteries_V2.Init();
#if UNITY_EDITOR
                Debug.Log("[DatabaseService] SQLitePCL Batteries_V2.Init() OK.");
#endif
            }
            catch (Exception ex1)
            {
#if UNITY_EDITOR
                Debug.LogWarning("[DatabaseService] Batteries_V2.Init falló, probando Batteries.Init(). Detalle: " + ex1.Message);
#endif
                try
                {
                    Batteries.Init();
#if UNITY_EDITOR
                    Debug.Log("[DatabaseService] SQLitePCL Batteries.Init() OK.");
#endif
                }
                catch (Exception ex2)
                {
                    Debug.LogError("[DatabaseService] Error al inicializar SQLitePCL: " + ex2);
                }
            }
        }

        /// <summary>
        /// Abre la conexión a game.db en la carpeta persistente.
        /// Depende de que SystemLoader.SetupDatabase ya haya copiado la BD.
        /// </summary>
        private void OpenConnection()
        {
            var databaseFolder = Path.Combine(Application.persistentDataPath, "Database");
            _dbPath = Path.Combine(databaseFolder, "game.db");

            if (!File.Exists(_dbPath))
            {
                Debug.LogError($"[DatabaseService] game.db no existe en: {_dbPath}. " +
                               "Asegúrate de que SystemLoader.SetupDatabase se ejecute antes.");
                return;
            }

            try
            {
                _connection = new SQLiteConnection(_dbPath);
#if UNITY_EDITOR
                Debug.Log("[DatabaseService] Conexión abierta a " + _dbPath);
#endif
            }
            catch (Exception ex)
            {
                Debug.LogError($"[DatabaseService] Error al abrir conexión SQLite: {ex.Message}");
                _connection = null;
            }
        }

        /// <summary>
        /// Guarda o actualiza un perfil en la tabla player_profiles.
        /// - slot: número de ranura (1, 2, 3, etc.).
        /// - name: nombre visible del perfil (para UI).
        /// - progressJson: JSON generado por GameStateManager.BuildSaveGameJson().
        /// 
        /// IMPORTANTE:
        /// La tabla player_profiles debe existir en game_template.db.
        /// </summary>
        public void SaveProfile(int slot, string name, string progressJson)
        {
            if (_connection == null)
            {
                Debug.LogError("[DatabaseService] SaveProfile llamado pero _connection es null.");
                return;
            }

            var now = DateTime.UtcNow.ToString("o");

            try
            {
                // ¿Ya existe un registro con ese slot?
                var cmdSelect = _connection.CreateCommand(
                    "SELECT id FROM player_profiles WHERE slot = ? LIMIT 1",
                    slot
                );
                var existingId = cmdSelect.ExecuteScalar<int>();

                if (existingId == 0)
                {
                    // Insert nuevo
                    var cmdInsert = _connection.CreateCommand(
                        "INSERT INTO player_profiles (slot, name, progress_json, created_at, updated_at) " +
                        "VALUES (?, ?, ?, ?, ?)",
                        slot, name, progressJson, now, now
                    );
                    cmdInsert.ExecuteNonQuery();
#if UNITY_EDITOR
                    Debug.Log($"[DatabaseService] Nuevo perfil guardado en slot {slot}.");
#endif
                }
                else
                {
                    // Update existente
                    var cmdUpdate = _connection.CreateCommand(
                        "UPDATE player_profiles " +
                        "SET name = ?, progress_json = ?, updated_at = ? " +
                        "WHERE id = ?",
                        name, progressJson, now, existingId
                    );
                    cmdUpdate.ExecuteNonQuery();
#if UNITY_EDITOR
                    Debug.Log($"[DatabaseService] Perfil actualizado en slot {slot} (id {existingId}).");
#endif
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[DatabaseService] Error en SaveProfile: {ex.Message}");
            }
        }

        /// <summary>
        /// Carga el JSON de progreso de un slot. 
        /// Devuelve null o string.Empty si no hay registro.
        /// </summary>
        public string LoadProfileJson(int slot)
        {
            if (_connection == null)
            {
                Debug.LogError("[DatabaseService] LoadProfileJson llamado pero _connection es null.");
                return null;
            }

            try
            {
                var cmd = _connection.CreateCommand(
                    "SELECT progress_json FROM player_profiles WHERE slot = ? LIMIT 1",
                    slot
                );
                var json = cmd.ExecuteScalar<string>();

#if UNITY_EDITOR
                Debug.Log($"[DatabaseService] LoadProfileJson slot {slot}: " +
                          (string.IsNullOrEmpty(json) ? "SIN DATA" : "OK"));
#endif

                return json;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[DatabaseService] Error en LoadProfileJson: {ex.Message}");
                return null;
            }
        }
    }
}