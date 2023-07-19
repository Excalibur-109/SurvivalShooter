using Excalibur;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excalibur
{
    public sealed class GameManager : MonoSingleton<GameManager>, IExecutableBehaviour
    {
        private readonly ExecutableBehaviourAssistant _executableBehaviourAssistant = new ExecutableBehaviourAssistant();
        public bool Executable { get => _executableBehaviourAssistant.Executable; set => _executableBehaviourAssistant.Executable = value; }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
        }

        protected override void Update()
        {
            Execute();
        }

        protected override void OnEnable()
        {
            Executable = true;
        }

        protected override void OnDisable()
        {
        }

        public void Execute()
        {
            if (Executable)
            {
                _executableBehaviourAssistant.Execute();
            }
        }

        public void AttachExecutableUnit(IExecutableBehaviour unit)
        {
            _executableBehaviourAssistant.Attach(unit);
        }

        public void DetachExecutableUnit(IExecutableBehaviour unit)
        {
            _executableBehaviourAssistant.Detach(unit);
        }

        public Coroutine StartRoutine(IEnumerator enumerator)
        {
            return StartCoroutine(enumerator);
        }
    }
}