using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Image cardImage;
    public int cardId;
    
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnCardClicked);
    }
    
    private void OnCardClicked()
    {
        Debug.Log("Card with ID " + cardId + " clicked!");
        PlayerController.Instance.SelectCard(cardId);
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
