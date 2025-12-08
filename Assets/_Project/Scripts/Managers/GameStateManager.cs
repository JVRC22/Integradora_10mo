using System;
using System.Collections.Generic;
using _Project.Scripts.Data.Runtime;
using _Project.Scripts.Services.Data;
using _Project.Scripts.Data.StaticModels;
using UnityEngine;

namespace _Project.Scripts.Managers
{
    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }

        // Progreso "simple" en memoria (lo que ya tenías)
        private PlayerProgressData _currentProgress;
        public PlayerProgressData CurrentProgress => _currentProgress;

        // Estado extendido para el sistema de guardado
        private readonly HashSet<string> _completedObjectives = new HashSet<string>();
        private readonly HashSet<string> _knownZones = new HashSet<string>();

        private Vector2 _lastKnownPosition;
        private string _lastDirection = "down";

        private string _currentSaveCreatedAt;

        /// <summary>
        /// Chapter actual en forma de key (ej: "chapter_1").
        /// Se refleja en _currentProgress.chapterKey.
        /// </summary>
        public string CurrentProgressChapterKey
        {
            get => _currentProgress?.chapterKey ?? "chapter_1";
            set
            {
                EnsureProgressExists();
                _currentProgress.chapterKey = value;
            }
        }

        /// <summary>
        /// Zona actual en forma de key (ej: "mist_forest", "lab_console").
        /// Se refleja en _currentProgress.currentZoneKey.
        /// </summary>
        public string CurrentProgressZoneKey
        {
            get => _currentProgress?.currentZoneKey ?? "mist_forest";
            set
            {
                EnsureProgressExists();
                _currentProgress.currentZoneKey = value;
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Crea un progreso nuevo desde cero (Nuevo Juego).
        /// Usa los valores iniciales de los JSON estáticos.
        /// </summary>
        public void StartNewGame()
        {
            var defaultChapterKey = "chapter_1";
            var defaultZoneKey    = "mist_forest";

            _currentProgress = new PlayerProgressData
            {
                profileId        = "local_profile_1",
                chapterKey       = defaultChapterKey,
                currentZoneKey   = defaultZoneKey,
                currentSceneName = "" // luego lo rellenamos cuando definamos escenas
            };

            _completedObjectives.Clear();
            _knownZones.Clear();
            _knownZones.Add(defaultZoneKey);

            _lastKnownPosition = Vector2.zero;
            _lastDirection = "down";
            _currentSaveCreatedAt = null;

            FlagsManager.Instance.InitializeForNewProfile();
            MissionManager.Instance.InitializeForNewProfile();

            _currentProgress = BuildProgressSnapshot(_currentProgress);

            #if UNITY_EDITOR
                Debug.Log("[GameStateManager] Nuevo juego inicializado.");
            #endif
        }

        /// <summary>
        /// Compat: aplica un PlayerProgressData simple (por si lo usas en alguna parte).
        /// A nivel “oficial” de guardado usaremos SaveGameData.
        /// </summary>
        public void LoadFromProgress(PlayerProgressData data)
        {
            if (data == null)
            {
                Debug.LogWarning("[GameStateManager] LoadFromProgress llamado con data null. Se usará StartNewGame().");
                StartNewGame();
                return;
            }

            _currentProgress = data;

            // 1. Flags
            var flagsDict = new Dictionary<string, bool>();
            if (data.flags != null)
            {
                foreach (var f in data.flags)
                {
                    if (!string.IsNullOrEmpty(f.id))
                        flagsDict[f.id] = f.value;
                }
            }
            FlagsManager.Instance.ImportFlags(flagsDict);

            // 2. Misiones
            var missionsDict = new Dictionary<string, string>();
            if (data.missions != null)
            {
                foreach (var m in data.missions)
                {
                    if (!string.IsNullOrEmpty(m.id))
                        missionsDict[m.id] = m.state;
                }
            }
            MissionManager.Instance.ImportMissionStates(missionsDict);

            #if UNITY_EDITOR
                Debug.Log("[GameStateManager] Progreso cargado y aplicado a managers runtime (PlayerProgressData).");
            #endif
        }

        /// <summary>
        /// Genera un snapshot de PlayerProgressData con flags y misiones actuales.
        /// (Útil si lo estabas usando para debug. El guardado real ahora usa SaveGameData.)
        /// </summary>
        public PlayerProgressData BuildCurrentProgressSnapshot()
        {
            EnsureProgressExists();
            _currentProgress = BuildProgressSnapshot(_currentProgress);
            return _currentProgress;
        }

        private PlayerProgressData BuildProgressSnapshot(PlayerProgressData baseData)
        {
            // Flags
            var flagsDict = FlagsManager.Instance.ExportFlags();
            var flagsList = new List<SerializableFlagState>();
            foreach (var kvp in flagsDict)
            {
                flagsList.Add(new SerializableFlagState
                {
                    id    = kvp.Key,
                    value = kvp.Value
                });
            }

            // Misiones
            var missionsDict = MissionManager.Instance.ExportMissionStates();
            var missionsList = new List<SerializableMissionState>();
            foreach (var kvp in missionsDict)
            {
                missionsList.Add(new SerializableMissionState
                {
                    id    = kvp.Key,
                    state = kvp.Value
                });
            }

            baseData.flags    = flagsList.ToArray();
            baseData.missions = missionsList.ToArray();

            return baseData;
        }

        // ==============================
        //  NUEVA PARTE: SAVEGAME DATA
        // ==============================

        /// <summary>
        /// Marca un objetivo como completado (por id de objective del JSON de capítulos).
        /// </summary>
        public void MarkObjectiveCompleted(string objectiveId)
        {
            if (!string.IsNullOrEmpty(objectiveId))
                _completedObjectives.Add(objectiveId);
        }

        public bool IsObjectiveCompleted(string objectiveId)
        {
            return _completedObjectives.Contains(objectiveId);
        }

        /// <summary>
        /// Registra que una zona ya fue visitada / conocida.
        /// </summary>
        public void RegisterZoneKnown(string zoneKey)
        {
            if (!string.IsNullOrEmpty(zoneKey))
                _knownZones.Add(zoneKey);
        }

        public bool IsZoneKnown(string zoneKey)
        {
            return _knownZones.Contains(zoneKey);
        }

        /// <summary>
        /// Llamar desde el PlayerController o algún sistema de posicionamiento.
        /// </summary>
        public void UpdatePlayerTransform(Vector2 position, string direction)
        {
            _lastKnownPosition = position;
            if (!string.IsNullOrEmpty(direction))
                _lastDirection = direction;
        }

        private void EnsureProgressExists()
        {
            if (_currentProgress != null) return;

            _currentProgress = new PlayerProgressData
            {
                profileId        = "local_profile_1",
                chapterKey       = "chapter_1",
                currentZoneKey   = "mist_forest",
                currentSceneName = ""
            };
        }

        /// <summary>
        /// Construye el SaveGameData completo usando:
        /// - chapterId / zoneKey / posición / dirección
        /// - flags activos
        /// - estados de misiones
        /// - objetivos completados
        /// - zonas conocidas
        /// </summary>
        public SaveGameData BuildSaveGameData()
        {
            EnsureProgressExists();

            var now = DateTime.UtcNow.ToString("o");

            // Resolver chapterId a partir de chapterKey
            var chapterId = 1;
            var chapterKey = CurrentProgressChapterKey;
            foreach (var kvp in StaticDataService.Instance.Chapters)
            {
                if (kvp.Key == chapterKey)
                {
                    chapterId = kvp.Value.id;
                    break;
                }
            }

            var save = new SaveGameData
            {
                version = "1.0.0",
                metadata = new SaveMetadata
                {
                    createdAt     = _currentSaveCreatedAt ?? now,
                    lastUpdatedAt = now
                },
                playerState = new SavePlayerState
                {
                    currentChapterId = chapterId,
                    currentZoneKey   = CurrentProgressZoneKey,
                    position = new SavePosition
                    {
                        x = _lastKnownPosition.x,
                        y = _lastKnownPosition.y
                    },
                    direction = _lastDirection
                },
                progress = new SaveProgress()
            };

            // Flags activos
            var flagsDict = FlagsManager.Instance.ExportFlags();
            var activeFlags = new List<string>();
            foreach (var kvp in flagsDict)
            {
                if (kvp.Value)
                    activeFlags.Add(kvp.Key);
            }
            save.progress.activeFlags = activeFlags.ToArray();

            // Misiones
            var missionsDict = MissionManager.Instance.ExportMissionStates();
            var missionsList = new List<SaveMissionProgress>();
            foreach (var kvp in missionsDict)
            {
                missionsList.Add(new SaveMissionProgress
                {
                    missionId     = kvp.Key,
                    state         = kvp.Value,
                    lastStepId    = "",  // si luego quieres, podemos trackearlo
                    lastUpdatedAt = now
                });
            }
            save.progress.missions = missionsList.ToArray();

            // Objetivos completados
            save.progress.completedObjectives = new List<string>(_completedObjectives).ToArray();

            // Zonas conocidas
            save.progress.zones = new List<string>(_knownZones).ToArray();

            _currentSaveCreatedAt = save.metadata.createdAt;

            return save;
        }

        /// <summary>
        /// Aplica un SaveGameData existente a los managers runtime y al estado interno.
        /// </summary>
        public void ApplySaveGameData(SaveGameData data)
        {
            if (data == null)
            {
                Debug.LogWarning("[GameStateManager] ApplySaveGameData recibido null. Usando StartNewGame().");
                StartNewGame();
                return;
            }

            _currentSaveCreatedAt = data.metadata?.createdAt;

            // Chapter/Zone
            CurrentProgressZoneKey    = data.playerState.currentZoneKey;
            CurrentProgressChapterKey = ResolveChapterKeyFromId(data.playerState.currentChapterId);

            // Posición / dirección
            if (data.playerState.position != null)
            {
                _lastKnownPosition = new Vector2(
                    data.playerState.position.x,
                    data.playerState.position.y
                );
            }
            _lastDirection = string.IsNullOrEmpty(data.playerState.direction)
                ? "down"
                : data.playerState.direction;

            // Flags
            var flagsDict = new Dictionary<string, bool>();
            if (data.progress.activeFlags != null)
            {
                foreach (var flagId in data.progress.activeFlags)
                {
                    flagsDict[flagId] = true;
                }
            }
            FlagsManager.Instance.ImportFlags(flagsDict);

            // Misiones
            var missionsDict = new Dictionary<string, string>();
            if (data.progress.missions != null)
            {
                foreach (var m in data.progress.missions)
                {
                    if (!string.IsNullOrEmpty(m.missionId))
                        missionsDict[m.missionId] = m.state;
                }
            }
            MissionManager.Instance.ImportMissionStates(missionsDict);

            // Objetivos
            _completedObjectives.Clear();
            if (data.progress.completedObjectives != null)
            {
                foreach (var obj in data.progress.completedObjectives)
                    _completedObjectives.Add(obj);
            }

            // Zonas
            _knownZones.Clear();
            if (data.progress.zones != null)
            {
                foreach (var z in data.progress.zones)
                    _knownZones.Add(z);
            }

            #if UNITY_EDITOR
                Debug.Log("[GameStateManager] SaveGameData aplicado.");
            #endif
        }

        private string ResolveChapterKeyFromId(int chapterId)
        {
            foreach (var kvp in StaticDataService.Instance.Chapters)
            {
                if (kvp.Value.id == chapterId)
                    return kvp.Key;
            }

            return "chapter_1";
        }

        /// <summary>
        /// Serializa el SaveGameData actual a JSON para guardarlo en BD.
        /// </summary>
        public string BuildSaveGameJson()
        {
            var data = BuildSaveGameData();
            return JsonUtility.ToJson(data, prettyPrint: false);
        }

        /// <summary>
        /// Reconstruye el estado desde un JSON previamente guardado.
        /// </summary>
        public void LoadFromSaveGameJson(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning("[GameStateManager] LoadFromSaveGameJson llamado con json vacío. StartNewGame().");
                StartNewGame();
                return;
            }

            var data = JsonUtility.FromJson<SaveGameData>(json);
            ApplySaveGameData(data);
        }
    }
}