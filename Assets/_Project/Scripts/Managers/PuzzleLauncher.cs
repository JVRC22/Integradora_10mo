using _Project.Scripts.Data.StaticModels;
using _Project.Scripts.Services.Data;
using UnityEngine;

namespace _Project.Scripts.Managers
{
    public class PuzzleLauncher : MonoBehaviour
    {
        public static PuzzleLauncher Instance { get; private set; }

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

        public void StartPuzzle(string puzzleKey)
        {
            var puzzle = StaticDataService.Instance.GetPuzzleByKey(puzzleKey);
            if (puzzle == null)
            {
                Debug.LogError($"[PuzzleLauncher] Puzzle no encontrado: {puzzleKey}");
                return;
            }

            switch (puzzle.key)
            {
                case "academic_console_check":
                    LaunchAcademicConsolePuzzle(puzzle);
                    break;
            }
        }

        private void LaunchAcademicConsolePuzzle(PuzzleData data)
        {
            Debug.Log("[PuzzleLauncher] Lanzando puzzle: " + data.title);

            // Aquí:
            // - Cargar escena del puzzle
            // - Activar canvas
            // - Instanciar prefab
            // - Cambiar estado del juego
        }
    }
}