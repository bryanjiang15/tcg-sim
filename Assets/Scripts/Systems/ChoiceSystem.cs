using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChoiceSystem : Singleton<ChoiceSystem>
{

    [Header("UI References")]
    public GameObject choicePanel; // The panel that holds the choice UI
    public TextMeshProUGUI promptText; // The prompt/question
    public Button confirmationButton; // The confirm button

    private IChoice currentChoice;

    private void Start()
    {
        if (choicePanel != null)
            choicePanel.SetActive(false);
    }

    /// <summary>
    /// Call this to show a choice to the player.
    /// </summary>
    public void ShowChoice(IChoice choice)
    {
        currentChoice = choice;

        if (promptText != null)
            promptText.text = choice.Prompt;

        if (confirmationButton != null)
            confirmationButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Call this to confirm the choice.
    /// </summary>
    public void ConfirmChoice()
    {
        if (confirmationButton != null)
            confirmationButton.gameObject.SetActive(false);

        if (choicePanel != null)
            choicePanel.SetActive(false);
        
        currentChoice.ConfirmChoice();

        currentChoice = null;
    }
}