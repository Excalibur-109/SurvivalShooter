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
        ScenesManager.Instance.GetBorder();
        ScenesManager.Instance.MoveObjectToScene(SceneName.UI.ToString(), CameraManager.Instance.uiCamera.gameObject);
        Vector3[] border = ScenesManager.Instance.Border;
        CameraController.Instance.SetBorder(border);
        CameraController.Instance.UpdatePosition((border[0] + border[1]) / 2f);
        StartGame();
    }

    public void StartGame()
    {
        foreach (int id in SpawnCfg.Config.Keys)
        {
            SpawnManager.Instance.SpawnUnit(id);
        }

        CameraController.Instance.MoveTo(CharacterManager.Instance.Player.position);
        CharacterManager.Instance.Executable = true;
    }
}
