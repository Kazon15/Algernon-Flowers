using System.Collections.Generic;
using System.Linq;
using Game.Logic;
using UnityEngine;
using Util;

namespace Game.Config
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Config/GameConfig")]
    public sealed class GameConfig : ScriptableObjectSingleton<GameConfig>
    {
        public enum ObjectileTypeEnum
        {
            KillAllEnemies
        }

        public enum SceneLoadTypeEnum
        {
            Revert,
            Single,
            Additive
        }

        public enum SceneTypeEnum
        {
            MainMenu = 0,
            EscapeMenu = 1,
            SettingsMenu = 2,
            Game = 3
        }

        [SerializeField]
        private List<KeyCode> abilitySelectKeys = new List<KeyCode>
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5
        };

        [SerializeField, Range(0f, 1f)]
        private float abilitySlowDown = 0.1f;

        [SerializeField, Range(0f, 10f)]
        private float abilitySlowDownTime = 4f;

        [SerializeField, Header("Abilities Config")]
        private KeyCode abilityUseKey = KeyCode.E;

        [SerializeField, Range(0f, 100f), Header("Characters Config")]
        private int maxCharacterHealth = 100;

        [SerializeField, Range(1f, 1000f)]
        private float mouseRadius = 50f;

        [SerializeField, Header("Objectile Config")]
        private ObjectileTypeEnum objectile = ObjectileTypeEnum.KillAllEnemies;

        [SerializeField, Header("Pause Config")]
        private KeyCode pauseKey = KeyCode.Escape;

        [SerializeField]
        private SceneLoadTypeEnum pauseSceneLoadType = SceneLoadTypeEnum.Additive;

        [SerializeField]
        private SceneTypeEnum pauseSceneType = SceneTypeEnum.EscapeMenu;

        [SerializeField, Range(1f, 1000f)]
        private float speedToHit = 150f;

        [SerializeField, Header("Game Config")]
        private KeyCode tacticalPauseKey = KeyCode.Space;

        private List<Enemy.Enemy> _enemies;
        private GameLogic _gameLogic;

        private Player.Player _player;

        public KeyCode TacticalPauseKey => tacticalPauseKey;
        public float SpeedToHit => speedToHit;
        public float MouseRadius => mouseRadius;
        public KeyCode PauseKey => pauseKey;
        public SceneTypeEnum PauseSceneType => pauseSceneType;
        public int PauseSceneTypeId => (int) pauseSceneType;
        public string PauseSceneTypeName => pauseSceneType.ToString();
        public SceneLoadTypeEnum PauseSceneLoadType => pauseSceneLoadType;
        public ObjectileTypeEnum Objectile => objectile;
        public int ObjectileId => (int) objectile;
        public string ObjectileName => objectile.ToString();
        public KeyCode AbilityUseKey => abilityUseKey;
        public float AbilitySlowDown => abilitySlowDown;
        public float AbilitySlowDownTime => abilitySlowDownTime;
        public List<KeyCode> AbilitySelectKeys => abilitySelectKeys;
        public int MaxCharacterHealth => maxCharacterHealth;

        public Player.Player Player =>
            _player == null ? _player = FindObjectOfType<Player.Player>() : _player;

        public List<Enemy.Enemy> Enemies =>
            _enemies ?? (_enemies = FindObjectsOfType<Enemy.Enemy>().ToList());

        public GameLogic GameLogic =>
            _gameLogic == null ? _gameLogic = FindObjectOfType<GameLogic>() : _gameLogic;
    }
}