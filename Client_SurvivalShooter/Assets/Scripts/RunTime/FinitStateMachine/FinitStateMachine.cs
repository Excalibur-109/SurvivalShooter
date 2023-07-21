using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excalibur;
using System;
using TMPro.EditorUtilities;
using UnityEngine.UI;

public enum FinitState { None, Idle, Walk, Run, Chase, Attack, Hurt, Dead }
public enum AnimName { idle, run, fire, Nothing }

public abstract class BaseFiniteState 
{
    private readonly string _anim;
    protected FinitStateMachine _fsm;
    public Action onEnter;
    public Action onExit;

    public abstract AnimName animName { get;}
    public string anim => _anim;

    public BaseFiniteState()
    {
        _anim = animName.ToString();
    }

    public void SetFSM(FinitStateMachine fsm) => _fsm = fsm;
    public virtual void SetComponent(IComponent componentData) { }

    public abstract void OnEnter();
    public abstract void OnExecute();
    public abstract void OnExit();
}

public class FinitStateMachine : IExecutableBehaviour
{
    private Unit _unit;
    private FinitState _currentState = FinitState.None;
    private readonly Dictionary<FinitState, BaseFiniteState> r_States = new Dictionary<FinitState, BaseFiniteState>();
    private Animator _animator;

    public Unit unit => _unit;

    public bool Executable { get; set; }

    public void Start()
    {
        Executable = true;
        CharacterManager.Instance.fsmAssistant.Attach(this);
    }

    public void Attach(Unit unit)
    {
        _unit = unit;
        _animator = _unit.GetComponent<Animator>();
    }

    public void LinkState(FinitState stateType, BaseFiniteState state)
    {
        r_States.Add(stateType, state);
        state.SetFSM(this);
    }

    public void TransitionState(FinitState state)
    {
        if (_currentState > FinitState.None)
        {
            r_States[_currentState].OnExit();
        }
        if (_currentState != state)
        {
            _currentState = state;

            r_States[_currentState].OnEnter();
            if (r_States[_currentState].animName != AnimName.Nothing)
            {
                _animator.Play(r_States[_currentState].anim);
            }
        }
    }

    public void Dispose()
    {
        r_States.Clear();
        _unit = null;
        _animator = null;
        Executable = false;
        CharacterManager.Instance.fsmAssistant.Detach(this);
    }

    public void Execute()
    {
        if (Executable)
        {
            r_States[_currentState].OnExecute();
        }
    }
}
