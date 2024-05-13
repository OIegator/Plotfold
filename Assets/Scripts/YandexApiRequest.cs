using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class YandexApiRequest : MonoBehaviour
{
    public static YandexApiRequest Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator GenerateResponse(RequestObject request)
    {
        string jsonData = JsonUtility.ToJson(request);

        using (UnityWebRequest www = new UnityWebRequest(Constants.BaseUrl, "POST"))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", "Bearer " + Constants.IamToken);
            www.SetRequestHeader("x-folder-id", Constants.FolderID);


            byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(jsonBytes);

            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();


            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                GameLogic.Instance.response = www.downloadHandler.text;
                GameManager.Instance.SetGameState(GameState.EndTurn);
            }
        }
    }
}