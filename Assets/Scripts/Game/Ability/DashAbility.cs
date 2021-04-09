using NaughtyAttributes;
using UnityEngine;
using Util.Character;

namespace Game.Ability
{
    public sealed class DashAbility : AbstractAbility
    {
        [SerializeField, ShowIf("useArrow")]
        private float speed = 10f;

        protected override void OnAbilityUse()
        {
            if(!ArrowInDistance) return;

            var body = Player.GetComponent<Rigidbody2D>();

            var position = Player.GetArrowPosition(Distance);

            body.AddForceAtPosition(Player.GetArrowDirection(position) * speed, position,
                                    ForceMode2D.Impulse);

            base.OnAbilityUse();
        }
    }
}