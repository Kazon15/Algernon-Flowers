using NaughtyAttributes;
using UnityEngine;
using Util.Character;

namespace Game.Ability
{
    public sealed class TossAbility : AbstractAbility
    {
        [SerializeField, ShowIf("useArrow")]
        private float speed = 10f;

        private Collider2D _findCollider;
        private Rigidbody2D _findObject;

        private bool _stick;

        private void FixedUpdate()
        {
            if(!_stick)
            {
                if(_findObject && _findCollider.isTrigger && !_findCollider.IsTouching(Player.ColliderTarget))
                    _findCollider.isTrigger = false;
                return;
            }

            _findObject.MovePosition(Player.Stick.transform.position);
        }

        protected override void OnAbilityClick()
        {
            _findObject = Player.GetClosestGameObject(Distance);

            if(_findObject == null) return;

            _findCollider = _findObject.GetComponent<Collider2D>();

            _findCollider.isTrigger = _stick = true;

            _findObject.MovePosition(Player.Stick.transform.position);

            base.OnAbilityClick();
        }

        protected override void OnAbilityUse()
        {
            if(_findObject == null) return;

            if(!ArrowInDistance)
            {
                _stick = _findCollider.isTrigger = false;
                return;
            }

            _stick = false;

            var position = Player.GetArrowPosition(Distance);
            _findObject.AddForceAtPosition(Player.GetArrowDirection(position) * speed, position,
                                           ForceMode2D.Impulse);

            base.OnAbilityUse();
        }
    }
}