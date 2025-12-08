using System;
using UnityEngine;

namespace _Project.Scripts.Data.StaticModels
{
    [Serializable]
    public class FlagsRoot
    {
        public string version;
        public FlagData[] flags;
    }

    [Serializable]
    public class FlagData
    {
        public string id;           // "flag_academico_mision_asignada"
        public string description;  // descripción para debug / diseño
        public string scope;        // ej: "chapter_1", "global", etc.
        public bool defaultValue;   // valor inicial cuando se crea un save
    }
}