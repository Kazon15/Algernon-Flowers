using UnityEngine;

namespace Util
{
    public class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if(_instance != null) return _instance;

                #if !UNITY_EDITOR
                _instance = Resources.FindObjectsOfTypeAll<T>().First();
                #else
                _instance = ScriptableSingletonsStorage.Instance.GetInstance<T>();
                #endif

                return _instance;
            }
        }
    }
}