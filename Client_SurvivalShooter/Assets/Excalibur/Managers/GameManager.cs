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
            StartRoutine(InitializeGame_Routine());
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

        IEnumerator InitializeGame_Routine()
        {
            CameraManager.Instance.CreateMainCamera();
            CameraManager.Instance.CreateUICamera();
            for (SceneName name = SceneName.Game; name <= SceneName.UI; ++name)
            {
                yield return StartRoutine(ScenesManager.Instance.LoadScene(name.ToString()));
            }
            ScenesManager.Instance.MoveObjectToScene(SceneName.UI.ToString(), CameraManager.Instance.uiCamera.gameObject);
            StartGame();
        }

        public void StartGame()
        {
            AssetsManager.Instance.LoadAsset<GameObject>("Player", go => ScenesManager.Instance.InstantiateObjectToGameScene(go));
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