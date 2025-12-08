using System;
using UnityEngine;

namespace _Project.Scripts.Data.StaticModels
{
    [Serializable]
    public class MissionsRoot
    {
        public string version;
        public MissionData[] missions;
    }

    [Serializable]
    public class MissionData
    {
        public string id;          // "restore_academic_service"
        public int chapterId;      // 1 (matchea con ChapterData.id)
        public string name;
        public string description;

        // "not_started", "in_progress", "waiting_turn_in", "completed"
        public string initialState;

        public MissionStateData[] states;
        public MissionStepData[] steps;
    }

    [Serializable]
    public class MissionStateData
    {
        public string id;          // "not_started", "in_progress", etc.
        public string description; // Texto descriptivo opcional
    }

    [Serializable]
    public class MissionStepData
    {
        public string id;          // "step_1", "step_2", ...
        public string state;       // estado actual requerido para que este step aplique
        public string[] triggersOn; // flags que dispanan este cambio de estado
        public string setStateTo;  // estado al que se va a mover la misión
    }
}