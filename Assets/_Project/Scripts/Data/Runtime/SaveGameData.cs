using System;
using UnityEngine;

namespace _Project.Scripts.Data.Runtime
{
    [Serializable]
    public class SaveGameData
    {
        public string version;
        public SaveMetadata metadata;
        public SavePlayerState playerState;
        public SaveProgress progress;
    }

    [Serializable]
    public class SaveMetadata
    {
        public string createdAt;     // ISO8601
        public string lastUpdatedAt; // ISO8601
    }

    [Serializable]
    public class SavePlayerState
    {
        public int currentChapterId;   // 1
        public string currentZoneKey;  // "lab_console"
        public SavePosition position;
        public string direction;       // "up", "down", "left", "right"
    }

    [Serializable]
    public class SavePosition
    {
        public float x;
        public float y;
    }

    [Serializable]
    public class SaveProgress
    {
        public string[] activeFlags;
        public SaveMissionProgress[] missions;
        public string[] completedObjectives;
        public string[] zones;
    }

    [Serializable]
    public class SaveMissionProgress
    {
        public string missionId;      // "restore_academic_service"
        public string state;          // "waiting_turn_in"
        public string lastStepId;     // "step_2" (por ahora lo vamos a dejar simple)
        public string lastUpdatedAt;  // ISO8601
    }
}