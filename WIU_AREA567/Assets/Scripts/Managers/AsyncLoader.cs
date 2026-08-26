using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

public class AsyncLoader : MonoBehaviour
{
    [Header("Slider")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Slider loadingSlider;

    [Header("Settings")]
    [SerializeField] private float targetSpeed = 1.0f;

    public UnityEvent OnSceneLoaded;

    public void LoadScene(string sceneName)
    {
        OnSceneLoaded?.Invoke();
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingPanel.SetActive(true);
        loadingSlider.value = 0.0f;

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);
        // prevent scene from switching once it hits 0.9
        loadOperation.allowSceneActivation = false;

        float visualProgress = 0.0f;

        while (!loadOperation.isDone)
        {
            float actualProgress = Mathf.Clamp01(loadOperation.progress / 0.9f);
            visualProgress = Mathf.MoveTowards(visualProgress, actualProgress, targetSpeed * Time.deltaTime);
            loadingSlider.value = visualProgress;

            if (Mathf.Approximately(visualProgress, 1.0f) && !loadOperation.allowSceneActivation)
            {
                yield return new WaitForSeconds(0.5f);
                loadOperation.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}