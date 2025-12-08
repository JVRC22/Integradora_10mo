using System;
using UnityEngine;

namespace _Project.Scripts.Data.StaticModels
{
    [Serializable]
    public class CharacterDialogueRoutesRoot
    {
        public string version;
        public CharacterDialogueRoutesForCharacter[] routes;
    }

    [Serializable]
    public class CharacterDialogueRoutesForCharacter
    {
        public string characterId; // "prof_jimenez", "byte", etc.
        public DialogueRouteCondition[] routes;
    }

    [Serializable]
    public class DialogueRouteCondition
    {
        // "mission_state", "zone_enter", "default", etc.
        public string type;

        // Para type = "mission_state"
        public string missionId;  // "restore_academic_service"
        public string state;      // "not_started", "in_progress", etc.

        // Para type = "zone_enter"
        public string zoneKey;    // "lab_console"

        // ID de la conversación a ejecutar cuando matchea
        public string conversationId;
    }
}