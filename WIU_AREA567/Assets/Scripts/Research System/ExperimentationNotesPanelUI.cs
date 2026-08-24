using TMPro;
using UnityEngine;

// REFERENCE STUB for Eileen's Experimentation UI - shows discovered notes
// for whichever alien is currently being tested. Call ShowNotesFor() when
// experimentation starts, Hide() when it ends. Never blocks the test -
// this is purely a reference panel, shows nothing if no notes exist yet.
public class ExperimentationNotesPanelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text notesText;

    void Awake()
    {
        Hide();
    }

    public void ShowNotesFor(AlienCategory category)
    {
        var notes = ResearchLog.Instance.GetNotesForAlienType(category);

        if (notes.Count == 0)
        {
            notesText.text = "No research yet - experiment to find out!";
        }
        else
        {
            notesText.text = "";
            foreach (ResearchData note in notes)
            {
                notesText.text += note.researchName + ": " + note.description + "\n";
            }
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}