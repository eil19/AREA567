using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class EndingController :
    MonoBehaviour
{
    [SerializeField]
    private GameObject endingPanel;

    [SerializeField]
    private TMP_Text endingText;

    [SerializeField]
    private GameObject exitButton;

    [SerializeField]
    private CutsceneDialogue endingDialogue;

    [SerializeField]
    private float typingSpeed = 0.04f;

    [SerializeField]
    private float delayBetweenLines = 1.5f;

    [SerializeField]
    private float bossDeathDelay = 2f;

    [Header("Player UI")] [SerializeField] private GameObject playerUI;

    public UnityEvent OnEndingStarted;
    public UnityEvent OnEndingFinished;

    private bool hasStarted;

    private void Start()
    {
        if (endingPanel != null)
            endingPanel.SetActive(false);

        if (exitButton != null)
            exitButton.SetActive(false);
    }

    public void BeginEnding()
    {
        if (playerUI != null)
        {
            playerUI.SetActive(false);
        }

        if (hasStarted)
            return;

        hasStarted = true;

        StartCoroutine(EndingSequence());
    }

    private IEnumerator EndingSequence()
    {
        OnEndingStarted?.Invoke();

        yield return
            new WaitForSecondsRealtime(
                bossDeathDelay
            );

        endingPanel?.SetActive(true);
        exitButton?.SetActive(false);

        if (endingDialogue != null &&
            endingDialogue.lines != null)
        {
            foreach (string line
                     in endingDialogue.lines)
            {
                yield return TypeLine(line);

                yield return
                    new WaitForSecondsRealtime(
                        delayBetweenLines
                    );
            }
        }

        yield return TypeLine(
            "Congratulations! You defeated the Alien Queen!"
        );

        exitButton?.SetActive(true);

        OnEndingFinished?.Invoke();
    }

    private IEnumerator TypeLine(string line)
    {
        if (endingText == null)
            yield break;

        endingText.text = "";

        foreach (char character in line)
        {
            endingText.text += character;

            yield return
                new WaitForSecondsRealtime(
                    typingSpeed
                );
        }
    }

    public void ExitToMainMenu()
    {
        SceneFlowManager sceneFlowManager =
            FindFirstObjectByType<
                SceneFlowManager>();

        if (sceneFlowManager != null)
        {
            sceneFlowManager.ReturnToMainMenu();
        }
        else
        {
            Debug.LogError(
                "EndingController: " +
                "SceneFlowManager not found."
            );
        }
    }
}