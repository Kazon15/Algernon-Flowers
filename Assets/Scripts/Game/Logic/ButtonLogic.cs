using System;
using Game.Config;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Logic
{
    [RequireComponent(typeof(Button))]
    public sealed class ButtonLogic : MonoBehaviour
    {
        [SerializeField]
        private GameConfig.SceneLoadTypeEnum sceneLoadType = GameConfig.SceneLoadTypeEnum.Single;

        [SerializeField]
        private GameConfig.SceneTypeEnum sceneType = GameConfig.SceneTypeEnum.MainMenu;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();

            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            switch(sceneLoadType)
            {
                case GameConfig.SceneLoadTypeEnum.Revert:
                    SceneManager.UnloadSceneAsync((int) sceneType);
                    break;
                case GameConfig.SceneLoadTypeEnum.Single:
                    SceneManager.LoadScene((int) sceneType, LoadSceneMode.Single);
                    break;
                case GameConfig.SceneLoadTypeEnum.Additive:
                    if(SceneManager.GetSceneByBuildIndex((int) sceneType).isLoaded) return;
                    SceneManager.LoadScene((int) sceneType, LoadSceneMode.Additive);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}