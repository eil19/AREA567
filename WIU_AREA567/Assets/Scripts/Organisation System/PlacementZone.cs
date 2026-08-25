using UnityEngine;

public class PlacementZone : MonoBehaviour
{
    [SerializeField] private string zoneId;
    public string ZoneId => zoneId;

    void Reset()
    {
        // Zones are detection areas, not physical obstacles.
        GetComponent<Collider2D>().isTrigger = true;
    }
}
