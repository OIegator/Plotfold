using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Image cardImage;
    public int cardId;
    
    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnCardClicked);
    }
    
    private void OnCardClicked()
    {
        Debug.Log("Card with ID " + cardId + " clicked!");
        PlayerController.Instance.SelectCard(cardId);
        HandDisplay.Instance.ClearDisplayedCards();
    }
    public void DisplayCard(Card card)
    {
        if (cardImage != null)
        {
            cardImage.sprite = card.image;
        }

        cardId = card.id;
    }
}
