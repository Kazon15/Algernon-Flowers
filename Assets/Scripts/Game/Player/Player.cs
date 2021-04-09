using System.Linq;
using Game.Config;
using Game.Object;
using UnityEngine;
using Util;
using Util.Character;

namespace Game.Player
{
    public sealed class Player : AbstractCharacter
    {
        public AbstractAbility Ability;

        [SerializeField]
        private Arrow arrow;

        public int MovementResource = 100;
        public int PowerResource = 100;

        public Arrow Arrow => arrow;

        public Camera CameraTarget { get; private set; }
        public Collider2D ColliderTarget { get; private set; }

        public Vector2 MousePosition =>
            CameraTarget.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));

        private void Awake()
        {
            CameraTarget = Camera.main;
            ColliderTarget = GetComponent<Collider2D>();

            if(arrow == null) arrow = transform.GetComponentInChildren<Arrow>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            Debug.Log($"OnCollisionEnter2D: relativeVelocity = {other.relativeVelocity.magnitude}");
            if(other.gameObject.GetComponent<Enemy.Enemy>() == null
            && other.relativeVelocity.magnitude >= GameConfig.Instance.SpeedToHit) Health -= 20;
        }

        public Rigidbody2D GetClosestGameObject(float distance) => FindObjectsOfType<Rigidbody2D>()?.ToList()
           .FindLast(e => e.gameObject != gameObject
                       && Vector2.Distance(GetArrowPosition(distance), e.transform.position)
                       <= GameConfig.Instance.MouseRadius);

        public Enemy.Enemy GetClosestEnemy(float distance) => FindObjectsOfType<Enemy.Enemy>()?.ToList()
           .FindLast(e => Vector2.Distance(GetArrowPosition(distance), e.transform.position)
                       <= GameConfig.Instance.MouseRadius);

        public Destroyable GetClosestDestroyableObject(float distance) => FindObjectsOfType<Destroyable>()
          ?.ToList().FindLast(e => Vector2.Distance(GetArrowPosition(distance), e.transform.position)
                                <= GameConfig.Instance.MouseRadius);

        public Vector2 GetArrowPosition(float distance) => Vector2.MoveTowards(transform.position,
            CameraTarget.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y)),
            distance);

        public Vector2 GetArrowDirection(Vector2 position)
        {
            var playerPosition = transform.position;
            return position - new Vector2(playerPosition.x, playerPosition.y);
        }
    }
}