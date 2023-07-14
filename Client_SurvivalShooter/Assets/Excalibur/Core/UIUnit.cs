using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excalibur
{
    public abstract class UIUnit : Unit
    {
        private RectTransform _rectTransform;
        public Vector2 anchoredPosition { get => rectTransform.anchoredPosition; set => rectTransform.anchoredPosition = value; }

        public RectTransform rectTransform => _rectTransform;

        protected override void OnAttached ()
        {
            _rectTransform = (RectTransform)transform;
        }

        public override void Activate ()
        {
            base.Activate ();
        }

        public override void Deactivate ()
        {
            base.Deactivate ();
        }

        protected void Broadcast (string eventName)
        {
            EventManager.Instance.Broadcast (eventName);
        }
    }
}
