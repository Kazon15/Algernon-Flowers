using Game.Config;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Object
{
    public sealed class Destroyable : MonoBehaviour
    {
        public delegate void OnDestroyDelegate();

        [SerializeField, ShowIf("createChildsAfterDestroy"), MinMaxSlider(0, 100)]
        private Vector2 childCount = new Vector2(3, 4);

        [SerializeField, ShowIf("createChildsAfterDestroy"), MinMaxSlider(0f, 500f)]
        private Vector2 childSize = new Vector2(50f, 50f);

        [SerializeField, ShowIf("createChildsAfterDestroy"), MinMaxSlider(0f, 1000f)]
        private Vector2 childSpawnRadius = new Vector2(50f, 150f);

        [SerializeField]
        private bool createChildsAfterDestroy = true;

        private void OnCollisionEnter2D(Collision2D other)
        {
            Debug.Log($"OnCollisionEnter2D: relativeVelocity = {other.relativeVelocity.magnitude}");
            if(other.relativeVelocity.magnitude >= GameConfig.Instance.SpeedToHit) Destroy();
        }

        public event OnDestroyDelegate OnDestroyEvent;

        public void Destroy()
        {
            if(createChildsAfterDestroy)
            {
                for(var i = 0; i < Random.Range(childCount.x, childCount.y); i++)
                {
                    var offset = Random.insideUnitCircle
                               * Random.Range(childSpawnRadius.x, childSpawnRadius.y);

                    var obj = transform;
                    var child = Instantiate(gameObject, obj.position, obj.rotation, obj.parent);

                    Destroy(child.GetComponent<Destroyable>());

                    child.GetComponent<BoxCollider2D>().size =
                        child.GetComponent<RectTransform>().sizeDelta = childSize;

                    var obstacle = child.GetComponent<NavMeshObstacle>();

                    if(!obstacle.enabled) obstacle.enabled = true;

                    obstacle.size = new Vector3(childSize.x, childSize.y, obstacle.size.z);

                    var newPosition = obj.position + new Vector3(offset.x, offset.y, 0f);

                    var body = child.GetComponent<Rigidbody2D>();

                    if(body == null)
                    {
                        body = child.AddComponent<Rigidbody2D>();
                        body.bodyType = RigidbodyType2D.Dynamic;
                        body.simulated = true;
                        body.mass = 2f;
                        body.drag = 10f;
                        body.angularDrag = body.gravityScale = 0f;
                        body.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
                        body.sleepMode = RigidbodySleepMode2D.StartAwake;
                        body.interpolation = RigidbodyInterpolation2D.None;
                        body.constraints = RigidbodyConstraints2D.FreezeRotation;
                    }

                    body.mass /= 2;
                    body.AddForce((newPosition - obj.position) * 20f, ForceMode2D.Impulse);
                }
            }

            OnDestroyEvent?.Invoke();
            Destroy(gameObject);
        }
    }
}