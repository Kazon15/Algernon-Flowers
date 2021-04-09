using System.Collections.Generic;
using System.Linq;
using Game.Config;
using Game.Player;
using UnityEngine;
using UnityEngine.UI;
using Util.Character;

namespace UI
{
    public class GameScreen : MonoBehaviour
    {
        [SerializeField]
        private Slider healthSlider;

        [SerializeField]
        private Slider movementSlider;

        [SerializeField, Header("Progress Bar Config")]
        private Slider powerSlider;

        private List<Button> _buttons;

        private Player _player;

        private void Awake()
        {
            if(powerSlider == null)
                powerSlider = transform.Find("Resources").Find("PowerBar").GetComponent<Slider>();
            if(movementSlider == null)
                movementSlider = transform.Find("Resources").Find("MovementBar").GetComponent<Slider>();
            if(healthSlider == null) healthSlider = transform.Find("HealthBar").GetComponent<Slider>();

            _buttons = transform.Find("AbilityMenu").GetComponentsInChildren<Button>().ToList();

            _player = GameConfig.Instance.Player;
        }

        private void Update()
        {
            if(!Input.anyKeyDown) return;

            var keyCodes = GameConfig.Instance.AbilitySelectKeys;
            for(var i = 0; i < keyCodes.Count; i++)
            {
                if(!Input.GetKeyDown(keyCodes[i]) && _buttons[i]) continue;
                SelectAbility(_buttons[i]);
                break;
            }
        }

        private void LateUpdate()
        {
            powerSlider.value = _player.PowerResource;
            movementSlider.value = _player.MovementResource;
            healthSlider.value = _player.Health;
        }

        public void SelectAbility(Button clickedButton)
        {
            if(!clickedButton.interactable) return;

            foreach(var button in _buttons) button.interactable = true;

            clickedButton.interactable = false;

            _player.Ability = clickedButton.GetComponent<AbstractAbility>();

            _player.Arrow.Active = false;
        }
    }
}