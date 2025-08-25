using UnityEngine;
using UnityEngine.UIElements;
using CardLibrary;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using System;

public class AbilityGeneratorUI : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private CardLibraryManager cardLibraryManager;

    private TextField promptInput;
    private TextField cardNameInput;
    private IntegerField powerInput;
    private IntegerField costInput;
    private TextField cardDescriptionInput;
    private Button generateButton;
    private Label statusText;

    private void OnEnable()
    {
        var root = uiDocument.rootVisualElement;
        
        // Get references to UI elements
        promptInput = root.Q<TextField>("prompt-input");
        cardNameInput = root.Q<TextField>("card-name-input");
        powerInput = root.Q<IntegerField>("power-input");
        costInput = root.Q<IntegerField>("cost-input");
        generateButton = root.Q<Button>("generate-button");
        statusText = root.Q<Label>("status-text");
        cardDescriptionInput = root.Q<TextField>("card-description-input");

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

        if (!int.TryParse(powerInput.value.ToString(), out int power))
        {
            statusText.text = "Please enter a valid power value";
            return;
        }

        if (!int.TryParse(costInput.value.ToString(), out int cost))
        {
            statusText.text = "Please enter a valid cost value";
            return;
        }

        generateButton.SetEnabled(false);
        statusText.text = "Generating ability...";

        var cardGenerationData = await cardLibraryManager.GetCardGenerator().GenerateAbilityFromPrompt(promptInput.value, cardDescriptionInput.value);

        if (cardGenerationData == null)
        {
            statusText.text = "Failed to generate ability";
            generateButton.SetEnabled(true);
            return;
        }

        AbilityDefinition? abilityDefinition = cardGenerationData.AbilityDefinition;

        Debug.Log(JsonUtility.ToJson(abilityDefinition, true));

        // Create a new card with the generated ability
        var cardDefinition = ScriptableObject.CreateInstance<SnapCardDefinition>();
        cardDefinition.abilities = new List<AbilityDefinition> { abilityDefinition };
        cardDefinition.card_name = cardNameInput.value;
        cardDefinition.cost = cost;
        cardDefinition.power = power;

        // Download and set the art
        StartCoroutine(DownloadArt(cardGenerationData.CardArtUrl, (sprite) => {
            cardDefinition.Art = sprite;
            
            // Add the card to the library
            string artPath = cardLibraryManager.SaveCardArt(Math.Abs(System.Guid.NewGuid().GetHashCode()), sprite.texture);
            cardDefinition.artPath = artPath;
            cardLibraryManager.AddCard(Math.Abs(System.Guid.NewGuid().GetHashCode()), cardDefinition);

            statusText.text = "Ability generated successfully!";
            generateButton.SetEnabled(true);
        }));
    }

    IEnumerator DownloadArt(string artUrl, Action<Sprite> onSpriteReady)
    {
        var artRequest = UnityWebRequestTexture.GetTexture(artUrl);
        
        yield return artRequest.SendWebRequest();

        if (artRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Art request failed: {artRequest.error}");
            onSpriteReady?.Invoke(null);
            yield break;
        }

        Texture2D texture = ((DownloadHandlerTexture)artRequest.downloadHandler).texture;
        
        // Convert Texture2D to Sprite
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        
        onSpriteReady?.Invoke(sprite);
    }
} 