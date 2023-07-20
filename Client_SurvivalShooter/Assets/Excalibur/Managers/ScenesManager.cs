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
        public const int BORDER_COUNT = 2;

        private readonly Dictionary<string, Scene> r_Scenes = new Dictionary<string, Scene>();

        private readonly Vector3[] r_Border = new Vector3[BORDER_COUNT];

        public Vector3[] Border => r_Border;

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
            if (sceneName == SceneName.Game.ToString())
            {
                GameObject[] rootObjects = r_Scenes[sceneName].GetRootGameObjects();
                int borderCtn = 0;
                for (int i = 0; i < rootObjects.Length; ++i)
                {
                    if (rootObjects[i].tag == "Border")
                    {
                        r_Border[borderCtn++] = rootObjects[i].transform.position;
                        if (borderCtn == BORDER_COUNT) { break; }
                    }
                }
                if (r_Border[0].x > r_Border[1].x)
                {
                    Vector3 tmp = r_Border[0];
                    r_Border[0] = r_Border[1];
                    r_Border[1] = tmp;
                }
            }
        }

        public GameObject[] GetSceneRootObjects(SceneName sceneName)
        {
            return r_Scenes[sceneName.ToString()].GetRootGameObjects();
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