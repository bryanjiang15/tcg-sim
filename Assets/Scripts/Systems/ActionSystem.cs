using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ActionSystem : Singleton<ActionSystem>
{
    private List<GameAction> reactions = null;
    public bool IsPerforming { get; private set; } = false;
    private static Dictionary<Type, List<Action<GameAction>>> preSubs = new();
    private static Dictionary<Type, List<Action<GameAction>>> postSubs = new();

    private static Dictionary<Type, Func<GameAction, IEnumerator>> performers = new();

    public void Perform(GameAction action, Action OnPerformFinished = null)
    {
        if (IsPerforming) return;
        IsPerforming = true;
        StartCoroutine(Flow(action, () =>
        {
            IsPerforming = false;
            OnPerformFinished?.Invoke();
        }));
    }

    public void AddReaction(GameAction gameAction){
        if (gameAction == null) return;
        reactions?.Add(gameAction);
    }

    private IEnumerator Flow(GameAction action, Action OnFlowFinished = null)
    {
        reactions = action.PreReactions;
        PerformSubscribers(action, preSubs);
        yield return PerformReaction();

        reactions = action.PerformReactions;
        yield return PerformPerformers(action);
        yield return PerformReaction();

        reactions = action.PostReactions;
        PerformSubscribers(action, postSubs);
        yield return PerformReaction();

        OnFlowFinished?.Invoke();
    }

    private IEnumerator PerformPerformers(GameAction action)
    {
        Type type = action.GetType();
        if (performers.ContainsKey(type))
        {
            yield return performers[type](action);
        }
    }

    private void PerformSubscribers(GameAction action, Dictionary<Type, List<Action<GameAction>>> subs) {
        Type type = action.GetType();
        foreach (var key in subs.Keys) {
            if (key.IsAssignableFrom(type)) { // Check if the key is a parent type of the action's type
                foreach (var sub in subs[key]) {
                    sub(action);
                }
            }
        }
    }

    private IEnumerator PerformReaction()
    {
        var reactionsCopy = new List<GameAction>(reactions);
        foreach (var reaction in reactionsCopy )
        {
            yield return Flow(reaction);
        }
    }
    public static void AttachPerformer<T>(Func<T, IEnumerator> performer) where T : GameAction
    {
        Type type = typeof(T);
        IEnumerator wrappedPerformer(GameAction action) => performer((T)action);
        if (performers.ContainsKey(type)) performers[type] = wrappedPerformer;
        else performers.Add(type, wrappedPerformer);
    }
    public static void DetachPerformer<T>() where T : GameAction
    {
        Type type = typeof(T);
        if (performers.ContainsKey(type)) performers.Remove(type);
    }
    public static void SubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        Type type = typeof(T);
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        void wrappedReaction(GameAction action) => reaction((T)action);
        if (subs.ContainsKey(typeof(T)))
        {
            subs[type].Add(wrappedReaction);
        }
        else
        {
            subs.Add(type, new List<Action<GameAction>> { wrappedReaction });
        }
    }
    public static void UnsubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        Type type = typeof(T);
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        if (subs.ContainsKey(type))
        {
            void wrappedReaction(GameAction action) => reaction((T)action);
            subs[type].Remove(wrappedReaction);
        }
    }
    
    public static void SubscribeReaction(Type type, Action<GameAction> reaction, ReactionTiming timing)
    {
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        if (subs.ContainsKey(type))
        {
            subs[type].Add(reaction);
        }
        else
        {
            subs.Add(type, new List<Action<GameAction>> { reaction });
        }
    }

    public static void UnsubscribeReaction(Type type, Action<GameAction> reaction, ReactionTiming timing)
    {
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        if (subs.ContainsKey(type))
        {
            subs[type].Remove(reaction);
        }
    }
}