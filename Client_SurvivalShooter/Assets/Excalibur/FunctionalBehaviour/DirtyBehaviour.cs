using UnityEngine;

namespace Excalibur
{
    public abstract class DirtyBehaviour : MonoBehaviour, IExecuteBehaviour
    {
        public bool Executable { get; set; }
        protected virtual void Awake () { Dirty (); }
        protected virtual void Start () { }
        protected virtual void Update () { }
        protected virtual void LateUpdate () { }
        protected virtual void OnEnable () { }
        protected virtual void OnDisable () { }
        protected virtual void OnDestroy () { }
        protected virtual void OnBecameVisible () { }
        protected virtual void OnBecameInvisible () { }
        protected virtual void Reset () { }
        protected virtual void OnValidate () { }
        protected virtual void OnApplicationFocus () { }
        protected virtual void OnApplicationPause () { }
        protected virtual void OnApplicationQuit () { }
        public virtual void Execute () { }
        protected virtual void Dirty () { }
        public void SetDirty () { Dirty (); }
    }
}