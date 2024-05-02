using System;
using UnityEngine;
using System.Collections.Generic;

public class GameLogic : MonoBehaviour
{
    public string basePrompt;
    public string player1Choice;
    public string player2Choice;

    public int numberOfTurns = 5;

    public List<TurnCondition> turnConditions = new List<TurnCondition>();

    public List<string> turnPrompts = new List<string>();

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
            case GameState.Turn:

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    [System.Serializable]
    public struct TurnCondition
    {
        public Tag tag;
        public int absurdity;
    }

    public RequestObject SetupRequest()
    {
        RequestObject request = new RequestObject();

        basePrompt = basePrompt.Replace("{player1Choice}", player1Choice);
        basePrompt = basePrompt.Replace("{player2Choice}", player2Choice);

        request.messages.Add(new Message("user", basePrompt));

        return request;
    }
}