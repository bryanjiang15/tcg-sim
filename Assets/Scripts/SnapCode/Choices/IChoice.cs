using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using System;

// Non-generic interface for UI components that don't need to know the specific type
public interface IChoice
{
    string Prompt { get; }
    bool IsOptional { get; }
    void ConfirmChoice();
}

interface IChoice<T> : IChoice
{
    IEnumerator MakeChoice(Action<IEnumerable<T>> onChoiceMade = null);
    bool IsValidChoice(T choice);
}