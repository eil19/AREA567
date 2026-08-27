using System.Collections.Generic;

public static class AlienRunData
{
    private static readonly HashSet<string>
        identifiedAliens =
            new HashSet<string>();

    private static readonly HashSet<string>
        tamedAliens =
            new HashSet<string>();

    public static void MarkIdentified(
        string alienID)
    {
        if (!string.IsNullOrEmpty(alienID))
        {
            identifiedAliens.Add(alienID);
        }
    }

    public static void MarkTamed(
        string alienID)
    {
        if (!string.IsNullOrEmpty(alienID))
        {
            tamedAliens.Add(alienID);
        }
    }

    public static bool IsIdentified(
        string alienID)
    {
        return
            !string.IsNullOrEmpty(alienID) &&
            identifiedAliens.Contains(alienID);
    }

    public static bool IsTamed(
        string alienID)
    {
        return
            !string.IsNullOrEmpty(alienID) &&
            tamedAliens.Contains(alienID);
    }

    public static void ResetRun()
    {
        identifiedAliens.Clear();
        tamedAliens.Clear();
    }
}