using System;

namespace _Project.Scripts.Data.StaticModels
{
    [Serializable]
    public class PuzzlesRoot
    {
        public string version;
        public PuzzleData[] puzzles;
    }

    [Serializable]
    public class PuzzleData
    {
        public int id;              // 101
        public string key;          // "academic_console_check"
        public string type;         // "interface", "logic", "timing", etc.
        public string title;        // "Consola del Servicio Académico"
        public string description;  // descripción larga
        public string difficulty;   // "facil", "media", "dificil"
        public string[] hints;      // pistas
        public string solution;     // "confirmar_estado_ok" (clave interna, no literal del input)
    }
}