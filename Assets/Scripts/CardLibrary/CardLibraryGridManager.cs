using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using CardHouse;

public class CardLibraryGridManager : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup cardLibraryGrid;
    [SerializeField] private GameObject cardUIPrefab;
    [SerializeField] private int cardsPerRow = 4;
    [SerializeField] private float cellWidth = 200f;
    [SerializeField] private float cellHeight = 300f;
    [SerializeField] private float spacing = 20f;

    private void Start()
    {
        ConfigureGridLayout();
        PopulateGrid();
    }

    private void ConfigureGridLayout()
    {
        if (cardLibraryGrid != null)
        {
            cardLibraryGrid.cellSize = new Vector2(cellWidth, cellHeight);
            cardLibraryGrid.spacing = new Vector2(spacing, spacing);
            cardLibraryGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            cardLibraryGrid.constraintCount = cardsPerRow;
        }
    }

    private void PopulateGrid()
    {
        if (cardLibraryGrid == null || cardUIPrefab == null) return;

        // Clear existing cards
        foreach (Transform child in cardLibraryGrid.transform)
        {
            Destroy(child.gameObject);
        }

        // Get all cards from the library
        var allCards = CardLibraryManager.Instance.GetAllCards();

        // Create UI elements for each card
        foreach (var cardEntry in allCards)
        {
            for (int i = 0; i < cardEntry.quantity; i++)
            {
                GameObject cardUI = Instantiate(cardUIPrefab, cardLibraryGrid.transform);
                CardUI cardUIComponent = cardUI.GetComponent<CardUI>();
                
                if (cardUIComponent != null)
                {
                    cardUIComponent.Initialize(cardEntry.cardDefinition, cardEntry.isFoil);
                }
            }
        }
    }

    public void RefreshGrid()
    {
        PopulateGrid();
    }
} 