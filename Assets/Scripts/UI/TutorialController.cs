using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialController : MonoBehaviour
{
    [Header("Tutorial Panels")]
    [SerializeField] private GameObject movementPanel;
    [SerializeField] private GameObject combatPanel;
    [SerializeField] private GameObject itemsPanel;
    [SerializeField] private GameObject objectivePanel;

    [Header("Tutorial Triggers")]
    [SerializeField] private Transform movementTrigger;
    [SerializeField] private Transform combatTrigger;
    [SerializeField] private Transform itemsTrigger;
    [SerializeField] private Transform objectiveTrigger;

    [Header("Settings")]
    [SerializeField] private float panelDisplayDuration = 5f;
    [SerializeField] private Button skipButton;

    private List<GameObject> shownPanels = new List<GameObject>();
    private bool tutorialCompleted = false;

    void Start()
    {
        // Hide all panels initially
        HideAllPanels();

        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipTutorial);
        }

        // Start tutorial
        StartCoroutine(TutorialSequence());
    }

    IEnumerator TutorialSequence()
    {
        // Wait a moment for scene to load
        yield return new WaitForSeconds(1f);

        // Show movement tutorial
        if (movementPanel != null)
        {
            ShowPanel(movementPanel);
            yield return new WaitForSeconds(panelDisplayDuration);
            HidePanel(movementPanel);
        }

        yield return new WaitForSeconds(1f);

        // Show combat tutorial
        if (combatPanel != null)
        {
            ShowPanel(combatPanel);
            yield return new WaitForSeconds(panelDisplayDuration);
            HidePanel(combatPanel);
        }

        yield return new WaitForSeconds(1f);

        // Show items tutorial
        if (itemsPanel != null)
        {
            ShowPanel(itemsPanel);
            yield return new WaitForSeconds(panelDisplayDuration);
            HidePanel(itemsPanel);
        }

        yield return new WaitForSeconds(1f);

        // Show objective tutorial
        if (objectivePanel != null)
        {
            ShowPanel(objectivePanel);
            yield return new WaitForSeconds(panelDisplayDuration);
            HidePanel(objectivePanel);
        }

        tutorialCompleted = true;
    }

    void ShowPanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(true);
            shownPanels.Add(panel);
        }
    }

    void HidePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    void HideAllPanels()
    {
        if (movementPanel != null) movementPanel.SetActive(false);
        if (combatPanel != null) combatPanel.SetActive(false);
        if (itemsPanel != null) itemsPanel.SetActive(false);
        if (objectivePanel != null) objectivePanel.SetActive(false);
    }

    public void SkipTutorial()
    {
        StopAllCoroutines();
        HideAllPanels();
        tutorialCompleted = true;

        // Start the game
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
    }

    public bool IsTutorialCompleted()
    {
        return tutorialCompleted;
    }

    void OnDestroy()
    {
        if (skipButton != null)
        {
            skipButton.onClick.RemoveAllListeners();
        }
    }
}

