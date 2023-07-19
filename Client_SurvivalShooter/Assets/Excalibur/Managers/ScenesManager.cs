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

        public IEnumerator LoadScene (string sceneName, Action onSceneLoaded = default)
        {
            if (r_Scenes.ContainsKey(sceneName))
            {
                yield break;
            }
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

        public void MoveObjectToGameScene(GameObject gameObject)
        {
            MoveObjectToScene(SceneName.Game.ToString(), gameObject);
        }

        public void MoveObjectToUIScene(GameObject gameObject)
        {
            MoveObjectToScene(SceneName.UI.ToString(), gameObject);
        }

        public GameObject InstantiateObjectToScene(string sceneName, GameObject gameObject, Action onInstantiate = default)
        {
            GameObject go = MonoExtension.InitializeObject(gameObject);
            MoveObjectToScene(sceneName, go);
            return go;
        }

        public GameObject InstantiateObjectToGameScene(GameObject gameObject, Action onInstantiate = default)
        {
            return InstantiateObjectToScene(SceneName.Game.ToString(), gameObject, onInstantiate);
        }

        public GameObject InstantiateObjectToUIScene(GameObject gameObject, Action onInstantiate = default)
        {
            return InstantiateObjectToScene(SceneName.UI.ToString(), gameObject, onInstantiate);
        }
    }
}