using UnityEngine;
using System.Collections.Generic;

public class HandDisplay : MonoBehaviour
{
    public static HandDisplay Instance;
    public GameObject cardPrefab;
    public Transform hand;

    private readonly List<GameObject> _displayedCards = new();
    
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

        foreach (var card in cards)
        {
            GameObject newCardObject = Instantiate(cardPrefab, hand);

            CardDisplay cardDisplay = newCardObject.GetComponent<CardDisplay>();
            if (cardDisplay != null)
            {
                cardDisplay.DisplayCard(card);
            }
            
            _displayedCards.Add(newCardObject);
        }
    }
    
    public void ClearDisplayedCards()
    {
        foreach (GameObject cardObject in _displayedCards)
        {
            Destroy(cardObject);
        }

        _displayedCards.Clear();
    }
}