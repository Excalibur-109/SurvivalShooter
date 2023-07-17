using UnityEngine;

namespace Excalibur
{
    public abstract class Unit : IUnitEntity
    {
        private GameObject _gameObject;
        private Transform _transform;
        protected CaptureBehaviour captureBehaviour;

        public virtual string name => gameObject.name;
        public bool visible { get => gameObject.activeSelf; set => SetActive (value); }
        public bool Executable { get; set; }
        public GameObject gameObject => _gameObject;
        public Transform transform => _transform;
        public Vector3 position { get => transform.position; set => transform.position = value; }
        public Vector3 localPosition { get => transform.localPosition; set => transform.localPosition = value; }
        public Vector3 scale { get => transform.localScale; set => transform.localScale = value; }
        public Vector3 eulerAngle { get => transform.localEulerAngles; set => transform.localEulerAngles = value; }
        public Quaternion rotation { get => transform.rotation; set => transform.rotation = value; }

        /// <summary>
        /// 手动执行Attach来连接GameObject
        /// </summary>
        public void Attach(GameObject gameObject)
        {
            _gameObject = gameObject;
            _transform = gameObject.transform;
            OnAttached();
        }

        public void Detach()
        {
            OnDetached();
        }

        public virtual void Activate ()
        {
        }

        public virtual void Reactivate ()
        {
        }

        public virtual void Deactivate ()
        {
        }

        public virtual void Execute ()
        {
        }

        public virtual void Terminate ()
        {
        }

        public void Destory ()
        {
            Object.Destroy (gameObject);
        }

        public T AddComponent<T> () where T : Component
        {
            return gameObject.AddComponent<T> ();
        }

        public T GetComponent<T> () where T : Component
        {
            return transform.GetComponent<T> ();
        }

        public bool RemoveComponent<T> () where T : Component
        {
            if (GetComponent<T> () != null)
            {
                Object.Destroy (GetComponent<T> ());
                return true;
            }
            return false;
        }

        public void SetActive (bool active)
        {
            if (gameObject.activeSelf != active)
            {
                gameObject.SetActive (active);
            }
        }

        public void SetParent (Transform parent)
        {
            transform.SetParent (parent);
        }

        protected virtual void OnAttached ()
        {
            captureBehaviour = transform.GetComponent<CaptureBehaviour>();
        }

        protected virtual void OnDetached()
        {
        }
    }
}