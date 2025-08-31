using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using CardHouse;

/// <summary>
/// Registry for storing DeckDefinition entries keyed by deck ID.
/// Persists via SnapDeckData JSON and reconstructs ScriptableObjects on load.
/// </summary>
public class DeckRegistry : Singleton<DeckRegistry>
{
	[SerializeField] private string saveFileName = "deck_registry.json";
	private readonly Dictionary<int, SnapDeckData> _decks = new Dictionary<int, SnapDeckData>();
	private int _nextId = 1;
	private string _savePath;

	protected override void Awake()
	{
		base.Awake();
		_savePath = Path.Combine(Application.persistentDataPath, saveFileName);
		Load();
	}

	public int Count => _decks.Count;
	public IEnumerable<int> AllIds => _decks.Keys;
	public IEnumerable<string> AllNames => _decks.Values.Select(d => d.deckName);
	public bool HasDeck(int deckId) => _decks.ContainsKey(deckId);
	public bool HasName(string deckName) => !string.IsNullOrEmpty(deckName) && _decks.Values.Any(d => d.deckName == deckName);

	public DeckDefinition GetDeckDefinition(int deckId)
	{
		if (!_decks.TryGetValue(deckId, out var data)) return null;
		return BuildDeckDefinition(data);
	}

	public DeckDefinition GetDeckDefinitionByName(string deckName)
	{
		var data = _decks.Values.FirstOrDefault(d => d.deckName == deckName);
		return data != null ? BuildDeckDefinition(data) : null;
	}

	private DeckDefinition BuildDeckDefinition(SnapDeckData data)
	{
		var deck = ScriptableObject.CreateInstance<DeckDefinition>();
		deck.name = data.deckName;
		deck.CardCollection = new List<SnapCardDefinition>();
		foreach (var id in data.CardIds)
		{
			var cardData = CardRegistry.Instance.GetCard(id);
			if (cardData != null)
			{
				deck.CardCollection.Add(cardData.GetCardDefinition());
			}
		}
		return deck;
	}

	public IEnumerable<DeckDefinition> GetAllDecks()
	{
		return _decks.Values.Select(d => BuildDeckDefinition(d));
	}

	public int InsertDeck(string deckName, IEnumerable<int> cardIds)
	{
		if (string.IsNullOrWhiteSpace(deckName))
		{
			Debug.LogError("Deck name cannot be empty");
			return -1;
		}

		if (_decks.Values.Any(d => d.deckName == deckName))
		{
			Debug.LogError($"Deck '{deckName}' already exists. Use Update instead.");
			return -1;
		}

		var idList = cardIds?.ToList() ?? new List<int>();
		var deckId = _nextId++;
		_decks[deckId] = new SnapDeckData
		{
			deckId = deckId,
			deckName = deckName,
			CardIds = idList,
			dateCreated = DateTime.Now,
			dateUpdated = DateTime.Now,
			deckArtPath = string.Empty
		};

		Save();
		return deckId;
	}

	public bool UpdateDeck(int deckId, IEnumerable<int> cardIds) 
	{
		if (!_decks.TryGetValue(deckId, out var data))
		{
			Debug.LogError($"Deck with ID '{deckId}' not found.");
			return false;
		}

		var idList = cardIds?.ToList() ?? new List<int>();
		data.CardIds = idList;
		data.dateUpdated = DateTime.Now;

		Save();
		return true;
	}

	public bool UpdateDeckByName(string deckName, IEnumerable<int> cardIds) 
	{
		var data = _decks.Values.FirstOrDefault(d => d.deckName == deckName);
		if (data == null)
		{
			Debug.LogError($"Deck '{deckName}' not found. Use Insert instead.");
			return false;
		}

		var idList = cardIds?.ToList() ?? new List<int>();
		data.CardIds = idList;
		data.dateUpdated = DateTime.Now;

		Save();
		return true;
	}

	public bool RemoveDeck(int deckId)
	{
		if (_decks.Remove(deckId))
		{
			Save();
			return true;
		}
		return false;
	}

	public bool RemoveDeckByName(string deckName)
	{
		var data = _decks.Values.FirstOrDefault(d => d.deckName == deckName);
		if (data != null && _decks.Remove(data.deckId))
		{
			Save();
			return true;
		}
		return false;
	}

	public List<int> FindByCardId(int cardId)
	{
		return _decks.Values
			.Where(d => d.CardIds != null && d.CardIds.Contains(cardId))
			.Select(d => d.deckId)
			.ToList();
	}

	public List<string> FindNamesByCardId(int cardId)
	{
		return _decks.Values
			.Where(d => d.CardIds != null && d.CardIds.Contains(cardId))
			.Select(d => d.deckName)
			.ToList();
	}

	public void Save()
	{
		try
		{
			var payload = _decks.Values.ToList();
			var json = JsonConvert.SerializeObject(payload, Formatting.Indented);
			File.WriteAllText(_savePath, json);
			// Debug.Log($"DeckRegistry saved {_decks.Count} entries to {_savePath}");
		}
		catch (Exception e)
		{
			Debug.LogError($"DeckRegistry save failed: {e.Message}");
		}
	}

	public void Load()
	{
		try
		{
			if (!File.Exists(_savePath))
			{
				_decks.Clear();
				_nextId = 1;
				return;
			}
			var json = File.ReadAllText(_savePath);
			_decks.Clear();
			_nextId = 1;

			// Try new schema first
			var newPayload = JsonConvert.DeserializeObject<List<SnapDeckData>>(json);
			if (newPayload != null && newPayload.Count > 0)
			{
				foreach (var data in newPayload)
				{
					if (data != null && !string.IsNullOrEmpty(data.deckName))
					{
						_decks[data.deckId] = data;
						if (data.deckId >= _nextId) _nextId = data.deckId + 1;
					}
				}
			}
			else
			{
				// Backward compatibility: legacy DeckRecord list
				try
				{
					var legacy = JsonConvert.DeserializeObject<List<LegacyDeckRecord>>(json) ?? new List<LegacyDeckRecord>();
					foreach (var rec in legacy)
					{
						if (rec != null && !string.IsNullOrEmpty(rec.deckName))
						{
							var mapped = new SnapDeckData
							{
								deckId = rec.id,
								deckName = rec.deckName,
								CardIds = rec.cardIds ?? new List<int>(),
								dateCreated = rec.createdAt,
								dateUpdated = rec.updatedAt,
								deckArtPath = string.Empty
							};
							_decks[mapped.deckId] = mapped;
							if (mapped.deckId >= _nextId) _nextId = mapped.deckId + 1;
						}
					}
				}
				catch
				{
					// ignore
				}
			}
			// Debug.Log($"DeckRegistry loaded {_decks.Count} entries from {_savePath}");
		}
		catch (Exception e)
		{
			Debug.LogError($"DeckRegistry load failed: {e.Message}");
			_decks.Clear();
			_nextId = 1;
		}
	}

	[Serializable]
	private class LegacyDeckRecord
	{
		public int id;
		public string deckName;
		public List<int> cardIds = new List<int>();
		public DateTime createdAt;
		public DateTime updatedAt;
	}
}
