using _Project.Scripts.Animation;
using UnityEngine;

namespace _Project.Scripts.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(DirectionalSpriteAnimator))]
    public class NpcMovement : MonoBehaviour
    {
        public float moveSpeed = 2f;
        public Vector2 moveDir; // Se puede ir cambiando desde otro script de IA

        private Rigidbody2D _rigidbody2D;
        private DirectionalSpriteAnimator _directionalSpriteAnimator;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _directionalSpriteAnimator = GetComponent<DirectionalSpriteAnimator>();
        }

        private void FixedUpdate()
        {
            var dir = moveDir.normalized;
            var vel = dir * moveSpeed;
            _rigidbody2D.linearVelocity = vel;

            _directionalSpriteAnimator.UpdateFromMovement(dir, vel.magnitude);
        }
    }
}