using UnityEngine;

namespace Game.Object
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField, Range(0f, 5000f)]
        private float speed = 500f;

        private void Awake() => InvokeRepeating(nameof(Destroy), 5f, 5f);

        private void FixedUpdate()
        {
            var bullet = transform;
            bullet.position += bullet.rotation * new Vector2(speed * Time.deltaTime, 0f);
        }

        private void Destroy() => Destroy(gameObject);
    }
}