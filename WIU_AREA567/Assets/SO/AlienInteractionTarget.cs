using UnityEngine;
public static class AlienInteractionTarget
{
    public static AlienInstance Current { get; private set; }

    public static void SetCurrent(AlienInstance alien)
    {
        Current = alien;
    }

    public static void ClearIfCurrent(AlienInstance alien)
    {
        if (Current == alien)
        {
            Current = null;
        }
    }
}