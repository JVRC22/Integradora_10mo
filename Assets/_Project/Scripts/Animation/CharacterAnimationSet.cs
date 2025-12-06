using UnityEngine;

namespace _Project.Scripts.Animation
{
    [CreateAssetMenu(
        fileName = "CharacterAnimationSet",
        menuName = "Characters/Animation Set"
    )]
    public class CharacterAnimationSet : ScriptableObject
    {
        [Header("Idle")]
        public Sprite[] idleRight;
        public Sprite[] idleUp;
        public Sprite[] idleLeft;
        public Sprite[] idleDown;

        [Header("Walk")]
        public Sprite[] walkRight;
        public Sprite[] walkUp;
        public Sprite[] walkLeft;
        public Sprite[] walkDown;
    }
}