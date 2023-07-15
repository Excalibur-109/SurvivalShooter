using System.Threading;
using UnityEngine;

namespace Excalibur
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>, new ()
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T> ();
                    if (_instance == null)
                    {
                        _instance = new GameObject (typeof (T).ToString ()).AddComponent<T> ();
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake ()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else
            {
                Destroy (this.gameObject);
            }

            DontDestroyOnLoad (this);
        }

        protected virtual void Start ()
        {

        }

        protected virtual void Update ()
        {

        }

        protected virtual void OnEnable ()
        {

        }

        protected virtual void OnDisable ()
        {

        }
    }
}
