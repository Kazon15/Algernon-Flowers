using Util.Character;

namespace Game.Ability
{
    public sealed class KillAbility : AbstractAbility
    {
        protected override void OnAbilityClick()
        {
            var enemy = Player.GetClosestEnemy(Distance);

            if(enemy == null) return;

            base.OnAbilityClick();

            enemy.Kill();
        }
    }
}