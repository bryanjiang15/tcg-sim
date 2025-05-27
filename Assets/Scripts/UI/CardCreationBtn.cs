using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
public class CardCreationBtn : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI cardAbilityInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private struct CardAbilityRequest
    {
        public string description;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateCard()
    {
        string cardAbility = cardAbilityInput.text;
        StartCoroutine(SendCardAbilityToServer(cardAbility));
    }

    private IEnumerator SendCardAbilityToServer(string cardAbility)
    {
        string url = "http://localhost:8000/parse-ability";
        
        // Create request object and serialize to JSON
        CardAbilityRequest requestData = new CardAbilityRequest { description = cardAbility };
        string jsonData = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Card ability sent successfully: " + request.downloadHandler.text);
                AbilityDefinition abilityDefinition = JsonUtility.FromJson<AbilityDefinition>(request.downloadHandler.text);
                Debug.Log(abilityDefinition);
            }
            else
            {
                Debug.LogError($"Error sending card ability. Status: {request.responseCode}, Error: {request.error}");
                Debug.LogError($"Response: {request.downloadHandler.text}");
                Debug.LogError($"Sent data: {jsonData}");
            }
        }
    }
}
