using System;
using UnityEngine;

namespace _Project.Scripts.Data.StaticModels
{
    [Serializable]
    public class DialoguesRoot
    {
        public string version;
        public ConversationData[] conversations;
    }

    [Serializable]
    public class ConversationData
    {
        public string id;           // "prof_jimenez_mission_intro"
        public string key;          // mismo valor, pero deja abierta la puerta a otro id lógico
        public string description;  // texto descriptivo para diseño

        public int chapterId;       // 1
        public string zoneKey;      // "mist_forest", "lab_console", etc.

        public ConversationNodeData[] nodes;
    }

    [Serializable]
    public class ConversationNodeData
    {
        public string id;           // "n1", "n2", ...
        public string speakerId;    // "prof_jimenez", "choche", "byte"
        public string text;         // línea de diálogo

        // "id" del siguiente nodo o "end" para terminar
        public string nextId;

        // flags que se activan al terminar este nodo (cuando saltas a nextId)
        public string[] flagsOnComplete;
    }
}