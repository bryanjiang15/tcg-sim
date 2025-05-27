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
                return JsonUtility.FromJson<AbilityDefinition>(request.downloadHandler.text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error generating ability: {ex.Message}");
            return null;
        }
    }

    private AbilityDefinition? ConvertApiResponseToAbilityDefinition(Dictionary<string, object> apiResponse)
    {
        // Create a new AbilityDefinition
        var abilityDefinition = new AbilityDefinition();

        // Parse trigger definition
        abilityDefinition.triggerDefinition = new AbilityTriggerDefinition
        {
            trigger = ParseEnum<AbilityTrigger>(apiResponse["trigger"]?.ToString()),
            triggeredTarget = ParseTargetDefinitions(apiResponse["triggeredTarget"])
        };

        // Parse effect
        abilityDefinition.effect = ParseEnum<AbilityEffect>(apiResponse["effect"]?.ToString());

        // Parse amount
        abilityDefinition.amount = new AbilityAmount
        {
            type = ParseEnum<AbilityAmountType>(apiResponse["amountType"]?.ToString()),
            value = apiResponse["amountValue"]?.ToString()
        };

        // Parse target definitions
        abilityDefinition.targetDefinition = ParseTargetDefinitions(apiResponse["targets"]);

        // Parse activation requirements
        abilityDefinition.activationRequirements = ParseRequirements(apiResponse["requirements"]);

        // Parse activation requirement targets
        abilityDefinition.activationRequirementTargets = ParseTargetDefinition(apiResponse["requirementTargets"]);

        // Set description
        abilityDefinition.description = apiResponse["description"]?.ToString();

        return abilityDefinition;
    }

    private List<AbilityTargetDefinition> ParseTargetDefinitions(object targetsData)
    {
        if (targetsData == null) return new List<AbilityTargetDefinition>();

        var targets = new List<AbilityTargetDefinition>();
        var targetsList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(targetsData.ToString());

        foreach (var targetData in targetsList)
        {
            targets.Add(ParseTargetDefinition(targetData));
        }

        return targets;
    }

    private AbilityTargetDefinition ParseTargetDefinition(object targetData)
    {
        if (targetData == null) return new AbilityTargetDefinition(AbilityTarget.Self);

        var targetDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(targetData.ToString());
        
        return new AbilityTargetDefinition(
            ParseEnum<AbilityTarget>(targetDict["target"]?.ToString()),
            ParseEnum<AbilityTargetRange>(targetDict["range"]?.ToString()),
            ParseEnum<AbilityTargetSort>(targetDict["sort"]?.ToString())
        );
    }

    private List<AbilityRequirement> ParseRequirements(object requirementsData)
    {
        if (requirementsData == null) return new List<AbilityRequirement>();

        var requirements = new List<AbilityRequirement>();
        var requirementsList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(requirementsData.ToString());

        foreach (var reqData in requirementsList)
        {
            requirements.Add(new AbilityRequirement
            {
                ReqType = ParseEnum<AbilityRequirementType>(reqData["type"]?.ToString()),
                ReqComparator = ParseEnum<AbilityRequirementComparator>(reqData["comparator"]?.ToString()),
                ReqCondition = ParseEnum<AbilityRequirementCondition>(reqData["condition"]?.ToString()),
                ReqAmount = new AbilityAmount
                {
                    type = ParseEnum<AbilityAmountType>(reqData["amountType"]?.ToString()),
                    value = reqData["amountValue"]?.ToString()
                }
            });
        }

        return requirements;
    }

    private T ParseEnum<T>(string value) where T : struct
    {
        if (string.IsNullOrEmpty(value)) return default(T);
        return Enum.TryParse<T>(value, out T result) ? result : default(T);
    }
}
