using UnityEngine;
using UnityEngine.Events;

public class NPC : MonoBehaviour, 
    IInteractable
{
    [Header("Dialogue")]
    [SerializeField] private NPCDialogue dialogueData;
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Dialogue Events")]
    public UnityEvent OnDialogueStarted;
    public UnityEvent OnDialogueEnded;

    private bool isMyDialogueActive;
    private UnityEvent dialogueFinishedEvent;

    private void Awake()
    {
        dialogueFinishedEvent = new UnityEvent();

        dialogueFinishedEvent.AddListener(HandleDialogueEnded);
    }

    public void Interact(GameObject interactor)
    {
        if (dialogueData == null || dialogueManager == null)
        {
            return;
        }

        // Dialogue already belongs to this NPC,
        // so pressing interact progresses it.
        if (isMyDialogueActive)
        {
            dialogueManager.NextLine();
        }
        else
        {
            StartDialogue();
        }
    }

    private void StartDialogue()
    {
        isMyDialogueActive = true;

        // Notify other components that dialogue started
        OnDialogueStarted?.Invoke();

        dialogueManager.StartDialogue(dialogueData, OnDialogueEnded);
    }

    public void HandleDialogueEnded()
    {
        isMyDialogueActive = false;
        OnDialogueEnded?.Invoke();
    }
}