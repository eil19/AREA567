using Unity.Cinemachine;
using UnityEngine;

public class TESTCameraSwitch : MonoBehaviour
{
    public static TESTCameraSwitch Instance { get; private set; }

    [Header("Default gameplay camera")]
    [SerializeField] private CinemachineCamera topDownCam;

    [Header("Priorities")]
    [SerializeField] private int activePriority = 20;
    [SerializeField] private int inactivePriority = 0;
    [SerializeField] private int topDownPriority = 10;

    private CinemachineCamera currentCloseUpCam;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        // Establish starting state: top-down active, no close-up cam active yet.
        if (topDownCam != null) topDownCam.Priority = topDownPriority;
    }

    public void SwitchToCloseUp(CinemachineCamera closeUpCam)
    {
        if (closeUpCam == null)
        {
            Debug.LogWarning("[CameraSequenceController] No close-up camera assigned.");
            return;
        }

        currentCloseUpCam = closeUpCam;
        currentCloseUpCam.Priority = activePriority;

        if (topDownCam != null) topDownCam.Priority = inactivePriority;
    }

    public void SwitchToTopDown()
    {
        if (currentCloseUpCam != null)
        {
            currentCloseUpCam.Priority = inactivePriority;
            currentCloseUpCam = null;
        }

        if (topDownCam != null) topDownCam.Priority = topDownPriority;
    }
}
