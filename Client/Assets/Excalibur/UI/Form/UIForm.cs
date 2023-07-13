using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excalibur
{
    public abstract class UIForm : UIUnit
    {
        private int id;
        protected string formName;
        protected EventHandler handler;
        /// <summary> ¿ªÆô´«²Î /// </summary>
        protected EventParam openParam;
        public override string name => formName;

        protected Stack<UIView> viewStack = new Stack<UIView> ();

        //public UIForm (string formName, GameObject gameObject) : base (gameObject)
        //{
        //    this.formName = formName;
        //}

        public void SetParams (int id, EventParam openParam = null)
        {
            this.id = id;
            this.openParam = openParam;
        }

        public override void Activate ()
        {
            base.Activate ();
            handler += InternalHandleEvent;
            EventManager.Instance.StartListen (name, handler);
            OnOpen ();
        }

        public override void Reactivate ()
        {
            base.Reactivate ();
            Refresh ();
        }

        public override void Deactivate ()
        {
            base.Deactivate ();
            handler -= InternalHandleEvent;
            EventManager.Instance.StopListen (name);
            OnClose ();
        }

        protected virtual void OnOpen () { }
        protected virtual void Refresh () { }
        protected virtual void OnClose () { }

        private void InternalHandleEvent (params object[] @param)
        {
            HandleEvent (@param.Length > 0 ? @param[0] as EventParam : null);
        }

        protected virtual void HandleEvent (EventParam eventParam)
        {

        }
    }
}
