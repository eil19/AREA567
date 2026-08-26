using UnityEngine;

[CreateAssetMenu(fileName = "AudioSettingsData", 
    menuName = "Scriptable Objects/AudioSettingsData")]
public class AudioSettingsData : ScriptableObject
{
    [Range(0.0f, 1.0f)]
    public float masterVolume = 1.0f;

    [Range(0.0f, 1.0f)]
    public float musicVolume = 1.0f;

    [Range(0.0f, 1.0f)]
    public float sfxVolume = 1.0f;
}