using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Excalibur
{
    public enum Interact
    {
        Button,
        Tog,
        Drag,
    }

    [DisallowMultipleComponent]
    public class InteractableBehaviour : UIBehaviour
    {
        public class InteractedEvent : UnityEvent
        {

        }

        private InteractedEvent _onInteracted;
        public InteractedEvent onInteracted { get => _onInteracted; set => onInteracted = value; }
    }
}
