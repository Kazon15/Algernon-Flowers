using System;
using Game.Config;
using Game.Player;
using UnityEngine;

namespace Util.Character
{
    public abstract class AbstractAbility : MonoBehaviour
    {
        [SerializeField]
        private float distance = 150f;

        [SerializeField]
        private int movementResource = 10;

        [SerializeField]
        private int powerResource = 10;

        [SerializeField]
        private bool useArrow;

        private bool _abilityUsed;

        private DateTime _slowDown;
        private bool _slowDownActive;

        public float Distance => distance;

        public bool UseArrow => useArrow;

        public bool ArrowInDistance { get; private set; }

        protected Player Player { get; private set; }

        protected GameConfig GameConfig { get; private set; }

        private void Awake()
        {
            GameConfig = GameConfig.Instance;
            Player = GameConfig.Player;
        }

        private void Update()
        {
            if(Player.Ability != this
            || Player.PowerResource < powerResource
            || Player.MovementResource < movementResource
            || GameConfig.GameLogic.Pause) return;

            var abilityKey = GameConfig.AbilityUseKey;
            if(Input.GetKeyDown(abilityKey))
            {
                if(UseArrow) Player.Arrow.Active = true;
                OnAbilityClick();
            }
            else if(Input.GetKeyUp(abilityKey))
            {
                if(UseArrow) Player.Arrow.Active = false;
                OnAbilityUse();
            }
        }

        private void LateUpdate()
        {
            if(!GameConfig.Instance.GameLogic.Pause && _slowDownActive && _slowDown <= DateTime.Now)
            {
                _slowDownActive = false;
                Time.timeScale = 1f;
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
            }

            if(Player.Ability != this || !UseArrow || !Player.Arrow.Active) return;

            if(Vector2.Distance(Player.transform.position, Player.MousePosition) > Distance)
            {
                ArrowInDistance = false;
                Player.Arrow.Color = Color.red;
            }
            else
            {
                var hit = Physics2D.Raycast(Player.MousePosition, Vector2.zero, 0f);

                if(!hit || !hit.transform.CompareTag("Ground"))
                {
                    ArrowInDistance = false;
                    Player.Arrow.Color = Color.red;
                }
                else
                {
                    ArrowInDistance = true;
                    Player.Arrow.Color = Color.white;
                }
            }
        }

        protected virtual void OnAbilityUse()
        {
            if(!_abilityUsed) return;

            Player.PowerResource -= powerResource;
            Player.MovementResource -= movementResource;

            _abilityUsed = false;

            Time.timeScale = GameConfig.AbilitySlowDown;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;

            _slowDownActive = true;
            _slowDown = DateTime.Now + TimeSpan.FromSeconds(GameConfig.AbilitySlowDownTime);
        }

        protected virtual void OnAbilityClick() => _abilityUsed = true;
    }
}