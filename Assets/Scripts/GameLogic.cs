using System;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;

public class GameLogic : MonoBehaviourPunCallbacks
{
    public static GameLogic Instance;

    public TMP_Text textField;
    public string response;
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

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
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
                    photonView.RPC("NotifyGuestTurnState", RpcTarget.Others, response); 
                }
                textField.text = response;
                handDisplay.DisplayCards(cardManager.GetCardsByTagAndAbsurdity(turnConditions[_currentTurn].tag, turnConditions[_currentTurn].absurdity, 3));
                break;
            case GameState.Starting:
                break;
            case GameState.Playing:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }
    
    [PunRPC]
    private void NotifyGuestTurnState(string responseNew)
    {
        GameManager.Instance.SetGameState(GameState.Turn);
        response = responseNew;
    }

    private void UpdateBasePrompt()
    {
        if (_currentTurn < turnPrompts.Count - 1)
        {
            _currentTurn++;
        }
        else
        {
            _currentTurn = 0;
        }

        basePrompt = turnPrompts[_currentTurn];
    }

    public RequestObject SetupRequest()
    {
        RequestObject request = new RequestObject();

        basePrompt = basePrompt.Replace("{hostChoice}", hostChoice.ToString());
        basePrompt = basePrompt.Replace("{guestChoice}", guestChoice.ToString());

        request.messages.Add(new Message("user", basePrompt));

        return request;
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