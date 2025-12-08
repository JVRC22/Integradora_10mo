using System;
using UnityEngine;

namespace _Project.Scripts.Data.StaticModels
{
    [Serializable]
    public class CharactersRoot
    {
        public string version;
        public CharacterData[] characters;
    }

    [Serializable]
    public class CharacterData
    {
        public string id;           // "choche", "byte", "prof_jimenez"
        public string key;          // "protagonist", "ai_assistant", etc.
        public string displayName;  // "Choche"
        public string role;         // "protagonista", "npc", "asistente_virtual"
        public string description;
        public bool isPlayable;
        public string[] tags;
    }
}