using UnityEngine;

public class SceneMusicTrigger : MonoBehaviour
{
    [SerializeField] private AudioClip trackForThisScene;
    [SerializeField] private float fadeDuration = 1f;

    private void Start()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayTrack(trackForThisScene, fadeDuration);
        }
    }
}
