using UnityEngine;
using System;
using System.Collections;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.Networking;

public class CardGenerator
{
#if UNITY_EDITOR
    private const string API_ENDPOINT = "http://localhost:8000/parse-ability"; // Development
#else
    private const string API_ENDPOINT = "https://your-production-domain.com/parse-ability"; // Production
#endif

    private struct CardAbilityRequest
    {
        public string description;
    }

    public async Task<AbilityDefinition?> GenerateAbilityFromPrompt(string prompt)
    {
        try
        {
            // Create the request payload
            CardAbilityRequest requestData = new CardAbilityRequest { description = prompt };
            string jsonData = JsonUtility.ToJson(requestData);
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
                return JsonUtility.FromJson<AbilityDefinition>(request.downloadHandler.text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error generating ability: {ex.Message}");
            return null;
        }
    }
}
