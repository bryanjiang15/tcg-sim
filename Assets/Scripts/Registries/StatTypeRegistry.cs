using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Registry for storing all StatType entries keyed by StatTypeId.
/// Provides JSON persistence and simple CRUD/query APIs.
/// </summary>
public class StatTypeRegistry : Singleton<StatTypeRegistry>
{
	[SerializeField] private string saveFileName = "stat_type_registry.json";
	private readonly Dictionary<int, StatType> _statTypes = new Dictionary<int, StatType>();
	private string _savePath;

	private int _nextStatTypeId = 1;

	protected override void Awake()
	{
		base.Awake();
		_savePath = Path.Combine(Application.persistentDataPath, saveFileName);
		Load();
	}

	public int Count => _statTypes.Count;
	public IEnumerable<int> AllIds => _statTypes.Keys;

	public bool HasStatType(int statTypeId) => _statTypes.ContainsKey(statTypeId);

	public StatType GetStatTypeById(int statTypeId)
	{
		return _statTypes.TryGetValue(statTypeId, out var data) ? data : null;
	}

	public StatType GetStatTypeByName(string name)
	{
		if (string.IsNullOrWhiteSpace(name)) return null;
		return _statTypes.Values.FirstOrDefault(s => 
			string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
	}

	public List<StatType> GetAllStatType()
	{
		return _statTypes.Values.ToList();
	}

	public bool TryGetStatType(int statTypeId, out StatType data)
	{
		if (_statTypes.TryGetValue(statTypeId, out var found))
		{
			data = found;
			return true;
		}
		data = null;
		return false;
	}

	public bool InsertStatType(StatType data)
	{
		if (data == null)
		{
			Debug.LogError("Cannot add null StatType to StatTypeRegistry");
			return false;
		}

		// Assign a new id if missing or already taken
		if (data.StatTypeId <= 0 || _statTypes.ContainsKey(data.StatTypeId))
		{
			data.StatTypeId = _nextStatTypeId;
		}

		_statTypes[data.StatTypeId] = data;

		// Bump next id if needed
		_nextStatTypeId = Math.Max(_nextStatTypeId, data.StatTypeId + 1);

		Save();
		return true;
	}

	public bool UpdateStatType(int statTypeId, StatType data)
	{
		if (data == null)
		{
			Debug.LogError("Cannot update with null StatType");
			return false;
		}

		if (!_statTypes.TryGetValue(statTypeId, out var existing))
		{
			Debug.LogError($"StatType with ID {statTypeId} not found. Use Insert instead.");
			return false;
		}

		data.StatTypeId = statTypeId;
		_statTypes[statTypeId] = data;

		Save();
		return true;
	}

	public bool DeleteStatType(int statTypeId)
	{
		if (_statTypes.Remove(statTypeId))
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
		return _statTypes.Values
			.Where(s => !string.IsNullOrEmpty(s.Name) && s.Name.IndexOf(namePart, StringComparison.OrdinalIgnoreCase) >= 0)
			.Select(s => s.StatTypeId)
			.ToList();
	}

	public void Save()
	{
		try
		{
			var payload = _statTypes.Values.ToList();
			var json = JsonConvert.SerializeObject(payload, Formatting.Indented);
			File.WriteAllText(_savePath, json);
			// Debug.Log($"StateTypeRegistry saved {_statTypes.Count} entries to {_savePath}");
		}
		catch (Exception e)
		{
			Debug.LogError($"StatTypeRegistry save failed: {e.Message}");
		}
	}

	public void Load()
	{
		try
		{
			if (!File.Exists(_savePath))
			{
				_statTypes.Clear();
				_nextStatTypeId = 1;
				return;
			}
			var json = File.ReadAllText(_savePath);
			_statTypes.Clear();

			var dataList = JsonConvert.DeserializeObject<List<StatType>>(json);
			if (dataList != null && dataList.Count > 0)
			{
				foreach (var s in dataList)
				{
					if (s != null)
					{
						_statTypes[s.StatTypeId] = s;
					}
				}
			}

			// compute next id
			_nextStatTypeId = _statTypes.Count == 0 ? 1 : _statTypes.Keys.Max() + 1;
			// Debug.Log($"StatTypeRegistry loaded {_statTypes.Count} entries from {_savePath}");
		}
		catch (Exception e)
		{
			Debug.LogError($"StatTypeRegistry load failed: {e.Message}");
			_statTypes.Clear();
			_nextStatTypeId = 1;
		}
	}
}
