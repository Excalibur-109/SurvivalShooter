using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excalibur;

public class GameLauncher : MonoSingleton<GameLauncher>
{
    protected override void Start()
    {
        StartCoroutine(InitializeGame_Routine());
    }

    IEnumerator InitializeGame_Routine()
    {
        DeserializeConfigurations.DeserilaizeConfigs();
        CameraManager.Instance.CreateMainCamera();
        CameraManager.Instance.CreateUICamera();
        CameraController.Instance.SetCamera(CameraManager.Instance.mainCamera.transform);
        for (SceneName name = SceneName.Game; name <= SceneName.UI; ++name)
        {
            yield return StartCoroutine(ScenesManager.Instance.LoadScene(name.ToString()));
        }
        ScenesManager.Instance.MoveObjectToScene(SceneName.UI.ToString(), CameraManager.Instance.uiCamera.gameObject);
        CameraController.Instance.SetBorder(ScenesManager.Instance.Border);
        StartGame();
    }

    public void StartGame()
    {
        CharacterManager.Instance.CreateCharacter(ConstParam.PLAYER_ID);
    }
}
