using System;
using System.Collections.Generic;
using UnityEngine;

public class SnapContext 
{
    public Dictionary<string, object> variables = new();
    
    int AbilityUsageCount = 0;

    public void Clear()
    {
        variables.Clear();
    }

    public void AbilityUsed()
    {
        AbilityUsageCount++;
    }

    public int GetAbilityUsageCount() {
        return AbilityUsageCount;
    }

    public void AddVariable<T>(string key, T value)
    {
        variables[key] = value;
    }
    
    public T GetVariable<T>(string key)
    {
        if (variables.TryGetValue(key, out var value))
        {
            Debug.Log("type of value: " + value.GetType());
            Debug.Log("type of T: " + typeof(T));
            try
            {
                return (T)value;
            }
            catch (InvalidCastException e)
            {
                return default(T);
            }
        }
        return default(T);
    }
    

}