using UnityEngine;

namespace Utils
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static bool _applicationIsQuitting = false;

        public static T Instance
        {
            get 
            {
                if (_applicationIsQuitting)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                        "' já destruída na saída da aplicação. Não será criada novamente - retornando null.");
                    return null;
                }
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<T>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject(typeof(T).Name);
                        _instance = go.AddComponent<T>();
                        Debug.Log("Uma instância de " + typeof(T) +
                            " é necessária, então " + go.name + " foi criado com DontDestroyOnLoad.");
                    }
                }
                return _instance; 
            } 
        }

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this) _applicationIsQuitting = true;
        }
    }
}
