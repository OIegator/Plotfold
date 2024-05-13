using UnityEngine;
using System.Collections.Generic;

public class HandDisplay : MonoBehaviour
{
    public static HandDisplay Instance;
    public GameObject cardPrefab;
    public Transform hand;

    private List<GameObject> displayedCards = new List<GameObject>();
    
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
    public void DisplayCards(List<Card> cards)
    {
        ClearDisplayedCards();
        
        for (int i = 0; i < cards.Count; i++)
        {

            GameObject newCardObject = Instantiate(cardPrefab, hand);

            CardDisplay cardDisplay = newCardObject.GetComponent<CardDisplay>();
            if (cardDisplay != null)
            {
                cardDisplay.DisplayCard(cards[i]);
            }
            
            displayedCards.Add(newCardObject);
        }
    }
    
    public void ClearDisplayedCards()
    {
        foreach (GameObject cardObject in displayedCards)
        {
            Destroy(cardObject);
        }

        displayedCards.Clear();
    }
}