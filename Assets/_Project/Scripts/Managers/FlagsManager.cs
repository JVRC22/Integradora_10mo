using System.Collections.Generic;
using _Project.Scripts.Data.StaticModels;
using _Project.Scripts.Services.Data;
using UnityEngine;

namespace _Project.Scripts.Managers
{
    public class FlagsManager : MonoBehaviour
    {
        public static FlagsManager Instance { get; private set; }

        // Estado actual de flags del jugador (partida activa)
        private readonly Dictionary<string, bool> _runtimeFlags = new Dictionary<string, bool>();

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
        /// Inicializa los flags en memoria para una partida, usando los valores por defecto.
        /// Esto deberías llamarlo cuando creas/cargas un perfil.
        /// </summary>
        public void InitializeForNewProfile()
        {
            _runtimeFlags.Clear();

            var staticFlags = StaticDataService.Instance.Flags;
            foreach (var kvp in staticFlags)
            {
                var flagData = kvp.Value;
                _runtimeFlags[flagData.id] = flagData.defaultValue;
            }

            #if UNITY_EDITOR
                Debug.Log($"[FlagsManager] Flags inicializados para nuevo perfil. Total: {_runtimeFlags.Count}");
            #endif
        }

        /// <summary>
        /// Asigna un valor a un flag (true/false).
        /// </summary>
        public void SetFlag(string flagId, bool value)
        {
            if (!_runtimeFlags.ContainsKey(flagId))
            {
                Debug.LogWarning($"[FlagsManager] SetFlag llamado con flag desconocido: {flagId}. Lo agrego on-the-fly.");
                _runtimeFlags[flagId] = value;
            }
            else
            {
                _runtimeFlags[flagId] = value;
            }

            #if UNITY_EDITOR
                Debug.Log($"[FlagsManager] {flagId} = {value}");
            #endif

            // Notificar a MissionManager SOLO cuando el flag se activa (true)
            if (value && MissionManager.Instance != null)
            {
                MissionManager.Instance.TryAdvanceMissionsOnFlag(flagId);
            }
        }

        /// <summary>
        /// Marca un flag como activo (true).
        /// </summary>
        public void RaiseFlag(string flagId)
        {
            SetFlag(flagId, true);
        }

        /// <summary>
        /// Devuelve el estado actual de un flag. Si no existe, devuelve false.
        /// </summary>
        public bool IsFlagActive(string flagId)
        {
            return _runtimeFlags.TryGetValue(flagId, out var value) && value;
        }

        /// <summary>
        /// Exporta todos los flags actuales a un diccionario para poder serializarlo.
        /// Esto lo usaremos luego para guardar en BD/JSON de progreso.
        /// </summary>
        public Dictionary<string, bool> ExportFlags()
        {
            return new Dictionary<string, bool>(_runtimeFlags);
        }

        /// <summary>
        /// Restaura flags desde un diccionario previamente guardado (load de partida).
        /// </summary>
        public void ImportFlags(Dictionary<string, bool> flags)
        {
            _runtimeFlags.Clear();

            foreach (var kvp in flags)
            {
                _runtimeFlags[kvp.Key] = kvp.Value;
            }
        }
    }
}