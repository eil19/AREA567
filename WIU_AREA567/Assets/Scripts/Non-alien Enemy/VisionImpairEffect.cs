using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VisionImpairEffect : MonoBehaviour
{
    public static VisionImpairEffect Instance;

    [SerializeField] private Image overlayImage;
    [SerializeField] private float maxAlpha = 0.95f;
    [SerializeField] private float fadeTime = 0.2f;

    private Coroutine activeRoutine;

    private void Awake()
    {
        Instance = this;

        if (overlayImage != null)
        {
            Color c = overlayImage.color;
            c.a = 0f;
            overlayImage.color = c;
        }
    }

    public void TriggerImpairment(float duration)
    {
        if (overlayImage == null) return;

        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(ImpairRoutine(duration));
    }

    private IEnumerator ImpairRoutine(float duration)
    {
        yield return Fade(0f, maxAlpha, fadeTime);
        yield return new WaitForSeconds(duration);
        yield return Fade(maxAlpha, 0f, fadeTime);
        activeRoutine = null;
    }

    private IEnumerator Fade(float from, float to, float time)
    {
        float elapsed = 0f;
        Color c = overlayImage.color;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, elapsed / time);
            overlayImage.color = c;
            yield return null;
        }

        c.a = to;
        overlayImage.color = c;
    }
}
