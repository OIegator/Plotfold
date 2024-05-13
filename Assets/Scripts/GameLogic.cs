using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class GameLogic : MonoBehaviourPunCallbacks
{
    public static GameLogic Instance;

    public TMP_Text textField;
    public string response;
    private readonly RequestObject _request = new();
    public string basePrompt;
    public int hostChoice;
    public int guestChoice;
    public HandDisplay handDisplay;
    public CardManager cardManager;

    public int numberOfTurns = 5;
    private int _currentTurn = 0;

    public List<TurnCondition> turnConditions = new List<TurnCondition>();

    public List<string> turnPrompts = new List<string>();

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
            basePrompt = turnPrompts[0];
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
                StartCoroutine(YandexApiRequest.Instance.GenerateResponse(SetupRequest()));
                break;
            case GameState.EndTurn:
                UpdateBasePrompt();
                break;
            case GameState.Turn:
                if (PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC("NotifyGuestTurnState", RpcTarget.All, response);
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
        if (jsonResponse.result.alternatives.Count > 0)
        {
            string text = jsonResponse.result.alternatives[0].message.text;

            text = text.Replace("\\n", "\n");

            textField.text = text;
        }
    }

    [PunRPC]
    private void EndGame(string responseNew)
    {
        Response jsonResponse = JsonConvert.DeserializeObject<Response>(responseNew);
        if (jsonResponse.result.alternatives.Count > 0)
        {
            string text = jsonResponse.result.alternatives[0].message.text;

            text = text.Replace("\\n", "\n");

            textField.text = text + " \n \n Спасибо за игру!";
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

        basePrompt = turnPrompts[_currentTurn];
        if (GameManager.Instance.gameState == GameState.EndTurn) GameManager.Instance.SetGameState(GameState.Turn);
    }

    private RequestObject SetupRequest()
    {
        basePrompt = basePrompt.Replace("{hostChoice}", CardManager.Instance.allCards[hostChoice].description);
        basePrompt = basePrompt.Replace("{guestChoice}", CardManager.Instance.allCards[guestChoice].description);

        _request.messages.Add(new Message("user", basePrompt));

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