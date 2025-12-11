using UnityEngine;

public class PuzzlesLabManager : MonoBehaviour
{
    public bool isPuzzle1Active = false;
    public bool isPuzzle2Active = false;
    public bool isPuzzle3Active = false;
    public TMPro.TextMeshProUGUI statusText;
    public InteractableObjectManager exitDoorInteractable;

    public void ActivatePuzzle1()
    {
        if (!isPuzzle1Active)
        {
            isPuzzle1Active = true;
            Debug.Log("Puzzle 1 Activado/Reparado");
            UpdateStatus("Puzzle 1 Completado");
            CheckAllPuzzles();
            // SceneManager.LoadScene(3);
        }
        else
        {
            Debug.Log("El Puzzle 1 ya estaba activado");
        }
    }

    public void ActivatePuzzle2()
    {
        if (!isPuzzle2Active)
        {
            isPuzzle2Active = true;
            Debug.Log("Puzzle 2 Activado/Reparado");
            UpdateStatus("Puzzle 2 Completado");
            CheckAllPuzzles();
        }
        else
        {
            Debug.Log("El Puzzle 2 ya estaba activado");
        }
    }

    public void ActivatePuzzle3()
    {
        if (!isPuzzle3Active)
        {
            isPuzzle3Active = true;
            Debug.Log("Puzzle 3 Activado/Reparado");
            UpdateStatus("Puzzle 3 Completado");
            CheckAllPuzzles();
        }
        else
        {
            Debug.Log("El Puzzle 3 ya estaba activado");
        }
    }

    public void TryExit()
    {
        if (isPuzzle1Active && isPuzzle2Active && isPuzzle3Active)
        {
            Debug.Log("¡TODOS LOS PUZZLES COMPLETADOS! Saliendo del nivel...");
            UpdateStatus("Saliendo del nivel...");
            if (exitDoorInteractable != null)
            {
                exitDoorInteractable.Interact();
            }
        }
        else
        {
            Debug.Log("Aún faltan puzzles por completar.");
            if (exitDoorInteractable != null)
            {
                exitDoorInteractable.ShowMessage("Debes completar los 3 puzzles antes de salir.");
            }
            UpdateStatus("Debes completar los 3 puzzles antes de salir.");
        }
    }

    private void CheckAllPuzzles()
    {
        if (isPuzzle1Active && isPuzzle2Active && isPuzzle3Active)
        {
            Debug.Log("Todos los sistemas están en línea. La salida está desbloqueada.");
        }
    }

    private void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }
}
