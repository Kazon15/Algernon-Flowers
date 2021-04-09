using UnityEngine;

namespace Util.Character
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class AbstractMovement : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody2D body;

        [SerializeField]
        private Transform controller;

        [SerializeField]
        private float speed = 15f;

        public float Speed => speed;
        
        public Transform Controller => controller;

        private void Awake()
        {
            if(body == null) body = GetComponent<Rigidbody2D>();
            if(controller == null) controller = transform.Find("Controller");
        }

        protected void Rotate(Vector2 position)
        {
            var controllerPosition = controller.position;

            position -= new Vector2(controllerPosition.x, controllerPosition.y);

            var angle = Mathf.Atan2(position.y, position.x) * Mathf.Rad2Deg;

            controller.rotation = Quaternion.Slerp(controller.rotation,
                                                   Quaternion.AngleAxis(angle, Vector3.forward),
                                                   5f * Time.deltaTime);
        }

        protected void Move(float horizontal, float vertical) =>
            body.AddForce(new Vector2(horizontal, vertical) * (100f * speed));

        /*protected void MoveToPosition(Vector2 position)
        {
            var thisPosition = transform.position;
            body.AddForce((position - new Vector2(thisPosition.x, thisPosition.y)) * speed);
        }*/
    }
}