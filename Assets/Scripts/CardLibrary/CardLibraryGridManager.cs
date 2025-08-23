using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using CardHouse;
using CardLibrary;

public class CardLibraryGridManager : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup cardLibraryGrid;
    [SerializeField] private GameObject cardUIPrefab;
    [SerializeField] private Button deleteButton;
    [SerializeField] private int cardsPerRow = 4;
    [SerializeField] private float cellWidth = 200f;
    [SerializeField] private float cellHeight = 300f;
    [SerializeField] private float spacing = 20f;
    private List<CardUI> activeCardUIs = new List<CardUI>();

    private void Start()
    {
        InitializeCardUIPool();
        ConfigureGridLayout();
        PopulateGrid();
    }

    /// <summary>
    /// Handles the delete button click event
    /// Deletes selected cards from the CardLibraryManager and updates the grid
    /// </summary>
    public void OnDeleteButtonClick()
    {
        // Find all selected cards
        List<CardEntry> selectedCards = new List<CardEntry>();
        
        foreach (var cardUI in activeCardUIs)
        {
            if (cardUI != null && cardUI.IsSelected())
            {
                CardEntry cardEntry = cardUI.GetCardEntry();
                if (cardEntry != null)
                {
                    selectedCards.Add(cardEntry);
                }
            }
        }

        // Delete selected cards from the library
        foreach (var cardEntry in selectedCards)
        {
            CardLibraryManager.Instance.RemoveCard(cardEntry.cardId);
            Debug.Log($"Deleted card: {cardEntry.cardName} (ID: {cardEntry.cardId})");
        }

        // Refresh the grid to reflect the changes
        RefreshGrid();
        
        Debug.Log($"Deleted {selectedCards.Count} card(s) from the library");
    }

    private void InitializeCardUIPool()
    {
        // Create a pool object as a child of this transform
        GameObject poolObject = new GameObject("CardUIPool");
        poolObject.transform.SetParent(transform);
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
        if (cardLibraryGrid == null || CardUIPool.Instance == null) return;

        // Clear existing cards
        ClearGrid();

        // Get all cards from the library
        var allCards = CardLibraryManager.Instance.GetAllCards();

        // Create UI elements for each card using the pool
        foreach (var cardEntry in allCards)
        {
            for (int i = 0; i < cardEntry.quantity; i++)
            {
                CardUI cardUI = CardUIPool.Instance.GetCardUI(cardLibraryGrid.transform);
                
                if (cardUI != null)
                {
                    cardUI.Initialize(cardEntry, cardEntry.isFoil);
                    activeCardUIs.Add(cardUI);
                }
            }
        }
    }

    private void ClearGrid()
    {
        // Return all active cards to the pool
        foreach (var cardUI in activeCardUIs)
        {
            if (cardUI != null)
            {
                CardUIPool.Instance.ReturnCardUI(cardUI);
            }
        }
        
        activeCardUIs.Clear();
    }

    /// <summary>
    /// Refreshes the grid with updated card data
    /// </summary>
    public void RefreshGrid()
    {
        PopulateGrid();
    }

    /// <summary>
    /// Gets the number of active CardUIs
    /// </summary>
    /// <returns>Number of active CardUIs</returns>
    public int GetActiveCardCount()
    {
        return activeCardUIs.Count;
    }

    /// <summary>
    /// Gets pool statistics for debugging
    /// </summary>
    /// <returns>Pool statistics string</returns>
    public string GetPoolStats()
    {
        if (CardUIPool.Instance == null) return "Pool not initialized";
        
        return $"Active: {CardUIPool.Instance.GetActiveCount()}, " +
               $"Available: {CardUIPool.Instance.GetAvailableCount()}, " +
               $"Total: {CardUIPool.Instance.GetTotalPoolSize()}";
    }
} 