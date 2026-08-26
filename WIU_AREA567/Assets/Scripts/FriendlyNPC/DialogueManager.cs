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
    [SerializeField] private GameObject backButton;   

    [SerializeField] private float typingSpeed = 0.05f;

    [Header("Events")]
    public UnityEvent OnDialogueStarted;
    public UnityEvent OnDialogueFinished;

    private NPCDialogue currentDialogue;
    private int dialogueIndex;

    private bool isTyping;
    private bool isDialogueActive;

    public UnityEvent dialogueEndedEvent;

    public bool IsDialogueActive => isDialogueActive;
    public bool IsTyping => isTyping;

    private void Start()
    {
        ShowDialogueUI(false);
    }

    public void StartDialogue(NPCDialogue dialogueData, UnityEvent onDialogueEnded)
    {
        if (dialogueData == null) return;
        if (isDialogueActive) return;
        if (dialogueData.dialogueLines == null || dialogueData.dialogueLines.Length == 0) return;

        currentDialogue = dialogueData;
        dialogueIndex = 0;
        dialogueEndedEvent = onDialogueEnded;

        isDialogueActive = true;

        if (backButton != null)
        {
            backButton.SetActive(false);
        }

        SetNPCInfo(currentDialogue.npcName, currentDialogue.npcPortrait);
        ShowDialogueUI(true);
        OnDialogueStarted?.Invoke();
        ShowCurrentLine();
    }

    public void NextLine()
    {
        if (!isDialogueActive || currentDialogue == null) return;

        // do not allow advancing while text is still typing
        if (isTyping)
        {
            return;
        }

        // use close button
        if (dialogueIndex >= currentDialogue.dialogueLines.Length - 1)
        {
            return;
        }

        dialogueIndex++;
        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (currentDialogue.dialogueLines == null ||
            dialogueIndex < 0 || dialogueIndex >= currentDialogue.dialogueLines.Length)
        {
            return;
        }

        StopAllCoroutines();

        StartCoroutine(TypeLine(currentDialogue.dialogueLines[dialogueIndex]));
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;

        dialogueText.text = "";

        foreach (char letter in line)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        if (dialogueIndex >= currentDialogue.dialogueLines.Length -1)
        {
            if (backButton != null)
            {
                backButton.SetActive(true);
            }
        }
    }

    public void EndDialogue()
    {
        StopAllCoroutines();

        isTyping = false;
        isDialogueActive = false;

        ShowDialogueUI(false);
        if (backButton != null)
        {
            backButton.SetActive(false);
        }

        UnityEvent finishedEvent = dialogueEndedEvent;
        dialogueEndedEvent = null;
        currentDialogue = null;
        dialogueIndex = 0;

        finishedEvent?.Invoke();
        OnDialogueFinished?.Invoke();
    }

    public void ShowDialogueUI(bool show)
    {
        if (dialoguePanel == null) return;
        dialoguePanel.SetActive(show);
        if (!show && backButton != null)
        {
            backButton.SetActive(false);
        }
    }

    public void SetNPCInfo(string npcName, Sprite portrait)
    {
        if (nameText == null) return;
        nameText.text = npcName;
        if (portraitImage == null) return;
        portraitImage.sprite = portrait;
        portraitImage.enabled = portrait != null;
    }

    public void SetDialogueText(string text)
    {
        if (dialogueText == null) return;
        dialogueText.text = text;
    }

    public void CloseDialogue()
    {
        if (!isDialogueActive) return;
        if (isTyping) return;
        if (currentDialogue == null) return;

        int lastIndex = currentDialogue.dialogueLines.Length - 1;

        // cannot close before reaching final dialogue line
        if (dialogueIndex < lastIndex) return;

        EndDialogue();
    }
}