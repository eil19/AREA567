using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image portraitImage;
    [SerializeField] private GameObject closeButton;

    [Header("Dialogue Timing")]
    [SerializeField] private float typingSpeed = 0.05f;

    [Tooltip(
        "How long the completed line remains " +
        "before the next line begins automatically."
    )]
    [SerializeField] private float delayBetweenLines = 1.5f;

    [Header("Events")]
    public UnityEvent OnDialogueStarted;
    public UnityEvent OnDialogueFinished;

    private NPCDialogue currentDialogue;
    private UnityEvent dialogueEndedEvent;

    private bool isTyping;
    private bool isDialogueActive;
    private bool canClose;

    private Coroutine dialogueCoroutine;

    public bool IsDialogueActive =>
        isDialogueActive;

    public bool IsTyping =>
        isTyping;

    private void Start()
    {
        ShowDialogueUI(false);

        if (closeButton != null)
        {
            closeButton.SetActive(false);
        }
    }

    public void StartDialogue(
        NPCDialogue dialogueData,
        UnityEvent onDialogueEnded)
    {
        if (dialogueData == null)
            return;

        if (isDialogueActive)
            return;

        if (dialogueData.dialogueLines == null ||
            dialogueData.dialogueLines.Length == 0)
        {
            return;
        }

        currentDialogue = dialogueData;
        dialogueEndedEvent = onDialogueEnded;

        isDialogueActive = true;
        isTyping = false;
        canClose = false;

        if (closeButton != null)
        {
            closeButton.SetActive(false);
        }

        SetNPCInfo(
            currentDialogue.npcName,
            currentDialogue.npcPortrait
        );

        ShowDialogueUI(true);

        OnDialogueStarted?.Invoke();

        dialogueCoroutine =
            StartCoroutine(
                PlayDialogue()
            );
    }

    private IEnumerator PlayDialogue()
    {
        for (int i = 0;
             i < currentDialogue.dialogueLines.Length;
             i++)
        {
            yield return TypeLine(
                currentDialogue.dialogueLines[i]
            );

            bool isLastLine =
                i ==
                currentDialogue.dialogueLines.Length - 1;

            if (!isLastLine)
            {
                yield return
                    new WaitForSeconds(
                        delayBetweenLines
                    );
            }
        }

        // All dialogue has now completely finished.
        canClose = true;
        dialogueCoroutine = null;

        if (closeButton != null)
        {
            closeButton.SetActive(true);
        }
    }

    private IEnumerator TypeLine(
        string line)
    {
        isTyping = true;

        if (dialogueText != null)
        {
            dialogueText.text = "";
        }

        foreach (char letter in line)
        {
            if (dialogueText != null)
            {
                dialogueText.text += letter;
            }

            yield return new WaitForSeconds(
                typingSpeed
            );
        }

        isTyping = false;
    }

    public void CloseDialogue()
    {
        if (!isDialogueActive)
            return;

        // Dialogue has not finished yet.
        if (!canClose)
            return;

        EndDialogue();
    }

    private void EndDialogue()
    {
        if (dialogueCoroutine != null)
        {
            StopCoroutine(
                dialogueCoroutine
            );

            dialogueCoroutine = null;
        }

        isTyping = false;
        isDialogueActive = false;
        canClose = false;

        ShowDialogueUI(false);

        if (closeButton != null)
        {
            closeButton.SetActive(false);
        }

        UnityEvent finishedEvent =
            dialogueEndedEvent;

        dialogueEndedEvent = null;
        currentDialogue = null;

        // Tell this specific NPC its dialogue ended.
        finishedEvent?.Invoke();

        // Tell global systems dialogue ended.
        OnDialogueFinished?.Invoke();
    }

    public void ShowDialogueUI(bool show)
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(show);
        }
    }

    public void SetNPCInfo(
        string npcName,
        Sprite portrait)
    {
        if (nameText != null)
        {
            nameText.text = npcName;
        }

        if (portraitImage != null)
        {
            portraitImage.sprite = portrait;
            portraitImage.enabled =
                portrait != null;
        }
    }
}