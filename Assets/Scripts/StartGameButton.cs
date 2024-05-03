using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class StartGameButton : MonoBehaviourPunCallbacks
{
    private Button startButton;

    private void Start()
    {
        startButton = GetComponent<Button>();

        startButton.interactable = false;

        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                startButton.gameObject.SetActive(true);
            }
        }
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            startButton.interactable = true;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            startButton.interactable = true;
        }
    }

    public void StartGame()
    {
        if (startButton.interactable && PhotonNetwork.IsMasterClient)
        {
            GameManager.Instance.SetGameState(GameState.GeneratingResponse);
            startButton.gameObject.SetActive(false);
        }
    }
}