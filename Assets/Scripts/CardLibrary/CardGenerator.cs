using UnityEngine;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.Networking;
using Mono.Cecil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CardLibrary;

public class CardGenerator
{
#if UNITY_EDITOR
    private const string API_ENDPOINT = "http://localhost:8000/parse-ability"; // Development
#else
    private const string API_ENDPOINT = "https://your-production-domain.com/parse-ability"; // Production
#endif

    [System.Serializable]
    private class CardAbilityRequest
    {
        public string abilityDescription;
        public string cardDescription;
    }

    public async Task<CardGenerationData?> GenerateAbilityFromPrompt(string prompt, string cardDescription)
    {
        try
        {
            // Create the request payload
            CardAbilityRequest requestData = new CardAbilityRequest { abilityDescription = prompt, cardDescription = cardDescription };
            string jsonData = JsonConvert.SerializeObject(requestData);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

            // Create the web request
            using (UnityWebRequest request = new UnityWebRequest(API_ENDPOINT, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                // Send the request
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"API request failed: {request.error}");
                    Debug.LogError($"Response: {request.downloadHandler.text}");
                    Debug.LogError($"Sent data: {jsonData}");
                    return null;
                }
                // Parse the API response
                Debug.Log("Response: " + request.downloadHandler.text);

                // Try to deserialize the response using Newtonsoft.Json for better handling
                CardGenerationData result;
                try
                {
                    // First deserialize to a JObject to handle the polymorphic deserialization manually
                    var jsonObject = JObject.Parse(request.downloadHandler.text);
                    
                    result = new CardGenerationData();
                    
                    // Handle CardArtUrl
                    if (jsonObject["CardArtUrl"] != null)
                        result.CardArtUrl = jsonObject["CardArtUrl"].ToString();
                    
                    // Handle AbilityDefinition with custom deserialization
                    if (jsonObject["AbilityDefinition"] != null)
                    {
                        var abilityJson = jsonObject["AbilityDefinition"] as JObject;
                        if (abilityJson != null)
                        {
                            result.AbilityDefinition = CardLibraryDeserializer.DeserializeAbilityDefinition(abilityJson);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"JSON deserialization failed: {ex.Message}");
                    Debug.LogError($"JSON content: {request.downloadHandler.text}");
                    return null;
                }
                
                if (result == null)
                {
                    Debug.LogError("Failed to deserialize CardGenerationData");
                    return null;
                }
                
                if (result.AbilityDefinition == null)
                {
                    Debug.LogError("AbilityDefinition is null after deserialization");
                    return null;
                }
                
                Debug.Log("Successfully deserialized CardGenerationData");
                Debug.Log("AbilityDefinition: " + JsonConvert.SerializeObject(result.AbilityDefinition, Formatting.Indented));

                return result;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error generating ability: {ex.Message}");
            return null;
        }
    }
}
