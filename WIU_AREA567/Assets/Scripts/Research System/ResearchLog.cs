using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Mirrors Inventory's persistent singleton pattern, but with no slot cap -
// discovered research is knowledge, not a carried resource, so the 5-item
// limit reasoning doesn't apply here. Duplicate discoveries are ignored.
public class ResearchLog : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent OnResearchLogChanged;
    public UnityEvent<ResearchData> OnResearchDiscovered;

    private List<ResearchData> discovered = new List<ResearchData>();
    public IReadOnlyList<ResearchData> Discovered => discovered;

    private static ResearchLog existingInstance;
    public static ResearchLog Instance => existingInstance;

    private void Awake()
    {
        if (existingInstance != null && existingInstance != this)
        {
            Destroy(gameObject);
            return;
        }

        existingInstance = this;
        //DontDestroyOnLoad(gameObject);
    }

    public bool AddResearch(ResearchData data)
    {
        if (data == null) return false;
        if (discovered.Contains(data)) return false; // already known

        discovered.Add(data);
        OnResearchDiscovered?.Invoke(data);
        OnResearchLogChanged?.Invoke();
        return true;
    }

    public bool HasDiscovered(ResearchData data)
    {
        return data != null && discovered.Contains(data);
    }

    // Sze Yee's CraftingSystem should call this as a gate before allowing
    // a craft, and her Crafting UI should only list recipes this returns
    // true for - unlocked recipes are the only ones that should appear at
    // all, not shown-but-greyed-out.
    public bool IsRecipeUnlocked(CraftingRecipe recipe)
    {
        if (recipe == null) return false;
        foreach (ResearchData data in discovered)
        {
            if (data.category == ResearchCategory.Recipe && data.unlockedRecipe == recipe)
                return true;
        }
        return false;
    }

    // Eileen's cryo tube/experimentation script should call this to know
    // what notes to display for whichever alien is currently being tested -
    // this never blocks the test itself, it's purely informational.
    public List<ResearchData> GetNotesForAlienType(AlienCategory category)
    {
        List<ResearchData> result = new List<ResearchData>();
        foreach (ResearchData data in discovered)
        {
            if (data.category == ResearchCategory.AlienType && data.relatedAlienCategory == category)
                result.Add(data);
        }
        return result;
    }

    public void DisplayDiscovered()
    {
        if (discovered.Count == 0)
        {
            Debug.Log("No research discovered yet.");
            return;
        }

        foreach (ResearchData data in discovered)
        {
            Debug.Log("Discovered: " + data.researchName);
        }
    }

    public void ClearResearch()
    {
        discovered.Clear();
        OnResearchLogChanged?.Invoke();
    }
}