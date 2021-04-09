using System;
using Game.Config;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Logic
{
    public sealed class GameLogic : MonoBehaviour
    {
        public delegate void OnEscapeDelegate();

        public delegate void OnPauseDelegate(bool paused);

        private bool _pause;

        private bool _tacticalPause;

        public bool Pause
        {
            get => _pause;
            set
            {
                if(SceneManager.GetActiveScene().buildIndex != (int) GameConfig.SceneTypeEnum.Game) return;

                _pause = value;

                if(!_pause)
                {
                    Debug.Log("Game Continue.");

                    Time.timeScale = 1f;
                }
                else
                {
                    Debug.Log("Game Pause");

                    Time.timeScale = 0f;
                }

                OnPauseEvent?.Invoke(_pause);
            }
        }

        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            OnEscapeEvent += OnEscape;
        }

        private void Update()
        {
            if(Input.GetKeyDown(GameConfig.Instance.PauseKey)) OnEscapeEvent?.Invoke();
            if(Input.GetKeyDown(GameConfig.Instance.TacticalPauseKey)) _tacticalPause = Pause = !Pause;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            OnEscapeEvent -= OnEscape;

            Time.timeScale = 1f;
        }

        public event OnEscapeDelegate OnEscapeEvent;
        public event OnPauseDelegate OnPauseEvent;

        private void OnEscape()
        {
            var sceneType = GameConfig.Instance.PauseSceneTypeId;

            if(SceneManager.GetActiveScene().buildIndex == sceneType)
            {
                SceneManager.UnloadSceneAsync(sceneType);
                return;
            }

            switch(GameConfig.Instance.PauseSceneLoadType)
            {
                case GameConfig.SceneLoadTypeEnum.Revert:
                    SceneManager.UnloadSceneAsync(sceneType);
                    break;
                case GameConfig.SceneLoadTypeEnum.Single:
                    SceneManager.LoadScene(sceneType, LoadSceneMode.Single);
                    break;
                case GameConfig.SceneLoadTypeEnum.Additive:
                    SceneManager.LoadScene(sceneType, LoadSceneMode.Additive);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if(mode == LoadSceneMode.Single) _tacticalPause = Pause = false;
            else
            {
                if(scene.buildIndex != GameConfig.Instance.PauseSceneTypeId) return;

                if(!Pause) Pause = true;

                SceneManager.SetActiveScene(scene);
            }
        }

        private void OnSceneUnloaded(Scene scene)
        {
            if(scene.buildIndex != GameConfig.Instance.PauseSceneTypeId) return;

            if(Pause && !_tacticalPause) Pause = false;
        }

        public void GetObjectile()
        {
            switch(GameConfig.Instance.Objectile)
            {
                case GameConfig.ObjectileTypeEnum.KillAllEnemies:
                    if(GameConfig.Instance.Player.Health == 0)
                    {
                        Debug.Log("You Loose");
                        Pause = true;
                    }
                    else if(GameConfig.Instance.Enemies.Count == 0)
                    {
                        Debug.Log("You Win");
                        Pause = true;
                    }

                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}