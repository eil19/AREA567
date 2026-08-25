using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class WireConnectionUI : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [Header("References")]
    [SerializeField] private RectTransform wireImage;
    [SerializeField] private WireEndpointUI startEndpoint;
    [SerializeField] private WirePuzzleController puzzleController;

    [Header("Events")]
    public UnityEvent OnCorrectConnection;
    public UnityEvent OnWrongConnection;

    private bool isConnected;
    private WireEndpointUI connectedEndpoint;

    public bool IsConnected => isConnected;

    private void Start()
    {
        ResetConnection();
    }

    public void OnBeginDrag(
        PointerEventData eventData)
    {
        if (isConnected ||
            wireImage == null ||
            startEndpoint == null)
        {
            return;
        }

        wireImage.gameObject.SetActive(true);

        UpdateWire(
            startEndpoint.transform.position,
            eventData.position
        );
    }

    public void OnDrag(
        PointerEventData eventData)
    {
        if (isConnected ||
            wireImage == null)
        {
            return;
        }

        UpdateWire(
            startEndpoint.transform.position,
            eventData.position
        );
    }

    public void OnEndDrag(
        PointerEventData eventData)
    {
        if (isConnected)
            return;

        GameObject target =
            eventData.pointerCurrentRaycast.gameObject;

        if (target == null)
        {
            HideWire();
            return;
        }

        WireEndpointUI endEndpoint =
            target.GetComponent<WireEndpointUI>();

        if (endEndpoint == null)
        {
            HideWire();
            return;
        }

        if (!endEndpoint.CanConnect(
            startEndpoint.WireType))
        {
            Debug.Log(
                "Incorrect wire connection: " +
                startEndpoint.WireType
            );
            OnWrongConnection?.Invoke();
            HideWire();
            return;
        }

        ConnectTo(endEndpoint);
    }

    private void ConnectTo(
        WireEndpointUI endpoint)
    {
        isConnected = true;
        connectedEndpoint = endpoint;

        endpoint.SetOccupied(true);

        UpdateWire(
            startEndpoint.transform.position,
            endpoint.transform.position
        );

        Debug.Log(
            startEndpoint.WireType +
            " wire connected correctly."
        );
        OnCorrectConnection?.Invoke();
        puzzleController?.RegisterConnection();
    }

    public void ResetConnection()
    {
        if (connectedEndpoint != null)
        {
            connectedEndpoint.SetOccupied(false);
        }

        connectedEndpoint = null;
        isConnected = false;

        HideWire();
    }

    private void HideWire()
    {
        if (wireImage != null)
        {
            wireImage.gameObject.SetActive(false);
        }
    }

    private void UpdateWire(
        Vector2 startPosition,
        Vector2 endPosition)
    {
        Vector2 direction =
            endPosition - startPosition;

        float distance =
            direction.magnitude;

        wireImage.position =
            startPosition;

        wireImage.sizeDelta =
            new Vector2(
                distance,
                wireImage.sizeDelta.y
            );

        float angle =
            Mathf.Atan2(
                direction.y,
                direction.x
            ) * Mathf.Rad2Deg;

        wireImage.rotation =
            Quaternion.Euler(
                0f,
                0f,
                angle
            );
    }
}