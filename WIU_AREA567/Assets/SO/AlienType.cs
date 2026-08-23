using UnityEngine;

public enum AlienCategory
{
    Healer,
    Damage,
    Flying,
    Tanker
}

[CreateAssetMenu(fileName = "AlienType", menuName = "Scriptable Objects/AlienType")]
public class AlienType : ScriptableObject
{
    [Header("Identity")]
    public string alienName;
    public AlienCategory category;

    [Header("Stats")]
    public float health;
    public float damage;
    public float moveSpeed;

    [Header("Experimentation")]
    public string reactionDescription; // e.g. "Glows green, reaches toward wounded specimen"

    [Header("Taming")]
    [Range(0f, 1f)] public float tameDifficulty; 
    public GameObject tamedPrefab;

    [Header("Detection")]
    public float detectRadius = 4f;
    public LayerMask enemyLayer;

    [Header("Essence Drop")]
    public GameObject essencePrefab;
    public ItemData essenceItemData;
}
