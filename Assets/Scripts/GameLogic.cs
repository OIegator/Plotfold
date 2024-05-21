using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class GameLogic : MonoBehaviourPunCallbacks
{
    public static GameLogic Instance;

    public TMP_Text responseField;
    public TMP_Text questionField;
    
    public string response;
    private readonly RequestObject _request = new();
    
    public string currentPrompt;
    public string currentQuestion;
    private int _currentTurn;
    
    public int hostChoice;
    public int guestChoice;
    
    public HandDisplay handDisplay;
    public CardManager cardManager;
    

    public List<TurnCondition> turnConditions = new();
    public List<string> turnPrompts = new();
    public List<string> turnQuestions = new();

    [Serializable]
    public struct TurnCondition
    {
        public Tag tag;
        public int absurdity;
    }

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

    private void Start()
    {
        if (turnPrompts.Count > 0)
        {
            currentPrompt = turnPrompts[0];
            currentQuestion = turnQuestions[0];
        }
    }

    public override void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    public override void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.GeneratingResponse:
                if (PhotonNetwork.IsMasterClient)
                {
                    StartCoroutine(YandexApiRequest.Instance.GenerateResponse(SetupRequest()));
                }
                break;
            case GameState.EndTurn:
                UpdateBasePrompt();
                break;
            case GameState.Turn:
                if (PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC("NotifyGuestTurnState", RpcTarget.AllViaServer, response);
                }
                handDisplay.DisplayCards(cardManager.GetCardsByTagAndAbsurdity(turnConditions[_currentTurn].tag,
                    turnConditions[_currentTurn].absurdity, 3));
                break;
            case GameState.Starting:
                break;
            case GameState.Playing:
                break;
            case GameState.Ending:
                if (PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC("EndGame", RpcTarget.All, response);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    [PunRPC]
    private void NotifyGuestTurnState(string responseNew)
    {
        Response jsonResponse = JsonConvert.DeserializeObject<Response>(responseNew);
        if (jsonResponse != null && jsonResponse.result.alternatives.Count > 0)
        {
            string text = jsonResponse.result.alternatives[0].message.text;

            text = text.Replace("\\n", "\n");

            responseField.text = text;
            Debug.Log(text);
            responseField.gameObject.transform.position = Vector3.zero;
            questionField.text = currentQuestion;
        }
    }

    [PunRPC]
    private void EndGame(string responseNew)
    {
        Response jsonResponse = JsonConvert.DeserializeObject<Response>(responseNew);
        if (jsonResponse != null && jsonResponse.result.alternatives.Count > 0)
        {
            string text = jsonResponse.result.alternatives[0].message.text;

            text = text.Replace("\\n", "\n");

            responseField.text = text;
            questionField.text = "Спасибо за игру!";
        }
    }

    private void UpdateBasePrompt()
    {
        if (_currentTurn < turnPrompts.Count - 1)
        {
            _currentTurn++;
        }
        else
        {
            GameManager.Instance.SetGameState(GameState.Ending);
            return;
        }

        currentPrompt = turnPrompts[_currentTurn];
        currentQuestion = turnQuestions[_currentTurn];
        if (GameManager.Instance.gameState == GameState.EndTurn) GameManager.Instance.SetGameState(GameState.Turn);
    }

    private RequestObject SetupRequest()
    {
        currentPrompt = currentPrompt.Replace("{hostChoice}", CardManager.Instance.allCards[hostChoice].description);
        currentPrompt = currentPrompt.Replace("{guestChoice}", CardManager.Instance.allCards[guestChoice].description);

        _request.messages.Add(new Message("user", currentPrompt));

        Debug.Log(_request.messages[^1].text);
        return _request;
    }

    public void UpdateHostChoice(int cardId)
    {
        hostChoice = cardId;
    }

    public void UpdateGuestChoice(int cardId)
    {
        hostChoice = cardId;
    }
}