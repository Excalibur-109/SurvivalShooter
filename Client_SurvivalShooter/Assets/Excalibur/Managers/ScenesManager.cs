using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Codice.Client.BaseCommands.Merge;

namespace Excalibur
{
    public enum SceneName { Main, Game, UI }

    public class ScenesManager : Singleton<ScenesManager>
    {
        private readonly Dictionary<string, Scene> r_Scenes = new Dictionary<string, Scene>();

        public void LoadScene(string sceneName, Action onSceneLoaded)
        {
            if (r_Scenes.ContainsKey(sceneName))
            {
                return;
            }
            GameManager.Instance.StartRoutine(LoadScene_Internal(sceneName, onSceneLoaded));
        }

        IEnumerator LoadScene_Internal (string sceneName, Action onSceneLoaded)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            operation.allowSceneActivation = false;
            yield return operation.isDone;
            operation.allowSceneActivation = true;
            r_Scenes.Add(sceneName, SceneManager.GetSceneByName(sceneName));
            onSceneLoaded?.Invoke();
        }

        public void MoveObjectToScene(string sceneName, GameObject gameObject)
        {
            SceneManager.MoveGameObjectToScene(gameObject, r_Scenes[sceneName]);
        }
    }
}