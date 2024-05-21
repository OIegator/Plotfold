using UnityEngine;
using Photon.Pun;
using System;

public enum GameState
{
    Lobby,
    Starting,
    GeneratingResponse,
    Turn,
    EndTurn,
    Playing,
    Ending
}

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    public PlayerController player;
    private int _receivedResponses = 0;

    public GameState gameState = GameState.Lobby;

    public static event Action<GameState> OnGameStateChanged;

    public override void OnEnable()
    {
        player.onCardSelected.AddListener(OnPlayerCardSelected);
    }

    public override void OnDisable()
    {
        player.onCardSelected.RemoveListener(OnPlayerCardSelected);
    }

    private void OnPlayerCardSelected(int cardId)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameLogic.Instance.UpdateHostChoice(cardId);
        }
        else
        {
            photonView.RPC("ReceiveClientCardSelection", RpcTarget.MasterClient, cardId);
        }
        
        _receivedResponses++;
        if (_receivedResponses == 2)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                SetGameState(GameState.GeneratingResponse);
            }
            _receivedResponses = 0;
        }
    }
    
    
    [PunRPC]
    private void ReceiveClientCardSelection(int cardId)
    {
        GameLogic.Instance.UpdateGuestChoice(cardId);
        
        _receivedResponses++;
        if (_receivedResponses == 2)
        {
            
            if (PhotonNetwork.IsMasterClient)
            {
                SetGameState(GameState.GeneratingResponse);
            }
            _receivedResponses = 0;
        }
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
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                SetGameState(GameState.Starting);
            }
        }
        else
        {
            Debug.LogError("Not connected to Photon!");
        }
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient && gameState == GameState.Lobby)
        {
            SetGameState(GameState.Starting);
        }
    }
    
    public void SetGameState(GameState newState)
    {
        photonView.RPC("InvokeSetGameState", RpcTarget.All, newState);
    }
    
    [PunRPC]
    public void InvokeSetGameState(GameState newState)
    {
        if (gameState == newState)
            return;

        gameState = newState;
        Debug.Log("Game state changed to: " + newState);

        OnGameStateChanged?.Invoke(newState);
    }
    
}