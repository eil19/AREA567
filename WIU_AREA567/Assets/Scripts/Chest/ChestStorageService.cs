using System.Collections.Generic;
using UnityEngine;

public class ChestStorageService : MonoBehaviour
{
    private Dictionary<string, List<ItemInstance>>
        chestContents =
        new Dictionary<string, List<ItemInstance>>();

    public List<ItemInstance> GetOrCreateChest(
        string chestId,
        int slotCount)
    {
        if (string.IsNullOrWhiteSpace(chestId))
        {
            Debug.LogError(
                "Chest must have a valid Chest ID."
            );

            return null;
        }

        if (!chestContents.TryGetValue(
            chestId,
            out List<ItemInstance> slots))
        {
            slots =
                new List<ItemInstance>();

            for (int i = 0;
                 i < slotCount;
                 i++)
            {
                slots.Add(null);
            }

            chestContents.Add(
                chestId,
                slots
            );
        }

        return slots;
    }
}