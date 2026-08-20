using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] private AudioSource sourceA;
    [SerializeField] private AudioSource sourceB;
    [SerializeField] private float defaultFadeDuration = 1f;
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 0.5f;

    private AudioSource activeSource;
    private AudioSource inactiveSource;
    private AudioClip currentClip;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (sourceA == null) sourceA = gameObject.AddComponent<AudioSource>();
        if (sourceB == null) sourceB = gameObject.AddComponent<AudioSource>();

        sourceA.loop = true;
        sourceB.loop = true;
        sourceA.playOnAwake = false;
        sourceB.playOnAwake = false;
        sourceA.volume = 0f;
        sourceB.volume = 0f;

        activeSource = sourceA;
        inactiveSource = sourceB;
    }

    public void PlayTrack(AudioClip clip, float fadeDuration = -1f)
    {
        if (clip == null || clip == currentClip) return;

        currentClip = clip;
        float duration = fadeDuration < 0f ? defaultFadeDuration : fadeDuration;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(CrossfadeTo(clip, duration));
    }

    public void StopMusic(float fadeDuration = -1f)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        float duration = fadeDuration < 0f ? defaultFadeDuration : fadeDuration;
        fadeRoutine = StartCoroutine(FadeOut(duration));
    }

    private IEnumerator CrossfadeTo(AudioClip newClip, float duration)
    {
        inactiveSource.clip = newClip;
        inactiveSource.volume = 0f;
        inactiveSource.Play();

        float t = 0f;
        float startActiveVolume = activeSource.volume;

        while (t < duration)
        {
            t += Time.deltaTime;
            float ratio = t / duration;
            activeSource.volume = Mathf.Lerp(startActiveVolume, 0f, ratio);
            inactiveSource.volume = Mathf.Lerp(0f, musicVolume, ratio);
            yield return null;
        }

        activeSource.volume = 0f;
        inactiveSource.volume = musicVolume;
        activeSource.Stop();

        var temp = activeSource;
        activeSource = inactiveSource;
        inactiveSource = temp;
    }

    private IEnumerator FadeOut(float duration)
    {
        float t = 0f;
        float startVolume = activeSource.volume;

        while (t < duration)
        {
            t += Time.deltaTime;
            activeSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        activeSource.volume = 0f;
        activeSource.Stop();
        currentClip = null;
    }
}
