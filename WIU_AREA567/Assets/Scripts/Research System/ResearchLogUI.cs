using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResearchLogUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_Text notesText;

    [Header("Research")]
    [SerializeField] private ResearchLog researchLog;

    private int currentNoteIndex = 0;
    private InputAction openLogAction;

    private void Start()
    {
        if (researchLog == null)
        {
            researchLog = FindFirstObjectByType<ResearchLog>();
        }

        if (InputSystem.actions != null)
        {
            openLogAction = InputSystem.actions.FindAction(
                "OpenResearchLog",
                false
            );
        }

        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }

        if (researchLog != null)
        {
            researchLog.OnResearchLogChanged.AddListener(RefreshCurrentNote);
        }
    }

    private void Update()
    {
        if (openLogAction != null &&
            openLogAction.WasPressedThisFrame())
        {
            ToggleLog();
        }

        if (panelRoot == null || !panelRoot.activeSelf)
        {
            return;
        }

        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.leftArrowKey.wasPressedThisFrame ||
            Keyboard.current.aKey.wasPressedThisFrame)
        {
            ShowPreviousNote();
        }

        if (Keyboard.current.rightArrowKey.wasPressedThisFrame ||
            Keyboard.current.dKey.wasPressedThisFrame)
        {
            ShowNextNote();
        }
    }

    private void ToggleLog()
    {
        if (panelRoot == null)
        {
            return;
        }

        bool shouldOpen = !panelRoot.activeSelf;

        if (shouldOpen)
        {
            currentNoteIndex = 0;
            RefreshCurrentNote();
        }

        panelRoot.SetActive(shouldOpen);
    }

    private void ShowPreviousNote()
    {
        if (!HasNotes())
        {
            return;
        }

        currentNoteIndex--;

        if (currentNoteIndex < 0)
        {
            currentNoteIndex = researchLog.Discovered.Count - 1;
        }

        RefreshCurrentNote();
    }

    private void ShowNextNote()
    {
        if (!HasNotes())
        {
            return;
        }

        currentNoteIndex++;

        if (currentNoteIndex >= researchLog.Discovered.Count)
        {
            currentNoteIndex = 0;
        }

        RefreshCurrentNote();
    }

    private void RefreshCurrentNote()
    {
        if (notesText == null)
        {
            return;
        }

        if (!HasNotes())
        {
            notesText.text = "No research notes collected.";
            return;
        }

        if (currentNoteIndex >= researchLog.Discovered.Count)
        {
            currentNoteIndex = researchLog.Discovered.Count - 1;
        }

        ResearchData note =
            researchLog.Discovered[currentNoteIndex];

        if (note == null)
        {
            notesText.text = "This research note is missing.";
            return;
        }

        notesText.text =
            "Research Note " +
            (currentNoteIndex + 1) +
            " / " +
            researchLog.Discovered.Count +
            "\n\n" +
            note.researchName +
            "\n\n" +
            note.description;
    }

    private bool HasNotes()
    {
        return researchLog != null &&
               researchLog.Discovered != null &&
               researchLog.Discovered.Count > 0;
    }

    private void OnDestroy()
    {
        if (researchLog != null)
        {
            researchLog.OnResearchLogChanged.RemoveListener(
                RefreshCurrentNote
            );
        }
    }
}