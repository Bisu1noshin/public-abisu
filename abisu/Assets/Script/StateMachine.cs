using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 各State毎のdelagateを登録しておくクラス
public class StateMapping
{
    public Action onEnter;
    public Action onExit;
    public Action<float> onUpdate;
}

public class Transition<TState, TTrigger>
{
    public TState To { get; set; }
    public TTrigger Trigger { get; set; }
}

public class StateMachine<TState, TTrigger>
    where TState : struct, IConvertible, IComparable
    where TTrigger : struct, IConvertible, IComparable
{
    private TState _stateType;
    private StateMapping _stateMapping;

    private Dictionary<object, StateMapping> _stateMappings = new Dictionary<object, StateMapping>();
    private Dictionary<TState, List<Transition<TState, TTrigger>>> _transitionLists = new Dictionary<TState, List<Transition<TState, TTrigger>>>();

    public StateMachine(TState initialState)
    {
        // StateからStateMappingを作成
        var enumValues = Enum.GetValues(typeof(TState));
        for (int i = 0; i < enumValues.Length; i++)
        {
            var mapping = new StateMapping();
            _stateMappings.Add(enumValues.GetValue(i), mapping);
        }

        // 最初のStateに遷移
        ChangeState(initialState);
    }

    /// <summary>
    /// トリガーを実行する
    /// </summary>
    public void ExecuteTrigger(TTrigger trigger)
    {
        var transitions = _transitionLists[_stateType];
        foreach (var transition in transitions)
        {
            if (transition.Trigger.Equals(trigger))
            {
                ChangeState(transition.To);
                break;
            }
        }
    }

    /// <summary>
    /// 遷移情報を登録する
    /// </summary>
    public void AddTransition(TState from, TState to, TTrigger trigger)
    {
        if (!_transitionLists.ContainsKey(from))
        {
            _transitionLists.Add(from, new List<Transition<TState, TTrigger>>());
        }
        var transitions = _transitionLists[from];
        var transition = transitions.FirstOrDefault(x => x.To.Equals(to));
        if (transition == null)
        {
            // 新規登録
            transitions.Add(new Transition<TState, TTrigger> { To = to, Trigger = trigger });
        }
        else
        {
            // 更新
            transition.To = to;
            transition.Trigger = trigger;
        }
    }

    /// <summary>
    /// Stateを初期化する
    /// </summary>
    public void SetupState(TState state, Action onEnter, Action onExit, Action<float> onUpdate)
    {
        var stateMapping = _stateMappings[state];
        stateMapping.onEnter = onEnter;
        stateMapping.onExit = onExit;
        stateMapping.onUpdate = onUpdate;
    }

    /// <summary>
    /// 更新する
    /// </summary>
    public void Update(float deltaTime)
    {
        if (_stateMapping != null && _stateMapping.onUpdate != null)
        {
            _stateMapping.onUpdate(deltaTime);
        }
    }

    /// <summary>
    /// 現在のステータスを取得する
    /// </summary>
    public TState GetState()
    {
        return _stateType;
    }

    /// <summary>
    /// Stateを直接変更する
    /// </summary>
    private void ChangeState(TState to)
    {
        // OnExit
        if (_stateMapping != null && _stateMapping.onExit != null)
        {
            _stateMapping.onExit();
        }

        // OnEnter
        _stateType = to;
        _stateMapping = _stateMappings[to];
        if (_stateMapping.onEnter != null)
        {
            _stateMapping.onEnter();
        }
    }
}
