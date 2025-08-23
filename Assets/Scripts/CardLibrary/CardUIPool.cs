using System.Collections.Generic;
using UnityEngine;
using CardLibrary;

namespace CardLibrary
{
    public class CardUIPool : Singleton<CardUIPool>
    {
        [Header("Pool Settings")]
        [SerializeField] private GameObject cardUIPrefab;
        [SerializeField] private int initialPoolSize = 20;
        [SerializeField] private int maxPoolSize = 100;

        private Queue<CardUI> availableCards = new Queue<CardUI>();
        private List<CardUI> activeCards = new List<CardUI>();
        private Transform poolParent;

        protected override void Awake()
        {
            base.Awake();
            poolParent = transform;
            InitializePool();
        }

        private void InitializePool()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                CreateNewCardUI();
            }
        }

        private void CreateNewCardUI()
        {
            if (cardUIPrefab == null) return;

            GameObject cardObj = Instantiate(cardUIPrefab, poolParent);
            CardUI cardUI = cardObj.GetComponent<CardUI>();
            
            if (cardUI != null)
            {
                cardObj.SetActive(false);
                availableCards.Enqueue(cardUI);
            }
        }

        /// <summary>
        /// Gets a CardUI from the pool or creates a new one if needed
        /// </summary>
        /// <param name="parent">Parent transform to attach the card to</param>
        /// <returns>CardUI component</returns>
        public CardUI GetCardUI(Transform parent = null)
        {
            CardUI cardUI;

            if (availableCards.Count > 0)
            {
                cardUI = availableCards.Dequeue();
            }
            else if (activeCards.Count < maxPoolSize)
            {
                CreateNewCardUI();
                cardUI = availableCards.Dequeue();
            }
            else
            {
                Debug.LogWarning("CardUI pool is full! Consider increasing maxPoolSize.");
                return null;
            }

            // Set parent and activate
            if (parent != null)
            {
                cardUI.transform.SetParent(parent, false);
            }
            
            cardUI.gameObject.SetActive(true);
            activeCards.Add(cardUI);
            
            return cardUI;
        }

        /// <summary>
        /// Returns a CardUI to the pool
        /// </summary>
        /// <param name="cardUI">CardUI to return</param>
        public void ReturnCardUI(CardUI cardUI)
        {
            if (cardUI == null) return;

            if (activeCards.Contains(cardUI))
            {
                activeCards.Remove(cardUI);
            }

            // Reset the card
            cardUI.Deselect();
            cardUI.transform.SetParent(poolParent, false);
            cardUI.gameObject.SetActive(false);
            
            availableCards.Enqueue(cardUI);
        }

        /// <summary>
        /// Returns all active CardUIs to the pool
        /// </summary>
        public void ReturnAllCards()
        {
            for (int i = activeCards.Count - 1; i >= 0; i--)
            {
                ReturnCardUI(activeCards[i]);
            }
        }

        /// <summary>
        /// Sets the card UI prefab for the pool
        /// </summary>
        /// <param name="prefab">The CardUI prefab to use</param>
        public void SetCardUIPrefab(GameObject prefab)
        {
            cardUIPrefab = prefab;
        }

        /// <summary>
        /// Gets the number of available cards in the pool
        /// </summary>
        /// <returns>Number of available cards</returns>
        public int GetAvailableCount()
        {
            return availableCards.Count;
        }

        /// <summary>
        /// Gets the number of active cards
        /// </summary>
        /// <returns>Number of active cards</returns>
        public int GetActiveCount()
        {
            return activeCards.Count;
        }

        /// <summary>
        /// Gets the total pool size
        /// </summary>
        /// <returns>Total pool size</returns>
        public int GetTotalPoolSize()
        {
            return availableCards.Count + activeCards.Count;
        }
    }
} 