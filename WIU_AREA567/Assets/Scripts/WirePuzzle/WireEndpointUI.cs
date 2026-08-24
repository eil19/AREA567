using UnityEngine;

public class WireEndpointUI : MonoBehaviour
{
    [SerializeField] private WireType wireType;

    [Header("Endpoint Settings")]
    [SerializeField] private bool isDestination;

    private bool isOccupied;

    public WireType WireType => wireType;
    public bool IsDestination => isDestination;
    public bool IsOccupied => isOccupied;

    public bool CanConnect(WireType type)
    {
        return isDestination &&
               !isOccupied &&
               wireType == type;
    }

    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
    }
}