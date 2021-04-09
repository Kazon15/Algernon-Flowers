using Game.Config;
using UnityEngine;
using Util.Character;

namespace Game.Player
{
    public sealed class PlayerMovement : AbstractMovement
    {
        private Vector3 _cameraOffset;
        private Player _player;

        private void Start()
        {
            _player = GameConfig.Instance.Player;
            _cameraOffset = _player.CameraTarget.transform.position - transform.position;
        }

        private void FixedUpdate()
        {
            _player.CameraTarget.transform.position = Vector3.Lerp(
                _player.CameraTarget.transform.position, transform.position + _cameraOffset,
                1f * Time.deltaTime);

            Rotate(_player.MousePosition);

            Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
    }
}