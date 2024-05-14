using UnityEngine;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    public List<Card> allCards = new();

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
        List<Card> filteredTagCards = new List<Card>();
        List<Card> matchingAbsurdityCards = new List<Card>();

        foreach (Card card in allCards)
        {
            foreach (var pair in card.tags)
            {
                if (pair.tag == tag)
                {
                    filteredTagCards.Add(card);

                    if (pair.value == absurdity)
                    {
                        matchingAbsurdityCards.Add(card);
                    }

                    break;
                }
            }
        }

        if (filteredTagCards.Count == 0)
        {
            Debug.LogWarning("CardManager: No cards match the specified tag.");
            return filteredTagCards;
        }

        List<Card> randomCards = new List<Card>();
        
        // If there are cards with the corresponding absurdity value, choose at least one of them
        if (matchingAbsurdityCards.Count > 0)
        {
            int randomIndex = Random.Range(0, matchingAbsurdityCards.Count);
            filteredTagCards.Remove(matchingAbsurdityCards[randomIndex]);
            randomCards.Add(matchingAbsurdityCards[randomIndex]);
        }

        // Pick the rest of the random cards
        HashSet<int> chosenIndices = new HashSet<int>();
        int cardsToChoose = Mathf.Min(count - 1, filteredTagCards.Count);
        while (randomCards.Count < cardsToChoose)
        {
            int randomIndex = Random.Range(0, filteredTagCards.Count);
            if (!chosenIndices.Contains(randomIndex))
            {
                randomCards.Add(filteredTagCards[randomIndex]);
                chosenIndices.Add(randomIndex);
            }
        }

        return randomCards;
    }
}