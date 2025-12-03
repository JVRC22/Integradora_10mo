using UnityEngine;

public class PlayerInteractionManager : MonoBehaviour
{
    [Header("Interaction Settings")]
    public LayerMask interactableLayer;
    public float interactionRange = 1f;

    private InteractableManager currentInteractable;

    private void Update()
    {
        CheckForInteractable();

        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.SendMessage("Interact", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void CheckForInteractable()
    {
        // Detectar objetos interactuables cercanos
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactionRange, interactableLayer);

        if (hits.Length > 0)
        {
            // Tomar el interactuable más cercano
            float closestDistance = Mathf.Infinity;
            InteractableManager closest = null;

            foreach (Collider2D hit in hits)
            {
                InteractableManager interactable = hit.GetComponent<InteractableManager>();
                if (interactable != null)
                {
                    float distance = Vector2.Distance(transform.position, hit.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closest = interactable;
                    }
                }
            }

            if (closest != currentInteractable)
            {
                currentInteractable = closest;
            }
        }
        else
        {
            currentInteractable = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualizar el rango de interacción en el editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
