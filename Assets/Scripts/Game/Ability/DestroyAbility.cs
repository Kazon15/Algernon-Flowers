using Util.Character;

namespace Game.Ability
{
    public sealed class DestroyAbility : AbstractAbility
    {
        protected override void OnAbilityClick()
        {
            var findObject = Player.GetClosestDestroyableObject(Distance);

            if(findObject == null) return;

            base.OnAbilityClick();

            findObject.Destroy();
        }
    }
}