using System.Collections;
using TMPro;
using UnityEngine;

// Simple auto-dismissing notification popup - shows a message at the top of
// the screen briefly, then hides itself. Generic - reusable for any "X
// happened!" moment later, not just research pickups.
public class NotificationPopupUI : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float displayDuration = 2.5f;

    private Coroutine hideRoutine;

    public static NotificationPopupUI Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Show(string message)
    {
        messageText.text = message;
        gameObject.SetActive(true);

        if (hideRoutine != null) StopCoroutine(hideRoutine);
        hideRoutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        gameObject.SetActive(false);
        hideRoutine = null;
    }
}