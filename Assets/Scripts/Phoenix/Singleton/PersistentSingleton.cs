using UnityEngine;

namespace Phoenix.Singleton
{
    public class PersistentSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        private static bool hasInstance => _instance != null;
        public static T TryGetInstance() => hasInstance ? _instance : null;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        var go = new GameObject(typeof(T).Name + " [Generated]");
                        _instance = go.AddComponent<T>();
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake() => InitializeSingleton();
        

        protected virtual void InitializeSingleton()
        {
            if(!Application.isPlaying) return;
            
            transform.SetParent(null);

            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                if(_instance != this)
                    Destroy(gameObject);
            }
            
        }
    }
}