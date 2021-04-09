using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Util
{
    public sealed class ScriptableSingletonsStorage : MonoBehaviour
    {
        private static ScriptableSingletonsStorage _instance;

        [SerializeField]
        private List<ScriptableObject> singletons = new List<ScriptableObject>();

        public static ScriptableSingletonsStorage Instance
        {
            get
            {
                if(_instance != null) return _instance;

                _instance = FindObjectOfType<ScriptableSingletonsStorage>();
                //DontDestroyOnLoad(instance);

                return _instance;
            }
        }

        public List<ScriptableObject> Singletons => singletons;

        private void Awake()
        {
            if(_instance == null)
            {
                _instance = this;
                //DontDestroyOnLoad(instance);
            }
            else if(_instance != this)
            {
                _instance.singletons.AddRange(singletons);
                Destroy(this);
            }
        }

        public T GetInstance<T>() where T : ScriptableObject
        {
            foreach(var t in singletons.OfType<T>()) return t;

            Debug.LogErrorFormat($"Singleton {typeof(T)} is not in storage!");
            return null;
        }
    }
}