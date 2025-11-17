using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Managers
{
    public class SystemLoader : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private GameObject managersPrefab;

        [Header("Scenes")]
        [SerializeField] private string mainMenuSceneName = "MainMenu";
    
        // Para evitar inicializar dos veces si Boot se recarga
        private static bool _alreadyInitialized;

        /*
         * Awake() se ejecuta al cargar la escena Boot y se encarga de iniciar
         * el proceso global del juego. Este método asegura que:
         *
         * 1. El SystemLoader no se destruya al cambiar de escena.
         * 2. Si el juego ya fue inicializado previamente (por ejemplo, tras regresar
         *    accidentalmente a Boot), se evite la reinicialización duplicada.
         * 3. Si ya existe una instancia inicializada, se redirige inmediatamente
         *    a la escena del menú principal.
         * 4. Si es la primera ejecución, se marca como inicializado y se inicia
         *    la corrutina principal de inicialización del proyecto.
         */
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            if (_alreadyInitialized)
                // Si ya se inicializo todo antes, manda al menu
                SceneManager.LoadScene(mainMenuSceneName);
        
            _alreadyInitialized = true;

            StartCoroutine(InitializeCoroutine());
        }

        /*
         * Corrutina principal de inicialización del juego.
         * Se ejecuta una sola vez al iniciar la aplicación, y prepara los
         * sistemas fundamentales antes de mostrar cualquier escena interactiva.
         *
         * Pasos:
         *  1. Instancia y configura todos los managers globales del juego
         *     (GameManager, AudioManager, UIManager, etc.) usando el prefab asignado.
         *
         *  2. Carga los datos estáticos del proyecto (JSON de capítulos, personajes,
         *     puzzles, misiones, flags, etc.) para tenerlos disponibles en memoria.
         *
         *  3. Copia la base de datos game_template.db desde StreamingAssets hacia
         *     persistentDataPath si aún no existe, creando así la base de datos
         *     de runtime del jugador (game.db).
         *
         *  4. Carga o reconstruye el progreso del jugador si existe un save previo.
         *
         *  5. Tras un frame de espera (evita saltos visuales), redirige a la
         *     escena principal del menú.
         *
         * Esta corrutina permite hacer el proceso progresivo y evitar bloqueos
         * durante el inicio.
         */
        private IEnumerator InitializeCoroutine()
        {
            InitializeManagers(); // 1. Instanciar managers globales
        
            LoadStaticJsonData(); // 2. Cargar datos estáticos desde JSON
        
            SetupDatabase(); // 3. Preparar base de datos SQLite
        
            LoadPlayerProgress(); // 4. Reconstruir el progreso si existe

            yield return null;

            SceneManager.LoadScene(mainMenuSceneName); // 5. Ir al menu principal
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void InitializeManagers()
        {
            if (!managersPrefab)
            {
                Debug.LogWarning("[SystemLoader] Managers prefab no asignado.");
                return;
            }

            var managersInstance = Instantiate(managersPrefab);
            managersInstance.name = "GameSystems";
            DontDestroyOnLoad(managersInstance);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private static void LoadStaticJsonData()
        {
            // Llamar al sistema de datos estáticos
            Debug.Log("[SystemLoader] LoadStaticJsonData() - pendiente de implementación.");
        }
        
        /*
         * Copia la base de datos inicial desde StreamingAssets hacia el directorio
         * persistente del usuario. Este proceso solo ocurre cuando la BD de runtime
         * (game.db) aún no existe.
         *
         * - sourcePath → ruta absoluta de la plantilla (game_template.db)
         * - targetPath → ruta de destino donde se creará game.db
         */
        private static void SetupDatabase()
        {
            // Ruta de destino: carpeta Database dentro de persistentDataPath
            var databaseFolder = Path.Combine(Application.persistentDataPath, "Database");
            if (!Directory.Exists(databaseFolder))
                Directory.CreateDirectory(databaseFolder);

            var targetPath = Path.Combine(databaseFolder, "game.db");

            // Si ya existe la DB de usuario, no se reemplazará
            if (File.Exists(targetPath))
                return;

            // Ruta de origen: StreamingAssets/Database/game_template.db
            var sourcePath = Path.Combine(Application.streamingAssetsPath, "Database/game_template.db");

            if (File.Exists(sourcePath))
                File.Copy(sourcePath, targetPath);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private static void LoadPlayerProgress()
        {
            // Llamar al sistema de datos estáticos
            Debug.Log("[SystemLoader] LoadPlayerProgress - pendiente de implementación.");
        }
    }
}