using UnityEngine;

[CreateAssetMenu(
    fileName = "CutsceneDialogue",
    menuName = "Dialogue/Cutscene Dialogue")]
public class CutsceneDialogue : ScriptableObject
{
    [TextArea(2, 5)]
    public string[] lines;
}