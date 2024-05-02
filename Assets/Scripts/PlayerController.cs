using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject handDisplay;

    private Card selectedCard;

    public void SelectCard(Card card)
    {
        selectedCard = card;
    }

    public void DeselectCard()
    {
        selectedCard = null;
    }
}