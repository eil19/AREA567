using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class EndingController :
    MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private GameObject endingPanel;

    [SerializeField]
    private TMP_Text endingText;

    [SerializeField]
    private GameObject mainMenuButton;

    [Header("Dialogue")]
    [SerializeField]
    private CutsceneDialogue endingDialogue;

    [Header("Timing")]
    [SerializeField]
    private float typingSpeed = 0.04f;

    [SerializeField]
    private float delayBetweenLines = 1.5f;

    [SerializeField]
    private float bossDeathDelay = 2f;

    [Header("Events")]
    public UnityEvent OnEndingStarted;
    public UnityEvent OnEndingFinished;

    private bool hasStarted;

    private void Start()
    {
        endingPanel.SetActive(false);
    }

    public void BeginEnding()
    {
        if (hasStarted)
            return;

        hasStarted = true;

        StartCoroutine(
            EndingSequence()
        );
    }

    private IEnumerator EndingSequence()
    {
        OnEndingStarted?.Invoke();

        // Allow Queen death animation to play.
        yield return new WaitForSeconds(
            bossDeathDelay
        );

        endingPanel.SetActive(true);
        mainMenuButton.SetActive(false);

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

        mainMenuButton.SetActive(true);

        OnEndingFinished?.Invoke();
    }

    private IEnumerator TypeLine(
        string line)
    {
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
}