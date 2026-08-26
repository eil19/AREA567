using System.Collections.Generic;
using System.Collections.Specialized;

public static class DoorRunData
{
    private static readonly HashSet<string> unlockedDoors = new HashSet<string>();

    public static void UnlockDoor(string doorID)
    {
        if (string.IsNullOrEmpty(doorID)) return;
        unlockedDoors.Add(doorID);  
    }

    public static bool IsDoorUnlocked(string doorID)
    {
        if (string.IsNullOrEmpty(doorID)) return false;
        return unlockedDoors.Contains(doorID);
    }

    public static void ResetRun()
    {
        unlockedDoors.Clear();
    }
}