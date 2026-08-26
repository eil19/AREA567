using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class IntroCutsceneController : MonoBehaviour
{
    [Header("World")]
    [SerializeField] private Animator alienQueenAnimator;

    [Header("Screen Effects")]
    [SerializeField] private Image redOverlay;
    [SerializeField] private Image blackOverlay;

    [Header("Dialogue")]
    [SerializeField]
    private CutsceneDialogue dialogue;

    [SerializeField]
    private TMP_Text dialogueText;

    [Header("Buttons")]
    [SerializeField]
    private GameObject choiceButtons;

    [SerializeField]
    private Button readyButton;

    [SerializeField]
    private Button notReadyButton;

    [Header("Typewriter")]
    [SerializeField]
    private float typingSpeed = 0.04f;

    [SerializeField]
    private float delayBetweenLines = 1.5f;

    [SerializeField]
    private float introDelay = 1f;

    [Header("Events")]
    public UnityEvent OnIntroStarted;
    public UnityEvent OnDialogueFinished;

    private Coroutine introCoroutine;

    public void BeginIntro()
    {
        if (introCoroutine != null)
        {
            StopCoroutine(introCoroutine);
        }

        dialogueText.text = "";

        if (choiceButtons != null)
        {
            choiceButtons.SetActive(false);
        }

        introCoroutine =
            StartCoroutine(PlayIntro());
    }

    private void Start()
    {
        dialogueText.text = "";

        if (choiceButtons != null)
        {
            choiceButtons.SetActive(false);
        }

        CutsceneDialogueLine line =
            dialogueLines[dialogueIndex];

        speakerNameText.text = line.speaker;
        dialogueText.text = line.dialogue;

        bool isLastLine =
            dialogueIndex == dialogueLines.Length - 1;

        continueButton.gameObject.SetActive(!isLastLine);
        readyButton.gameObject.SetActive(isLastLine);
    }

    private IEnumerator PlayIntro()
    {
        OnIntroStarted?.Invoke();

        // Allows the death / impact SFX
        // to play against a black screen.
        yield return new WaitForSecondsRealtime(
            introDelay
        );

        if (dialogue == null ||
            dialogue.lines == null)
        {
            FinishDialogue();
            yield break;
        }

        foreach (string line in dialogue.lines)
        {
            yield return TypeLine(line);

            yield return new WaitForSecondsRealtime(
                delayBetweenLines
            );
        }

        FinishDialogue();
    }

    private IEnumerator TypeLine(
        string line)
    {
        dialogueText.text = "";

        foreach (char character in line)
        {
            dialogueText.text += character;

            yield return new WaitForSecondsRealtime(
                typingSpeed
            );

            yield return null;
        }

        SetImageAlpha(image, endAlpha);
    }

    private void FinishDialogue()
    {
        introCoroutine = null;

        OnDialogueFinished?.Invoke();

        if (choiceButtons != null)
        {
            choiceButtons.SetActive(true);
        }
    }
}