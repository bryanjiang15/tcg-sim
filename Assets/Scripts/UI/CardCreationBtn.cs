using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class CardCreationBtn : MonoBehaviour
{
    [SerializeField] InputField cardAbilityInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        string url = "https://yourserver.com/api/cards"; // Replace with your server URL
        WWWForm form = new WWWForm();
        form.AddField("ability", cardAbility);

        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Card ability sent successfully: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error sending card ability: " + request.error);
            }
        }
    }
}
