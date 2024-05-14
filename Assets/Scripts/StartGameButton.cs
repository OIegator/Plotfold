using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class StartGameButton : MonoBehaviourPunCallbacks
{
    private Button _startButton;

    private void Start()
    {
        _startButton = GetComponent<Button>();

        _startButton.interactable = false;

        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _startButton.gameObject.SetActive(true);
            }
            else
            {
                _startButton.gameObject.SetActive(false);
            }
        }
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            _startButton.interactable = true;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            _startButton.interactable = true;
        }
    }

    public void StartGame()
    {
        if (_startButton.interactable && PhotonNetwork.IsMasterClient)
        {
            GameManager.Instance.SetGameState(GameState.GeneratingResponse);
            _startButton.gameObject.SetActive(false);
        }
    }
}