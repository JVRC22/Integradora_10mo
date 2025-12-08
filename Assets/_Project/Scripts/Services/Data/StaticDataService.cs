using System.Collections.Generic;
using _Project.Scripts.Data.StaticModels;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace _Project.Scripts.Services.Data
{
    public class StaticDataService : MonoBehaviour
    {
        public static StaticDataService Instance { get; private set; }

        [Header("JSON estáticos")]
        [SerializeField] private TextAsset chaptersJson;
        [SerializeField] private TextAsset puzzlesJson;
        [SerializeField] private TextAsset missionsJson;
        [SerializeField] private TextAsset flagsJson;
        [SerializeField] private TextAsset charactersJson;
        [SerializeField] private TextAsset characterDialogueRoutesJson;
        [SerializeField] private TextAsset dialoguesJson;

        private Dictionary<string, ChapterData> _chaptersByKey;
        private Dictionary<string, PuzzleData>  _puzzlesByKey;
        private Dictionary<string, MissionData> _missionsById;
        private Dictionary<string, FlagData> _flagsById;
        private Dictionary<string, CharacterData> _charactersById;
        private Dictionary<string, CharacterDialogueRoutesForCharacter> _dialogueRoutesByCharacterId;
        private Dictionary<string, ConversationData> _conversationsById;

        public IReadOnlyDictionary<string, ChapterData> Chapters => _chaptersByKey;
        public IReadOnlyDictionary<string, PuzzleData>  Puzzles  => _puzzlesByKey;
        public IReadOnlyDictionary<string, MissionData> Missions => _missionsById;
        public IReadOnlyDictionary<string, FlagData> Flags => _flagsById;
        public IReadOnlyDictionary<string, CharacterData> Characters => _charactersById;
        public IReadOnlyDictionary<string, CharacterDialogueRoutesForCharacter> CharacterDialogueRoutes => _dialogueRoutesByCharacterId;
        public IReadOnlyDictionary<string, ConversationData> Conversations => _conversationsById;

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
        /// Carga todos los JSON estáticos necesarios en memoria.
        /// Llamar una sola vez en la escena Boot.
        /// </summary>
        public void Initialize()
        {
            LoadChapters();
            LoadPuzzles();
            LoadMissions();
            LoadFlags();
            LoadCharacters();
            LoadCharacterDialogueRoutes();
            LoadDialogues();
        }

        private void LoadChapters()
        {
            if (chaptersJson == null)
            {
                Debug.LogError("[StaticDataService] chaptersJson no asignado en el inspector.");
                _chaptersByKey = new Dictionary<string, ChapterData>();
                return;
            }

            var root = JsonUtility.FromJson<ChaptersRoot>(chaptersJson.text);
            _chaptersByKey = new Dictionary<string, ChapterData>();

            if (root?.chapters == null)
            {
                Debug.LogError("[StaticDataService] No se pudieron parsear capítulos desde chapters.json");
                return;
            }

            foreach (var chapter in root.chapters)
            {
                if (!string.IsNullOrEmpty(chapter.key))
                {
                    _chaptersByKey[chapter.key] = chapter;
                }
            }

            #if UNITY_EDITOR
                Debug.Log($"[StaticDataService] Chapters cargados: {_chaptersByKey.Count}");
            #endif
        }

        // Helper para obtener un capítulo específico
        public ChapterData GetChapterByKey(string key)
        {
            if (_chaptersByKey == null) return null;
            _chaptersByKey.TryGetValue(key, out var chapter);
            return chapter;
        }
        
        private void LoadPuzzles()
        {
            if (puzzlesJson == null)
            {
                Debug.LogError("[StaticDataService] puzzlesJson no asignado en el inspector.");
                _puzzlesByKey = new Dictionary<string, PuzzleData>();
                return;
            }

            var root = JsonUtility.FromJson<PuzzlesRoot>(puzzlesJson.text);
            _puzzlesByKey = new Dictionary<string, PuzzleData>();

            if (root?.puzzles == null)
            {
                Debug.LogError("[StaticDataService] No se pudieron parsear puzzles desde puzzles.json");
                return;
            }

            foreach (var puzzle in root.puzzles)
            {
                if (!string.IsNullOrEmpty(puzzle.key))
                {
                    _puzzlesByKey[puzzle.key] = puzzle;
                }
            }

            #if UNITY_EDITOR
                Debug.Log($"[StaticDataService] Puzzles cargados: {_puzzlesByKey.Count}");
            #endif
        }
        
        // Helper
        public PuzzleData GetPuzzleByKey(string key)
        {
            if (_puzzlesByKey == null) return null;
            _puzzlesByKey.TryGetValue(key, out var puzzle);
            return puzzle;
        }
        
        private void LoadMissions()
        {
            if (missionsJson == null)
            {
                Debug.LogError("[StaticDataService] missionsJson no asignado en el inspector.");
                _missionsById = new Dictionary<string, MissionData>();
                return;
            }

            var root = JsonUtility.FromJson<MissionsRoot>(missionsJson.text);
            _missionsById = new Dictionary<string, MissionData>();

            if (root?.missions == null)
            {
                Debug.LogError("[StaticDataService] No se pudieron parsear misiones desde missions.json");
                return;
            }

            foreach (var mission in root.missions)
            {
                if (!string.IsNullOrEmpty(mission.id))
                {
                    _missionsById[mission.id] = mission;
                }
            }

            #if UNITY_EDITOR
                Debug.Log($"[StaticDataService] Missions cargadas: {_missionsById.Count}");
            #endif
        }

        public MissionData GetMissionById(string id)
        {
            if (_missionsById == null) return null;
            _missionsById.TryGetValue(id, out var mission);
            return mission;
        }
        
        private void LoadFlags()
        {
            if (flagsJson == null)
            {
                Debug.LogError("[StaticDataService] flagsJson no asignado en el inspector.");
                _flagsById = new Dictionary<string, FlagData>();
                return;
            }

            var root = JsonUtility.FromJson<FlagsRoot>(flagsJson.text);
            _flagsById = new Dictionary<string, FlagData>();

            if (root?.flags == null)
            {
                Debug.LogError("[StaticDataService] No se pudieron parsear flags desde flags.json");
                return;
            }

            foreach (var flag in root.flags)
            {
                if (!string.IsNullOrEmpty(flag.id))
                {
                    _flagsById[flag.id] = flag;
                }
            }

            #if UNITY_EDITOR
                Debug.Log($"[StaticDataService] Flags cargados: {_flagsById.Count}");
            #endif
        }

        public FlagData GetFlagById(string id)
        {
            if (_flagsById == null) return null;
            _flagsById.TryGetValue(id, out var flag);
            return flag;
        }
        
        private void LoadCharacters()
        {
            if (charactersJson == null)
            {
                Debug.LogError("[StaticDataService] charactersJson no asignado en el inspector.");
                _charactersById = new Dictionary<string, CharacterData>();
                return;
            }

            var root = JsonUtility.FromJson<CharactersRoot>(charactersJson.text);
            _charactersById = new Dictionary<string, CharacterData>();

            if (root?.characters == null)
            {
                Debug.LogError("[StaticDataService] No se pudieron parsear personajes desde characters.json");
                return;
            }

            foreach (var ch in root.characters)
            {
                if (!string.IsNullOrEmpty(ch.id))
                {
                    _charactersById[ch.id] = ch;
                }
            }

            #if UNITY_EDITOR
                Debug.Log($"[StaticDataService] Characters cargados: {_charactersById.Count}");
            #endif
        }

        public CharacterData GetCharacterById(string id)
        {
            if (_charactersById == null) return null;
            _charactersById.TryGetValue(id, out var character);
            return character;
        }
        
        private void LoadCharacterDialogueRoutes()
        {
            if (characterDialogueRoutesJson == null)
            {
                Debug.LogError("[StaticDataService] characterDialogueRoutesJson no asignado en el inspector.");
                _dialogueRoutesByCharacterId = new Dictionary<string, CharacterDialogueRoutesForCharacter>();
                return;
            }

            var root = JsonUtility.FromJson<CharacterDialogueRoutesRoot>(characterDialogueRoutesJson.text);
            _dialogueRoutesByCharacterId = new Dictionary<string, CharacterDialogueRoutesForCharacter>();

            if (root?.routes == null)
            {
                Debug.LogError("[StaticDataService] No se pudieron parsear rutas de diálogo desde characterDialogueRoutes.json");
                return;
            }

            foreach (var group in root.routes)
            {
                if (!string.IsNullOrEmpty(group.characterId))
                {
                    _dialogueRoutesByCharacterId[group.characterId] = group;
                }
            }

            #if UNITY_EDITOR
                Debug.Log($"[StaticDataService] Dialogue routes cargadas para personajes: {_dialogueRoutesByCharacterId.Count}");
            #endif
        }

        public CharacterDialogueRoutesForCharacter GetDialogueRoutesForCharacter(string characterId)
        {
            if (_dialogueRoutesByCharacterId == null) return null;
            _dialogueRoutesByCharacterId.TryGetValue(characterId, out var routes);
            return routes;
        }
        
        private void LoadDialogues()
        {
            if (dialoguesJson == null)
            {
                Debug.LogError("[StaticDataService] dialoguesJson no asignado en el inspector.");
                _conversationsById = new Dictionary<string, ConversationData>();
                return;
            }

            var root = JsonUtility.FromJson<DialoguesRoot>(dialoguesJson.text);
            _conversationsById = new Dictionary<string, ConversationData>();

            if (root == null || root.conversations == null)
            {
                Debug.LogError("[StaticDataService] No se pudieron parsear conversaciones desde dialogues.json");
                return;
            }

            foreach (var conv in root.conversations)
            {
                if (!string.IsNullOrEmpty(conv.id))
                {
                    _conversationsById[conv.id] = conv;
                }
            }

            #if UNITY_EDITOR
                Debug.Log($"[StaticDataService] Dialogues cargados: {_conversationsById.Count}");
            #endif
        }

        public ConversationData GetConversationById(string id)
        {
            if (_conversationsById == null) return null;
            _conversationsById.TryGetValue(id, out var conv);
            return conv;
        }
    }
}