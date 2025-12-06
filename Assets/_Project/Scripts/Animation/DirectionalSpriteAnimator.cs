using UnityEngine;

namespace _Project.Scripts.Animation
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class DirectionalSpriteAnimator : MonoBehaviour
    {
        public enum AnimState { Idle, Walk }
        public enum Direction { Up, Down, Left, Right }

        [Header("Config")]
        public CharacterAnimationSet animationSet;
        public float framesPerSecond = 12f;

        private SpriteRenderer _spriteRenderer;

        [SerializeField] private AnimState currentState = AnimState.Idle;
        [SerializeField] private Direction currentDirection = Direction.Down;

        private float _frameTimer;
        private int _currentFrameIndex;

        private Vector2 _lastMoveDir = Vector2.down;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void UpdateFromMovement(Vector2 moveDir, float speed)
        {
            // Determinar estado
            if (speed > 0.01f)
            {
                currentState = AnimState.Walk;

                // Decidir dirección dominante
                if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
                {
                    currentDirection = moveDir.x > 0 ? Direction.Right : Direction.Left;
                }
                else
                {
                    currentDirection = moveDir.y > 0 ? Direction.Up : Direction.Down;
                }

                _lastMoveDir = moveDir;
            }
            else
            {
                currentState = AnimState.Idle;

                // Si está quieto, mantiene la última dirección
                if (_lastMoveDir.sqrMagnitude > 0.001f)
                {
                    if (Mathf.Abs(_lastMoveDir.x) > Mathf.Abs(_lastMoveDir.y))
                    {
                        currentDirection = _lastMoveDir.x > 0 ? Direction.Right : Direction.Left;
                    }
                    else
                    {
                        currentDirection = _lastMoveDir.y > 0 ? Direction.Up : Direction.Down;
                    }
                }
            }
        }
        
        private void Update()
        {
            var currentAnim = GetCurrentAnimationArray();
            if (currentAnim == null || currentAnim.Length == 0)
                return;

            _frameTimer += Time.deltaTime;
            var frameDuration = 1f / framesPerSecond;

            if (_frameTimer >= frameDuration)
            {
                _frameTimer -= frameDuration;
                _currentFrameIndex = (_currentFrameIndex + 1) % currentAnim.Length;
            }

            _spriteRenderer.sprite = currentAnim[_currentFrameIndex];
        }
        
        private Sprite[] GetCurrentAnimationArray()
        {
            if (!animationSet)
                return null;

            switch (currentState)
            {
                case AnimState.Idle:
                    switch (currentDirection)
                    {
                        case Direction.Down: return animationSet.idleDown;
                        case Direction.Up: return animationSet.idleUp;
                        case Direction.Left: return animationSet.idleLeft;
                        case Direction.Right: return animationSet.idleRight;
                    }
                    break;

                case AnimState.Walk:
                    switch (currentDirection)
                    {
                        case Direction.Down: return animationSet.walkDown;
                        case Direction.Up: return animationSet.walkUp;
                        case Direction.Left: return animationSet.walkLeft;
                        case Direction.Right: return animationSet.walkRight;
                    }
                    break;
            }

            return null;
        }
    }
}
