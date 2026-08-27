using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlienGuessUI : MonoBehaviour
{
    public static AlienGuessUI Instance { get; private set; }
    public bool Guessed { get; private set; }
    public GameObject panelRoot; 
    public Button healerButton;
    public Button damageButton;
    public Button flyingButton;
    public Button tankerButton;
    public TMP_Text resultText;


    [Header("Result display")]
    [SerializeField] private float resultDisplayDuration = 1.5f;
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color wrongColor = Color.red;

    private AlienInstance currentAlien;
    private Coroutine closeRoutine;

    public event System.Action OnPanelClosed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (panelRoot != null) panelRoot.SetActive(false);
        if (resultText != null) resultText.gameObject.SetActive(false);

        healerButton.onClick.AddListener(() => SubmitGuess(AlienCategory.Healer));
        damageButton.onClick.AddListener(() => SubmitGuess(AlienCategory.Damage));
        flyingButton.onClick.AddListener(() => SubmitGuess(AlienCategory.Flying));
        //tankerButton.onClick.AddListener(() => SubmitGuess(AlienCategory.Tanker));
    }


    public void Show(AlienInstance alien)
    {
        if (alien == null || alien.Guessed) return;

        if (closeRoutine != null)
        {
            StopCoroutine(closeRoutine);
            closeRoutine = null;
        }

        currentAlien = alien;
        if (resultText != null) resultText.text = "";
        if (panelRoot != null) panelRoot.SetActive(true);
    }



    private void SubmitGuess(AlienCategory guess)
    {
        if (currentAlien == null) return;

        bool correct = currentAlien.SubmitGuess(guess);
        SetButtonsInteractable(false); // stop double-clicks

        if (resultText != null)
        {
            resultText.gameObject.SetActive(true);
            resultText.text = correct ? "Correct! Essence obtained." : "Wrong — no essence this time.";
            resultText.color = correct ? correctColor : wrongColor;
        }

        closeRoutine = StartCoroutine(CloseAfterDelay());
    }

    private IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSeconds(resultDisplayDuration);
        ClosePanel();
    }

    private void ClosePanel()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
        if (resultText != null) resultText.gameObject.SetActive(false);
        SetButtonsInteractable(true);
        currentAlien = null;
        closeRoutine = null;

        OnPanelClosed?.Invoke();
    }

    private void SetButtonsInteractable(bool value)
    {
        healerButton.interactable = value;
        damageButton.interactable = value;
        flyingButton.interactable = value;
        tankerButton.interactable = value;
    }
}