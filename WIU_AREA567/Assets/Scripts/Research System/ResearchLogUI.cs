using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ResearchLogUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_Text notesText;
    [SerializeField] private ResearchLog researchLog;

    private void Start()
    {
        SetupScrollView();

        if (researchLog == null)
        {
            researchLog = FindFirstObjectByType<ResearchLog>();
        }

        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }

        if (researchLog != null)
        {
            researchLog.OnResearchLogChanged.AddListener(RefreshNotes);
        }
    }

    private void SetupScrollView()
    {
        if (panelRoot == null || notesText == null)
        {
            return;
        }

        RectTransform panelTransform = panelRoot.GetComponent<RectTransform>();
        RectTransform viewport = new GameObject(
            "ResearchLogViewport",
            typeof(RectTransform),
            typeof(RectMask2D)
        ).GetComponent<RectTransform>();
        viewport.SetParent(panelTransform, false);
        viewport.anchorMin = Vector2.zero;
        viewport.anchorMax = Vector2.one;
        viewport.offsetMin = new Vector2(20f, 20f);
        viewport.offsetMax = new Vector2(-20f, -20f);

        RectTransform content = new GameObject(
            "ResearchLogContent",
            typeof(RectTransform),
            typeof(ContentSizeFitter),
            typeof(VerticalLayoutGroup)
        ).GetComponent<RectTransform>();
        content.SetParent(viewport, false);
        content.anchorMin = new Vector2(0f, 1f);
        content.anchorMax = new Vector2(1f, 1f);
        content.pivot = new Vector2(0.5f, 1f);
        content.anchoredPosition = Vector2.zero;
        content.sizeDelta = Vector2.zero;

        ContentSizeFitter contentFitter = content.GetComponent<ContentSizeFitter>();
        contentFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        VerticalLayoutGroup contentLayout = content.GetComponent<VerticalLayoutGroup>();
        contentLayout.childAlignment = TextAnchor.UpperLeft;
        contentLayout.childControlWidth = true;
        contentLayout.childControlHeight = true;
        contentLayout.childForceExpandWidth = true;
        contentLayout.childForceExpandHeight = false;
        contentLayout.spacing = 8f;

        RectTransform textTransform = notesText.rectTransform;
        textTransform.SetParent(content, false);
        textTransform.anchorMin = new Vector2(0f, 1f);
        textTransform.anchorMax = new Vector2(1f, 1f);
        textTransform.pivot = new Vector2(0.5f, 1f);
        textTransform.anchoredPosition = Vector2.zero;
        textTransform.sizeDelta = new Vector2(-10f, 0f);

        ContentSizeFitter textFitter = notesText.gameObject.AddComponent<ContentSizeFitter>();
        textFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        textFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        ScrollRect scrollRect = panelRoot.AddComponent<ScrollRect>();
        scrollRect.viewport = viewport;
        scrollRect.content = content;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.inertia = true;
    }

    private void Update()
    {
        if (InputSystem.actions["OpenResearchLog"].WasPressedThisFrame())
        {
            Toggle();
        }
    }

    private void OnDestroy()
    {
        if (researchLog != null)
        {
            researchLog.OnResearchLogChanged.RemoveListener(RefreshNotes);
        }
    }

    private void Toggle()
    {
        if (panelRoot == null)
        {
            return;
        }

        bool opening = !panelRoot.activeSelf;
        if (opening)
        {
            RefreshNotes();
        }

        panelRoot.SetActive(opening);
    }

    private void RefreshNotes()
    {
        if (notesText == null)
        {
            return;
        }

        if (researchLog == null || researchLog.Discovered.Count == 0)
        {
            notesText.text = "No research notes collected.";
            return;
        }

        StringBuilder text = new StringBuilder();
        foreach (ResearchData note in researchLog.Discovered)
        {
            if (note == null) continue;
            text.Append(note.researchName);
            text.Append("\n");
            text.Append(note.description);
            text.Append("\n\n");
        }

        notesText.text = text.ToString().TrimEnd();
    }
}
