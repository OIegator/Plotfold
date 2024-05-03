using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public GameObject handDisplay;
    
    public UnityEvent<int> onCardSelected = new UnityEvent<int>();

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
    public void SelectCard(int cardId)
    {
        onCardSelected.Invoke(cardId);
    }
    
}