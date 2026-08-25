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

    [SerializeField] private float typingSpeed = 0.05f;

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

        currentDialogue = dialogueData;
        dialogueIndex = 0;
        dialogueEndedEvent = onDialogueEnded;

        isDialogueActive = true;

        SetNPCInfo(currentDialogue.npcName, currentDialogue.npcPortrait);
        ShowDialogueUI(true);
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

        dialogueIndex++;

        if (dialogueIndex >= currentDialogue.dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (currentDialogue.dialogueLines == null ||
            dialogueIndex < 0 || dialogueIndex >= currentDialogue.dialogueLines.Length)
        {
            EndDialogue();
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
    }

    public void EndDialogue()
    {
        StopAllCoroutines();

        isTyping = false;
        isDialogueActive = false;

        ShowDialogueUI(false);

        UnityEvent finishedEvent = dialogueEndedEvent;
        dialogueEndedEvent = null;
        currentDialogue = null;
        dialogueIndex = 0;
    }

    public void ShowDialogueUI(bool show)
    {
        if (dialoguePanel == null) return;
        dialoguePanel.SetActive(show);
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
}