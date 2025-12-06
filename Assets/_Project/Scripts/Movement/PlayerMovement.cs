using _Project.Scripts.Animation;
using UnityEngine;

namespace _Project.Scripts.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(DirectionalSpriteAnimator))]
    public class PlayerMovement : MonoBehaviour
    {
        public float moveSpeed = 3f;

        private Rigidbody2D _rigidbody2D;
        private DirectionalSpriteAnimator _directionalSpriteAnimator;

        private Vector2 _input;
        private Vector2 _velocity;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _directionalSpriteAnimator = GetComponent<DirectionalSpriteAnimator>();
        }

        private void Update()
        {
            _input.x = Input.GetAxisRaw("Horizontal");
            _input.y = Input.GetAxisRaw("Vertical");

            Vector2 moveDir = _input.normalized;
            _velocity = moveDir * moveSpeed;

            // Animación
            _directionalSpriteAnimator.UpdateFromMovement(moveDir, _velocity.magnitude);
        }

        private void FixedUpdate()
        {
            _rigidbody2D.linearVelocity = _velocity;
        }
    }
}