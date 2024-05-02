using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public enum GameState
{
    Lobby,
    Starting,
    GeneratingResponse,
    Turn,
    Playing,
    Ending
}

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    private GameState gameState = GameState.Lobby;

    public static event Action<GameState> OnGameStateChanged;

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
        if (gameState == newState)
            return;

        gameState = newState;
        Debug.Log("Game state changed to: " + newState);

        OnGameStateChanged?.Invoke(newState);
    }
}