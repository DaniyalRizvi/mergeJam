using UnityEngine;

public class Singelton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object Lock = new object();
    private static bool _isShuttingDown;
    public static T Instance
    {
        get
        {
            if (_isShuttingDown)
            {
                Debug.LogWarning($"Instance of {typeof(T)} is being destroyed. Returning null.");
                return null; 
            }

            lock (Lock)
            {
                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        lock (Lock)
        {
            _instance = this as T;
        }
    }

    private void OnApplicationQuit()
    {
        _isShuttingDown = true; 
    }
}