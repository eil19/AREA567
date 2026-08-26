using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OpeningCutsceneController : MonoBehaviour
{
    [Header("World")]
    [SerializeField] private Animator alienQueenAnimator;

    [Header("Screen Effects")]
    [SerializeField] private Image redOverlay;
    [SerializeField] private Image blackOverlay;

    [Header("Dialogue")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button readyButton;

    [Header("Dialogue Lines")]
    [SerializeField] private CutsceneDialogueLine[] dialogueLines;

    [Header("Timing")]
    [SerializeField] private float attackDelay = 1.5f;
    [SerializeField] private float redFadeDuration = 1f;
    [SerializeField] private float blackFadeDuration = 1.5f;

    [Header("Events")]
    public UnityEvent OnFatalAttack;
    public UnityEvent OnFlashbackStarted;
    public UnityEvent OnCutsceneFinished;

    private int dialogueIndex;
    private bool dialogueStarted;

    private void Start()
    {
        SetupInitialState();
        StartCoroutine(PlayOpeningSequence());
    }

    private void SetupInitialState()
    {
        SetImageAlpha(redOverlay, 0f);
        SetImageAlpha(blackOverlay, 0f);

        dialoguePanel.SetActive(false);
        continueButton.gameObject.SetActive(false);
        readyButton.gameObject.SetActive(false);

        dialogueIndex = 0;
        dialogueStarted = false;
    }

    private IEnumerator PlayOpeningSequence()
    {
        yield return new WaitForSeconds(attackDelay);

        OnFatalAttack?.Invoke();

        if (alienQueenAnimator != null)
        {
            alienQueenAnimator.SetTrigger("FatalAttack");
        }

        yield return FadeImage(
            redOverlay,
            0f,
            1f,
            redFadeDuration
        );

        yield return FadeImage(
            blackOverlay,
            0f,
            1f,
            blackFadeDuration
        );

        StartDialogue();
    }

    private void StartDialogue()
    {
        if (dialogueLines == null ||
            dialogueLines.Length == 0)
        {
            ShowReadyButton();
            return;
        }

        dialogueStarted = true;
        dialogueIndex = 0;

        dialoguePanel.SetActive(true);

        OnFlashbackStarted?.Invoke();

        ShowCurrentDialogue();
    }

    private void ShowCurrentDialogue()
    {
        if (!dialogueStarted)
            return;

        if (dialogueIndex >= dialogueLines.Length)
        {
            ShowReadyButton();
            return;
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

    public void ContinueDialogue()
    {
        if (!dialogueStarted)
            return;

        dialogueIndex++;

        ShowCurrentDialogue();
    }

    private void ShowReadyButton()
    {
        continueButton.gameObject.SetActive(false);
        readyButton.gameObject.SetActive(true);
    }

    public void FinishCutscene()
    {
        OnCutsceneFinished?.Invoke();
    }

    private IEnumerator FadeImage(
        Image image,
        float startAlpha,
        float endAlpha,
        float duration)
    {
        if (image == null)
            yield break;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(elapsed / duration);

            SetImageAlpha(
                image,
                Mathf.Lerp(
                    startAlpha,
                    endAlpha,
                    t
                )
            );

            yield return null;
        }

        SetImageAlpha(image, endAlpha);
    }

    private void SetImageAlpha(
        Image image,
        float alpha)
    {
        if (image == null)
            return;

        Color colour = image.color;
        colour.a = alpha;
        image.color = colour;
    }
}