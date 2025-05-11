using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Singleton { get; private set; }

    private Camera _mainCamera;

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(Singleton);
        }

        _mainCamera = GetComponentInChildren<Camera>();
    }
}
