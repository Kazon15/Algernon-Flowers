using Game.Config;
using UnityEngine;
using Util.Character;

namespace Game.Enemy
{
    public sealed class Enemy : AbstractCharacter
    {
        [SerializeField]
        private GameObject bulletPrefab;

        private void Awake() => InvokeRepeating(nameof(Shot), 3f, 3f);

        private void OnCollisionEnter2D(Collision2D other)
        {
            Debug.Log($"OnCollisionEnter2D: relativeVelocity = {other.relativeVelocity.magnitude}");
            if(other.gameObject.GetComponent<Player.Player>() == null
            && other.relativeVelocity.magnitude >= GameConfig.Instance.SpeedToHit) Kill();
        }

        private void Shot() => Instantiate(bulletPrefab, Stick.transform.position,
                                           GetComponent<EnemyMovement>().Controller.rotation,
                                           gameObject.transform.parent);
    }
}