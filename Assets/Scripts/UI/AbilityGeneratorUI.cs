using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using System.Linq;

public class AbilityGeneratorUI : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private CardLibraryManager cardLibraryManager;

    private TextField promptInput;
    private Button generateButton;
    private Label statusText;

    private void OnEnable()
    {
        var root = uiDocument.rootVisualElement;
        
        // Get references to UI elements
        promptInput = root.Q<TextField>("prompt-input");
        generateButton = root.Q<Button>("generate-button");
        statusText = root.Q<Label>("status-text");

        // Register button click event
        generateButton.clicked += OnGenerateButtonClicked;
    }

    private void OnDisable()
    {
        // Unregister button click event
        if (generateButton != null)
        {
            generateButton.clicked -= OnGenerateButtonClicked;
        }
    }

    private async void OnGenerateButtonClicked()
    {
        if (string.IsNullOrEmpty(promptInput.value))
        {
            statusText.text = "Please enter a prompt";
            return;
        }

        generateButton.SetEnabled(false);
        statusText.text = "Generating ability...";

        var abilityDefinition = await cardLibraryManager.GetCardGenerator().GenerateAbilityFromPrompt(promptInput.value);

        if (abilityDefinition == null)
        {
            statusText.text = "Failed to generate ability";
            generateButton.SetEnabled(true);
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
        generateButton.SetEnabled(true);
    }
} 