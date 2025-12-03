using UnityEngine;

public class PlayerMovementManager : MonoBehaviour
{
    public float moveSpeed = 3f;
    public LayerMask obstacleLayer;
    public Vector2 colliderSize = new Vector2(0.4f, 0.4f);
    public Vector2 colliderOffset = Vector2.zero;
    public Animator animator;
    
    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 lastDirection = Vector2.down;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        
        if (movement.x != 0) movement.y = 0;
        
        if (movement != Vector2.zero)
        {
            lastDirection = movement.normalized;
            animator.SetFloat("InputX", movement.x);
            animator.SetFloat("InputY", movement.y);
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
            animator.SetFloat("InputX", lastDirection.x);
            animator.SetFloat("InputY", lastDirection.y);
        }
    }

    private void FixedUpdate()
    {
        if (movement != Vector2.zero)
        {
            Vector2 targetPosition = rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime;
            
            if (!CheckCollision(targetPosition))
            {
                rb.MovePosition(targetPosition);
            }
        }
    }

    private bool CheckCollision(Vector2 targetPosition)
    {
        Collider2D hit = Physics2D.OverlapBox(
            targetPosition + colliderOffset,
            colliderSize,
            0f,
            obstacleLayer
        );
        
        return hit != null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 pos = transform.position;
        Gizmos.DrawWireCube(pos + (Vector3)colliderOffset, colliderSize);
    }
}
