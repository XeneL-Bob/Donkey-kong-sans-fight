using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Prevents this object from being destroyed when loading a new scene
        }
        else
        {
            Destroy(gameObject); // Destroy the new instance if one already exists
        }
    }
}
