using System;
using System.Collections.Generic;
using _Project.Scripts.Data.StaticModels;
using _Project.Scripts.Services.Data;
using UnityEngine;

namespace _Project.Scripts.Managers
{
    public class MissionManager : MonoBehaviour
    {
        public static MissionManager Instance { get; private set; }

        // Estado actual de cada misión -> missionId → stateId ("not_started", "in_progress", etc.)
        private readonly Dictionary<string, string> _missionStates = new Dictionary<string, string>();

        public event Action<string, string, string> OnMissionStateChanged;
        // (missionId, oldState, newState)

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
        /// Inicializa el estado de todas las misiones para un perfil nuevo,
        /// usando el initialState definido en missions.json.
        /// Llama esto cuando crees / cargues un perfil por primera vez.
        /// </summary>
        public void InitializeForNewProfile()
        {
            _missionStates.Clear();

            var missions = StaticDataService.Instance.Missions;
            foreach (var kvp in missions)
            {
                var mission = kvp.Value;
                _missionStates[mission.id] = mission.initialState;
            }

            #if UNITY_EDITOR
                Debug.Log($"[MissionManager] Misiones inicializadas para nuevo perfil. Total: {_missionStates.Count}");
            #endif
        }

        /// <summary>
        /// Devuelve el estado actual de una misión. Si no existe, devuelve null.
        /// </summary>
        public string GetMissionState(string missionId)
        {
            return _missionStates.TryGetValue(missionId, out var state) ? state : null;
        }

        /// <summary>
        /// Indica si una misión está en un estado específico.
        /// </summary>
        public bool IsMissionInState(string missionId, string stateId)
        {
            var current = GetMissionState(missionId);
            return current == stateId;
        }

        /// <summary>
        /// Indica si una misión está completada.
        /// Por ahora asumimos que el estado "completed" significa eso.
        /// </summary>
        public bool IsMissionCompleted(string missionId)
        {
            return IsMissionInState(missionId, "completed");
        }

        /// <summary>
        /// Fuerza el cambio de estado de una misión (por código).
        /// Normalmente se usa internamente al reaccionar a flags.
        /// </summary>
        public void SetMissionState(string missionId, string newState)
        {
            _missionStates.TryGetValue(missionId, out var oldState);

            if (oldState == newState)
                return;

            _missionStates[missionId] = newState;

            #if UNITY_EDITOR
                Debug.Log($"[MissionManager] {missionId}: {oldState ?? "[null]"} → {newState}");
            #endif

            OnMissionStateChanged?.Invoke(missionId, oldState, newState);
        }

        /// <summary>
        /// Intenta avanzar misiones en base a un flag recién activado.
        /// Recorre todas las misiones y sus steps, buscando:
        /// - step.state == estado actual
        /// - step.triggersOn contiene ese flagId
        /// y si matchea, les hace SetMissionState(setStateTo).
        /// </summary>
        public bool TryAdvanceMissionsOnFlag(string flagId)
        {
            var missions = StaticDataService.Instance.Missions;
            var changedAny = false;

            foreach (var kvp in missions)
            {
                var mission = kvp.Value;

                // Estado actual de esa misión
                if (!_missionStates.TryGetValue(mission.id, out var currentState))
                {
                    // Si no existía aún, usamos el initialState
                    currentState = mission.initialState;
                    _missionStates[mission.id] = currentState;
                }

                foreach (var step in mission.steps)
                {
                    // Solo interesa el step cuyo "state" coincida con el estado actual
                    if (step.state != currentState)
                        continue;

                    // Si no hay triggers, nada que hacer
                    if (step.triggersOn == null || step.triggersOn.Length == 0)
                        continue;

                    // ¿Este step reacciona al flag recibido?
                    var matchesFlag = false;
                    foreach (var triggerFlag in step.triggersOn)
                    {
                        if (triggerFlag == flagId)
                        {
                            matchesFlag = true;
                            break;
                        }
                    }

                    if (!matchesFlag) continue;

                    // Tenemos match: cambiamos de estado
                    SetMissionState(mission.id, step.setStateTo);
                    changedAny = true;

                    // IMPORTANTE: solo aplicamos un step por flag por misión
                    break;
                }
            }

            return changedAny;
        }

        /// <summary>
        /// Exporta los estados actuales de misiones para guardarlo en BD/JSON.
        /// </summary>
        public Dictionary<string, string> ExportMissionStates()
        {
            return new Dictionary<string, string>(_missionStates);
        }

        /// <summary>
        /// Restaura estados de misiones desde un diccionario previo (load).
        /// </summary>
        public void ImportMissionStates(Dictionary<string, string> states)
        {
            _missionStates.Clear();
            foreach (var kvp in states)
            {
                _missionStates[kvp.Key] = kvp.Value;
            }
        }
    }
}