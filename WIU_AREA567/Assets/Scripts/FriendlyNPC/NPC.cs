using UnityEngine;
using UnityEngine.Events;

public class NPC : MonoBehaviour,
    IInteractable
{
    [Header("Dialogue")]
    [SerializeField] private NPCDialogue dialogueData;

    [SerializeField] private DialogueManager dialogueManager;

    [Header("Events")]
    public UnityEvent OnDialogueStarted;
    public UnityEvent OnDialogueEnded;

    private bool isMyDialogueActive;

    private UnityEvent dialogueFinishedEvent;

    private void Awake()
    {
        dialogueFinishedEvent = new UnityEvent();
        dialogueFinishedEvent.AddListener(HandleDialogueEnded);
    }

    private void Start()
    {
        if (dialogueManager == null)
        {
            dialogueManager = FindFirstObjectByType<DialogueManager>();
        }
    }

    private void OnDestroy()
    {
        if (dialogueFinishedEvent != null)
        {
            dialogueFinishedEvent.RemoveListener(
                HandleDialogueEnded);
        }
    }

    public void Interact(GameObject interactor)
    {
        if (dialogueManager == null ||
            dialogueData == null)
        {
            return;
        }

        if (isMyDialogueActive)
        {
            dialogueManager.NextLine();
            return;
        }

        if (dialogueManager.IsDialogueActive)
        {
            return;
        }

        StartDialogue();
    }

    private void StartDialogue()
    {
        isMyDialogueActive = true;
        OnDialogueStarted?.Invoke();
        dialogueManager.StartDialogue(dialogueData, dialogueFinishedEvent);
    }

    private void HandleDialogueEnded()
    {
        isMyDialogueActive = false;
        OnDialogueEnded?.Invoke();
    }
}