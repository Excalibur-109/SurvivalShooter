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
        for (SceneName name = SceneName.Game; name <= SceneName.UI; ++name)
        {
            yield return StartCoroutine(ScenesManager.Instance.LoadScene(name.ToString()));
        }
        ScenesManager.Instance.MoveObjectToScene(SceneName.UI.ToString(), CameraManager.Instance.uiCamera.gameObject);
        StartGame();
    }

    public void StartGame()
    {
        CharacterManager.Instance.CreateCharacter(ConstParam.PLAYER_ID);
    }
}
