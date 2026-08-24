using UnityEngine;

public enum ResearchCategory { Recipe, AlienType, Lore }

[CreateAssetMenu(fileName = "ResearchData", menuName = "Research/ResearchData")]
public class ResearchData : ScriptableObject
{
    [Header("Display")]
    public string researchName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("What this unlocks (leave category as Lore for flavor-only notes)")]
    public ResearchCategory category;
    [Tooltip("Only used if Category = Recipe")]
    public CraftingRecipe unlockedRecipe;
    [Tooltip("Only used if Category = AlienType - references Eileen's AlienCategory enum")]
    public AlienCategory relatedAlienCategory;
}