using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excalibur
{
    public enum UIType { Form, View, Component }

    public sealed class UIManager : Singleton<UIManager>
    {
        private readonly Stack<UIUnit> _formStack = new Stack<UIUnit> ();

        private Transform _uiRoot;

        public Camera uiCamera => CameraManager.Instance.uiCamera;

        protected override void OnConstructed ()
        {
            CameraManager.Instance.CreateUICamera();
        }

        public void Open (string formName, EventParam eventParam = null)
        {

        }

        public void Close (string formName)
        {

        }
    }
}
