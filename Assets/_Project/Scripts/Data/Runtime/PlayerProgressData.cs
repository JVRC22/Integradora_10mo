using System;

namespace _Project.Scripts.Data.Runtime
{
    [Serializable]
    public class PlayerProgressData
    {
        public string profileId;        // opcional, por ahora lo puedes dejar vacío

        public string chapterKey;       // ej: "chapter_1"
        public string currentZoneKey;   // ej: "mist_forest"
        public string currentSceneName; // ej: "Lab_Nodo01" (cuando tengas nombres definidos)

        public SerializableFlagState[] flags;
        public SerializableMissionState[] missions;
    }

    [Serializable]
    public class SerializableFlagState
    {
        public string id;
        public bool value;
    }

    [Serializable]
    public class SerializableMissionState
    {
        public string id;
        public string state;
    }
}