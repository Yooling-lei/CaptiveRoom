using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance { get; private set; }
    public static bool IsInitialized => Instance is not null;


    protected virtual void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = (T)this;
    }

    protected void OnDestroy()
    {
        
        if (Instance == this) Instance = null;
    }
}