using Game.Config;
using UnityEngine;
using UnityEngine.UI;

namespace Util
{
    public class Arrow : MonoBehaviour
    {
        [SerializeField]
        private Image image;

        private RectTransform _rect;

        public bool Active
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }

        public Color Color
        {
            get => image.color;
            set => image.color = value;
        }

        private void Awake()
        {
            if(image == null) image = GetComponent<Image>();

            _rect = GetComponent<RectTransform>();
        }

        private void LateUpdate()
        {
            var position = transform.parent.transform.position;

            var anchorPos = new Vector2(position.x, position.y);
            var currentPos = GameConfig.Instance.Player.MousePosition;

            var midPointVector = (currentPos + anchorPos) / 2;
            var relative = currentPos - anchorPos;
            var maggy = relative.magnitude;

            var angle = Mathf.Atan2(relative.y, relative.x) * Mathf.Rad2Deg;
            var rotate = Quaternion.AngleAxis(angle, Vector3.forward);

            var obj = transform;
            obj.rotation = rotate;
            obj.position = midPointVector;
            _rect.sizeDelta = new Vector2(maggy < 120f ? 0f : maggy, _rect.sizeDelta.y);
        }
    }
}