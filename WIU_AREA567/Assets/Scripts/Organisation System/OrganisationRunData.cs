using System.Collections.Generic;
using UnityEngine;

public static class OrganisationRunData
{
    private static readonly Dictionary<string, Vector3>
        organisedItems = new Dictionary<string, Vector3>();

    public static void MarkOrganised(
        string itemID,
        Vector3 position)
    {
        if (string.IsNullOrEmpty(itemID))
            return;

        organisedItems[itemID] = position;
    }

    public static void RemoveOrganised(
        string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
            return;

        organisedItems.Remove(itemID);
    }

    public static bool TryGetPosition(
        string itemID,
        out Vector3 position)
    {
        return organisedItems.TryGetValue(
            itemID,
            out position
        );
    }

    public static void ResetRun()
    {
        organisedItems.Clear();
    }
}