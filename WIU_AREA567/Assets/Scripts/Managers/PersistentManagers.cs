using UnityEngine;

public class PersistentManagers : MonoBehaviour
{
    private static PersistentManagers existingInstance;
    private void Awake()
    {
        if (existingInstance != null && existingInstance != this)
        {
            Destroy(gameObject);
            return;
        }
        existingInstance = this;
        DontDestroyOnLoad(gameObject);
    }
}
