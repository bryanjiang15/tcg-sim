using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System.Linq;

public class AbilityGeneratorUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI promptInput;
    [SerializeField] private Button generateButton;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private CardLibraryManager cardLibraryManager;

    private void Start()
    {
        generateButton.onClick.AddListener(OnGenerateButtonClicked);
    }

    private async void OnGenerateButtonClicked()
    {
        if (string.IsNullOrEmpty(promptInput.text))
        {
            statusText.text = "Please enter a prompt";
            return;
        }

        generateButton.interactable = false;
        statusText.text = "Generating ability...";

        var abilityDefinition = await cardLibraryManager.GetCardGenerator().GenerateAbilityFromPrompt(promptInput.text);

        if (abilityDefinition == null)
        {
            statusText.text = "Failed to generate ability";
            generateButton.interactable = true;
            return;
        }

        Debug.Log(JsonUtility.ToJson(abilityDefinition, true));

        // Create a new card with the generated ability
        var cardDefinition = ScriptableObject.CreateInstance<SnapCardDefinition>();
        cardDefinition.abilities = new[] { abilityDefinition.Value };
        cardDefinition.card_name = "Generated Card";
        cardDefinition.cost = 1;
        cardDefinition.power = 1;

        // Add the card to the library
        cardLibraryManager.AddCard("generated_" + System.Guid.NewGuid().ToString(), cardDefinition);

        statusText.text = "Ability generated successfully!";
        generateButton.interactable = true;
    }
} 