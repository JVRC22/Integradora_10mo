using UnityEngine;
using TMPro;

public class InteractableObjectManager : MonoBehaviour
{
    public string interactionMessage = "Has interactuado con este objeto";
    public bool destroyAfterUse = false;
    public bool disableAfterUse = true;    
    public Sprite afterInteractionSprite;
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;
    public float displayDuration = 3f;
    private SpriteRenderer spriteRenderer;
    private bool hasBeenUsed = false;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }
    }

    public void Interact()
    {
        if (hasBeenUsed && disableAfterUse)
        {
            ShowMessage("Ya has usado este objeto");
            return;
        }

        hasBeenUsed = true;

        if (afterInteractionSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = afterInteractionSprite;
        }

        ShowMessage(interactionMessage);
        
        if (disableAfterUse)
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = false;
            }
        }

        if (destroyAfterUse)
        {
            Destroy(gameObject, displayDuration);
        }
    }

    private void ShowMessage(string message)
    {
        Debug.Log($"ShowMessage llamado con: {message}");
        Debug.Log($"Panel es null? {messagePanel == null}, Text es null? {messageText == null}");
        
        if (messagePanel != null && messageText != null)
        {
            Debug.Log("Mostrando mensaje en UI");
            messageText.text = message;
            messagePanel.SetActive(true);
            CancelInvoke("HideMessage");
            Invoke("HideMessage", displayDuration);
        }
        else
        {
            Debug.Log(message);
        }
    }

    private void HideMessage()
    {
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }
    }
}
