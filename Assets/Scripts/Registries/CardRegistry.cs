using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Registry for storing all SnapCardData entries keyed by cardId.
/// Provides JSON persistence and simple CRUD/query APIs.
/// </summary>
public class CardRegistry : Singleton<CardRegistry>
{
	[SerializeField] private string saveFileName = "card_registry.json";
	private readonly Dictionary<int, SnapCardData> _cards = new Dictionary<int, SnapCardData>();
	private string _savePath;

	private int _nextCardId = 1;

	protected override void Awake()
	{
		base.Awake();
		_savePath = Path.Combine(Application.persistentDataPath, saveFileName);
		Load();
	}

	public int Count => _cards.Count;
	public IEnumerable<int> AllIds => _cards.Keys;

	public bool HasCard(int cardId) => _cards.ContainsKey(cardId);

	public SnapCardData GetCard(int cardId)
	{
		return _cards.TryGetValue(cardId, out var data) ? data : null;
	}

	public List<SnapCardData> GetAllCards()
	{
		return _cards.Values.ToList();
	}

	public bool TryGetCard(int cardId, out SnapCardData data)
	{
		if (_cards.TryGetValue(cardId, out var found))
		{
			data = found;
			return true;
		}
		data = null;
		return false;
	}

	public bool InsertCard(SnapCardData data)
	{
		if (data == null)
		{
			Debug.LogError("Cannot add null SnapCardData to CardRegistry");
			return false;
		}

		// Assign a new id if missing or already taken
		if (data.cardId <= 0 || _cards.ContainsKey(data.cardId))
		{
			data.cardId = _nextCardId;
		}

		// Set timestamps
		if (data.dateCreated == default)
		{
			data.dateCreated = DateTime.Now;
		}
		data.dateUpdated = DateTime.Now;

		_cards[data.cardId] = data;

		// Bump next id if needed
		_nextCardId = Math.Max(_nextCardId, data.cardId + 1);

		Save();
		return true;
	}

	public bool UpdateCard(int cardId, SnapCardData data)
	{
		if (data == null)
		{
			Debug.LogError("Cannot update with null SnapCardData");
			return false;
		}

		if (!_cards.TryGetValue(cardId, out var existing))
		{
			Debug.LogError($"Card with ID {cardId} not found. Use Insert instead.");
			return false;
		}

		// Preserve original created date
		if (data.dateCreated == default)
		{
			data.dateCreated = existing.dateCreated;
		}
		data.cardId = cardId;
		data.dateUpdated = DateTime.Now;

		_cards[cardId] = data;

		Save();
		return true;
	}

	public bool RemoveCard(int cardId)
	{
		if (_cards.Remove(cardId))
		{
			Save();
			return true;
		}
		return false;
	}

	public List<int> FindByName(string namePart)
	{
		if (string.IsNullOrWhiteSpace(namePart)) return new List<int>();
		namePart = namePart.Trim();
		return _cards.Values
			.Where(d => !string.IsNullOrEmpty(d.card_name) && d.card_name.IndexOf(namePart, StringComparison.OrdinalIgnoreCase) >= 0)
			.Select(d => d.cardId)
			.ToList();
	}

	public void Save()
	{
		try
		{
			var payload = _cards.Values.ToList();
			var json = JsonConvert.SerializeObject(payload, Formatting.Indented);
			File.WriteAllText(_savePath, json);
			// Debug.Log($"CardRegistry saved {_cards.Count} entries to {_savePath}");
		}
		catch (Exception e)
		{
			Debug.LogError($"CardRegistry save failed: {e.Message}");
		}
	}

	public void Load()
	{
		try
		{
			if (!File.Exists(_savePath))
			{
				_cards.Clear();
				_nextCardId = 1;
				return;
			}
			var json = File.ReadAllText(_savePath);
			_cards.Clear();

			// Try new schema: list of SnapCardData
			var dataList = JsonConvert.DeserializeObject<List<SnapCardData>>(json);
			if (dataList != null && dataList.Count > 0)
			{
				foreach (var d in dataList)
				{
					if (d != null)
					{
						_cards[d.cardId] = d;
					}
				}
			}
			else
			{
				// Backward-compatibility: old schema with CardRecord wrapper
				try
				{
					var legacy = JsonConvert.DeserializeObject<List<LegacyCardRecord>>(json) ?? new List<LegacyCardRecord>();
					foreach (var rec in legacy)
					{
						if (rec?.data != null)
						{
							rec.data.cardId = rec.cardId;
							if (rec.data.dateCreated == default) rec.data.dateCreated = rec.createdAt;
							rec.data.dateUpdated = rec.updatedAt == default ? DateTime.Now : rec.updatedAt;
							_cards[rec.cardId] = rec.data;
						}
					}
				}
				catch
				{
					// ignore if still fails
				}
			}

			// compute next id
			_nextCardId = _cards.Count == 0 ? 1 : _cards.Keys.Max() + 1;
			// Debug.Log($"CardRegistry loaded {_cards.Count} entries from {_savePath}");
		}
		catch (Exception e)
		{
			Debug.LogError($"CardRegistry load failed: {e.Message}");
			_cards.Clear();
			_nextCardId = 1;
		}
	}

	[Serializable]
	private class LegacyCardRecord
	{
		public int cardId;
		public SnapCardData data;
		public DateTime createdAt;
		public DateTime updatedAt;
	}
}
