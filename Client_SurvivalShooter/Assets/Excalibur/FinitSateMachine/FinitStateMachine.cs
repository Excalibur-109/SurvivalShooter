using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excalibur
{
    public enum FinitState { Idle, Walk, Run, Chase, Attack, Hurt, Dead }

    public interface IFinitState 
    {
        void OnEnter();
        void OnExecute();
        void OnExit();
    }

    public class FinitStateMachine
    {
        private Unit _unit;
        private IFinitState _currentState;
        private readonly Dictionary<FinitState, IFinitState> r_States = new Dictionary<FinitState, IFinitState>();
        private readonly Dictionary<FinitState, string> r_Anims = new Dictionary<FinitState, string>();
        private Animator _animater;

        public Unit unit => _unit;

        public void Attach(Unit unit)
        {
            _unit = unit;
            _animater = _unit.GetComponent<Animator>();
        }

        public void LinkState(FinitState stateType, IFinitState state, string animName)
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
        }
    }
}
