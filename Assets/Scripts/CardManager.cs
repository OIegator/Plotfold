using UnityEngine;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    public List<Card> allCards = new List<Card>();

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

    public List<Card> GetCardsByTagAndAbsurdity(Tag tag, int absurdity, int count)
    {
        List<Card> filteredCards = new List<Card>();

        foreach (Card card in allCards)
        {
            foreach (var pair in card.tags)
            {
                if (pair.tag == tag && pair.value == absurdity)
                {
                    filteredCards.Add(card);
                    break;
                }
            }
        }

        if (filteredCards.Count == 0)
        {
            Debug.LogWarning("CardManager: No cards match the specified criteria.");
            return filteredCards;
        }

        List<Card> randomCards = new List<Card>();
        HashSet<int> chosenIndices = new HashSet<int>();
        int cardsToChoose = Mathf.Min(count, filteredCards.Count);
        while (randomCards.Count < cardsToChoose)
        {
            int randomIndex = Random.Range(0, filteredCards.Count);
            if (!chosenIndices.Contains(randomIndex))
            {
                randomCards.Add(filteredCards[randomIndex]);
                chosenIndices.Add(randomIndex);
            }
        }

        return randomCards;
    }


    public void AddCard(Card newCard)
    {
        allCards.Add(newCard);
    }
}