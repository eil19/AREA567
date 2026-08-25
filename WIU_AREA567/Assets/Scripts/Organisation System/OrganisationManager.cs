using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OrganisationManager : MonoBehaviour
{
    public static OrganisationManager Instance { get; private set; }

    [Range(0f, 1f)]
    [SerializeField] private float travellingMachineThreshold = 0.3f;
    public UnityEvent<float> OnProgressChanged;

    private readonly List<OrganisableItem> registeredItems = new List<OrganisableItem>();

    public float PercentOrganised
    {
        get
        {
            if (registeredItems.Count == 0) return 0f;

            int organisedCount = 0;
            for (int i = 0; i < registeredItems.Count; i++)
            {
                if (registeredItems[i] != null && registeredItems[i].IsOrganised)
                {
                    organisedCount++;
                }
            }

            return (float)organisedCount / registeredItems.Count;
        }
    }

    public bool HasMetThreshold => PercentOrganised >= travellingMachineThreshold;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Register(OrganisableItem item)
    {
        if (item != null && !registeredItems.Contains(item))
        {
            registeredItems.Add(item);
        }
        NotifyChanged();
    }

    public void Unregister(OrganisableItem item)
    {
        registeredItems.Remove(item);
        NotifyChanged();
    }

    public void NotifyChanged()
    {
        OnProgressChanged?.Invoke(PercentOrganised);
    }
}
