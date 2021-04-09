using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using Util.Character;

namespace Game.Ability
{
    public sealed class JumpAbility : AbstractAbility
    {
        [SerializeField, ShowIf("useArrow")]
        private float speed = 10f;

        private Collider2D _playerCollider;

        private Coroutine _trigger;

        protected override void OnAbilityUse()
        {
            if(!ArrowInDistance) return;

            var body = Player.GetComponent<Rigidbody2D>();

            _playerCollider = Player.GetComponent<Collider2D>();

            _playerCollider.isTrigger = true;

            var position = Player.GetArrowPosition(Distance);

            body.AddForceAtPosition(Player.GetArrowDirection(position) * speed, position,
                                    ForceMode2D.Impulse);

            _trigger = StartCoroutine(DisableTrigger());

            base.OnAbilityUse();
        }

        private IEnumerator DisableTrigger()
        {
            yield return new WaitForSeconds(1f);
            _playerCollider.isTrigger = false;
            StopCoroutine(_trigger);
        }
    }
}