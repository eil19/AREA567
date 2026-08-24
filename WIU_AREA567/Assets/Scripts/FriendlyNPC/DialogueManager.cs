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

    private NPCDialogue currentDialogue;
    private int dialogueIndex;

    private bool isTyping;
    private bool isDialogueActive;

    // Stores the event belonging to the NPC currently speaking
    private UnityEvent currentDialogueEndedEvent;

    public bool IsDialogueActive
    {
        get { return isDialogueActive; }
    }

    private void Start()
    {
        ShowDialogueUI(false);
    }

    public void StartDialogue(NPCDialogue dialogueData, UnityEvent dialogueEndedEvent)
    {
        if (dialogueData == null) return;

        currentDialogue = dialogueData;
        currentDialogueEndedEvent = dialogueEndedEvent;

        dialogueIndex = 0;
        isDialogueActive = true;

        SetNPCInfo(currentDialogue.npcName, currentDialogue.npcPortrait);
        ShowDialogueUI(true);
        StartCoroutine(TypeLine());
    }

    public void NextLine()
    {
        if (!isDialogueActive || currentDialogue == null) return;

        // If currently typing, finish the current sentence immediately
        if (isTyping)
        {
            StopAllCoroutines();

            SetDialogueText(currentDialogue.dialogueLines[dialogueIndex]);

            isTyping = false;
            return;
        }

        dialogueIndex++;

        // More dialogue lines remain
        if (dialogueIndex < currentDialogue.dialogueLines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    private IEnumerator TypeLine()
    {
        isTyping = true;

        SetDialogueText("");

        foreach (char letter in currentDialogue.dialogueLines[dialogueIndex])
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(currentDialogue.typingSpeed);
        }

        isTyping = false;

        // Automatically progress if this line is marked as auto-progress
        if (currentDialogue.autoProgressLines != null &&
            dialogueIndex < currentDialogue.autoProgressLines.Length &&
            currentDialogue.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(currentDialogue.autoProgressDelay);
            NextLine();
        }
    }

    public void EndDialogue()
    {
        StopAllCoroutines();

        isTyping = false;
        isDialogueActive = false;

        SetDialogueText("");
        ShowDialogueUI(false);

        currentDialogue = null;
        currentDialogueEndedEvent?.Invoke();
        currentDialogueEndedEvent = null;
    }

    public void ShowDialogueUI(bool show)
    {
        dialoguePanel.SetActive(show);
    }

    public void SetNPCInfo(string npcName, Sprite portrait)
    {
        nameText.text = npcName;
        portraitImage.sprite = portrait;
    }

    public void SetDialogueText(string text)
    {
        dialogueText.text = text;
    }
}