using System;
using Game.Config;
using Game.Enemy;
using UnityEngine;

namespace Util.Character
{
    public abstract class AbstractCharacter : MonoBehaviour
    {
        [SerializeField]
        private int health = 100;

        [SerializeField]
        private GameObject stick;

        public GameObject Stick => stick;

        public int Health
        {
            get => health;
            set
            {
                health = Mathf.Clamp(value, 0, GameConfig.Instance.MaxCharacterHealth);

                onHealthChange?.Invoke();

                if(health > 0) return;

                var enemy = this as Enemy;
                if(enemy != null) GameConfig.Instance.Enemies.Remove(enemy);

                GameConfig.Instance.GameLogic.GetObjectile();

                onDeath?.Invoke();

                if(enemy != null) Destroy(gameObject);
            }
        }

        public void Kill() => Health = 0;

        public Action onDeath;
        public Action onHealthChange;
    }
}