using UnityEngine;

[CreateAssetMenu(fileName = "AlienType", menuName = "Scriptable Objects/AlienType")]
public class AlienType : ScriptableObject
{
    [Header("Identity")]
    public string alienName;
    //public AlienCategory category; // enum: Healer, Damage, Flying, Tanker

    [Header("Stats")]
    public float health;
    public float damage;
    public float moveSpeed;

    [Header("Experimentation")]
    public string reactionDescription; // e.g. "Glows green, reaches toward wounded specimen"
    public AnimationClip reactionAnimation;
    public GameObject essencePrefab;
    public bool isHealer;
    public bool isFlying;

    [Header("Taming")]
    [Range(0f, 1f)] public float tameDifficulty; // affects success chance
    public GameObject tamedPrefab; // how it looks/acts once allied
}
