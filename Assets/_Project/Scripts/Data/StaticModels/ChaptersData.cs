using System;

namespace _Project.Scripts.Data.StaticModels
{
    [Serializable]
    public class ChaptersRoot
    {
        public string version;
        public ChapterData[] chapters;
    }

    [Serializable]
    public class ChapterData
    {
        public int id;
        public string key;
        public string name;
        public string description;
        public ZoneData[] zones;
    }

    [Serializable]
    public class ZoneData
    {
        public int id;
        public string key;
        public string name;
        public string description;

        // "locked", "unlocked"
        public string initialState;

        public ZoneUnlockRequirement[] unlockRequirements;
        public ZoneObjective[] objectives;
        public ZoneExplorationItem[] explorationItems;
    }

    [Serializable]
    public class ZoneUnlockRequirement
    {
        public string type;   // ej: "flag_active"
        public string flagId; // ej: "flag_academico_mision_asignada"
    }

    [Serializable]
    public class ZoneObjective
    {
        public string id;
        public string description;

        // Aquí lo ideal es que en el JSON NO se use null, sino "" o se omita.
        public string puzzleKey;      // ej: "puzzle_101_academic_status" o ""
        public string rewardItemKey;  // ej: "item_pass_lab" o ""

        public string[] flagsOnComplete;
    }

    [Serializable]
    public class ZoneExplorationItem
    {
        public string id;
        public string itemKey;
        public string description;
        public bool oneTime;
    }
}