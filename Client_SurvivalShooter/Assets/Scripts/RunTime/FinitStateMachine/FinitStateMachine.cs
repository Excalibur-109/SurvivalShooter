using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excalibur;
using System;
using TMPro.EditorUtilities;

public enum FinitState { Idle, Walk, Run, Chase, Attack, Hurt, Dead }

public abstract class BaseFiniteState 
{
    protected FinitStateMachine fsm;
    public string anim;
    public Action onEnter;
    public Action onExit;

    public BaseFiniteState(FinitStateMachine fsm) { this.fsm = fsm; }

    public abstract void OnEnter();
    public abstract void OnExecute();
    public abstract void OnExit();
}

public class FinitStateMachine
{
    private Unit _unit;
    private BaseFiniteState _currentState;
    private readonly Dictionary<FinitState, BaseFiniteState> r_States = new Dictionary<FinitState, BaseFiniteState>();
    private Animator _animater;

    public Unit unit => _unit;

    public void Attach(Unit unit)
    {
        _unit = unit;
        _animater = _unit.GetComponent<Animator>();
    }

    public void LinkState(FinitState stateType, BaseFiniteState state, string animName)
    {
        r_States.Add(stateType, state);
    }

    public void TransitionState(FinitState state)
    {
        if (_currentState != null)
        {
            _currentState.OnExit();
        }

        _currentState = r_States[state];
        _currentState.OnEnter();
        _animater.Play(_currentState.anim);
    }
}