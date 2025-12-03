using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class InteractableManager : MonoBehaviour
{
    public string interactionPrompt = "Presiona E para interactuar";
    public UnityEvent onInteract;
    public GameObject promptPanel;
    public TextMeshProUGUI promptText;

    private bool playerInRange = false;
    private static GameObject sharedPromptPanel;
    private static TextMeshProUGUI sharedPromptText;

    private void Awake()
    {
        if (sharedPromptPanel == null && promptPanel != null)
        {
            sharedPromptPanel = promptPanel;
            sharedPromptText = promptText;
        }
        
        if (promptPanel != null)
        {
            promptPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void Interact()
    {
        onInteract?.Invoke();
        Invoke("DelayedHidePrompt", 0.1f);
        Debug.Log($"Interactuando con {gameObject.name}");
    }

    private void DelayedHidePrompt()
    {
        HidePrompt();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entró al trigger");
            playerInRange = true;
            ShowPrompt();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player salió del trigger");
            playerInRange = false;
            HidePrompt();
        }
    }

    private void ShowPrompt()
    {
        Debug.Log("ShowPrompt llamado");
        if (sharedPromptPanel != null && sharedPromptText != null)
        {
            Debug.Log("Activando panel");
            sharedPromptText.text = interactionPrompt;
            sharedPromptPanel.SetActive(true);
        }
        else
        {
            Debug.Log("Panel o texto es null");
        }
    }

    private void HidePrompt()
    {
        Debug.Log("HidePrompt llamado");
        if (sharedPromptPanel != null)
        {
            Debug.Log("Desactivando panel");
            sharedPromptPanel.SetActive(false);
        }
    }
}
