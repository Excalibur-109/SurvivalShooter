using OfficeOpenXml.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using static UnityEngine.EventSystems.PointerEventData;

namespace Excalibur
{
    public sealed class InputManager : Singleton<InputManager>, IExecutableBehaviour
    {
        private readonly ExecutableBehaviourAssistant _inputExecuteAssistant = new ExecutableBehaviourAssistant ();

        public bool Executable { get; set; }

        protected override void OnConstructed ()
        {
            GameManager.Instance.AttachExecutableUnit (this);
            Executable = true;
        }

        public void Execute ()
        {
            if (Executable)
            {
                _inputExecuteAssistant.Execute();
            }
        }

        public void AttachInputResponser (InputResponser responser)
        {
            _inputExecuteAssistant.Attach(responser);
        }


        public void DetachInputResponser (InputResponser responser)
        {
            _inputExecuteAssistant.Detach(responser);
        }
    }

    public enum MouseButton
    {
        None = -1,
        Left = 0,
        Middle = 1,
        Right = 2,
    }

    public delegate void InputAction ();

    /// <summary> 按键输入响应类 /// </summary>
    public class InputResponser : IExecutableBehaviour
    {
        #region Fields

        private readonly List<KeyCode> _inputKeys;
        private readonly List<KeyCode> _inputKeysMap; // 按键映射
        private readonly Dictionary<KeyCode, InputAction> _inputKeyDownActions;
        private readonly Dictionary<KeyCode, InputAction> _inputKeyActions;
        private readonly Dictionary<KeyCode, InputAction> _inputKeyUpActions;

        private readonly List<MouseButton> _inputButtons;
        private readonly Dictionary<MouseButton, InputAction> _inputButtonDownActions;
        private readonly Dictionary<MouseButton, InputAction> _inputButtonActions;
        private readonly Dictionary<MouseButton, InputAction> _inputButtonUpActions;

        private readonly InputAction
            _keyDownAction, _keyAction, _keyUpAction,
            _buttonDownAction, _buttonAction, _buttonUpAction;

        private int i;

        #endregion

        public bool Executable { get; set; }
        public List<KeyCode> OriginalKeys => _inputKeys;
        public List<KeyCode> ResponseKeys => _inputKeysMap;
        public List<MouseButton> ResponseButtons => _inputButtons;

        public InputResponser ()
        {
            _inputKeys = new List<KeyCode> ();
            _inputKeysMap = new List<KeyCode> ();
            _inputKeyDownActions = new Dictionary<KeyCode, InputAction> ();
            _inputKeyActions = new Dictionary<KeyCode, InputAction> ();
            _inputKeyUpActions = new Dictionary<KeyCode, InputAction> ();

            _inputButtons = new List<MouseButton> ();
            _inputButtonDownActions = new Dictionary<MouseButton, InputAction> ();
            _inputButtonActions = new Dictionary<MouseButton, InputAction> ();
            _inputButtonUpActions = new Dictionary<MouseButton, InputAction> ();
        }

        #region Attach Key Actions

        /// <summary> 链接键盘按键 /// </summary>
        public void AttachInputKeys (List<KeyCode> inputKeys)
        {
            if (_inputKeys.Count > 0) { return; }
            for (int i = 0; i < inputKeys.Count; ++i)
            {
                _inputKeys.Add (inputKeys[i]);
                _inputKeysMap.Add (inputKeys[i]);
                _inputKeyDownActions.Add (inputKeys[i], _keyDownAction);
                _inputKeyActions.Add (inputKeys[i], _keyAction);
                _inputKeyUpActions.Add (inputKeys[i], _keyUpAction);
            }

            Executable = true;
        }

        public void AttachKeyDownAction (KeyCode keyCode, InputAction inputDownAction)
        {
            _inputKeyDownActions[keyCode] += inputDownAction;
        }

        public void AttachKeyAction (KeyCode keyCode, InputAction inputAction)
        {
            _inputKeyActions[keyCode] += inputAction;
        }

        public void AttachKeyUpAction (KeyCode keyCode, InputAction inputUpAction)
        {
            _inputKeyUpActions[keyCode] += inputUpAction;
        }

        public void AttachKeyDownAction (Dictionary<KeyCode, InputAction> inputKeyDownActions)
        {
            foreach (KeyCode keyCode in inputKeyDownActions.Keys)
            {
                _inputKeyDownActions[keyCode] += inputKeyDownActions[keyCode];
            }
        }

        public void AttachKeyAction (Dictionary<KeyCode, InputAction> inputKeyActions)
        {
            foreach (KeyCode keyCode in inputKeyActions.Keys)
            {
                _inputKeyActions[keyCode] += inputKeyActions[keyCode];
            }
        }

        public void AttachKeyUpAction (Dictionary<KeyCode, InputAction> inputKeyUpActions)
        {
            foreach (KeyCode keyCode in inputKeyUpActions.Keys)
            {
                _inputKeyUpActions[keyCode] += inputKeyUpActions[keyCode];
            }
        }

        #endregion

        #region Attach MouseButton Actions

        /// <summary> 链接鼠标按键 /// </summary>
        public void AttachInputButton (List<MouseButton> inputButtons)
        {
            if (_inputButtons.Count > 0) { return; }
            for (int i = 0; i < inputButtons.Count; ++i)
            {
                _inputButtons.Add (inputButtons[i]);
                _inputButtonDownActions.Add (inputButtons[i], _buttonDownAction);
                _inputButtonActions.Add (inputButtons[i], _buttonAction);
                _inputButtonUpActions.Add (inputButtons[i], _buttonUpAction);
            }

            Executable = true;
        }

        public void AttachButtonDownAction (MouseButton mouseButton, InputAction inputDownAction)
        {
            _inputButtonDownActions[mouseButton] += inputDownAction;
        }

        public void AttachButtonAction (MouseButton mouseButton, InputAction inputAction)
        {
            _inputButtonActions[mouseButton] += inputAction;
        }

        public void AttachButtonUpAction (MouseButton mouseButton, InputAction inputUpAction)
        {
            _inputButtonUpActions[mouseButton] += inputUpAction;
        }

        public void AttachButtonDownAction (Dictionary<MouseButton, InputAction> inputButtonDownActions)
        {
            foreach (MouseButton button in inputButtonDownActions.Keys)
            {
                _inputButtonDownActions[button] += inputButtonDownActions[button];
            }
        }

        public void AttachButtonAction (Dictionary<MouseButton, InputAction> inputButtonActions)
        {
            foreach (MouseButton button in inputButtonActions.Keys)
            {
                _inputButtonActions[button] += inputButtonActions[button];
            }
        }

        public void AttachButtonUpAction (Dictionary<MouseButton, InputAction> inputButtonUpActions)
        {
            foreach (MouseButton button in inputButtonUpActions.Keys)
            {
                _inputButtonUpActions[button] += inputButtonUpActions[button];
            }
        }

        #endregion

        #region Response
        /// <summary> 键盘按下 /// </summary>
        private void _ResponseKeyDown (KeyCode keyCode)
        {
            _inputKeyDownActions[_GetOriginalKey (keyCode)]?.Invoke ();
        }

        /// <summary> 键盘按住 /// </summary>
        private void _ResponseKey (KeyCode keyCode)
        {
            _inputKeyActions[_GetOriginalKey (keyCode)]?.Invoke ();
        }

        /// <summary> 键盘抬起 /// </summary>
        private void _ResponseKeyUp (KeyCode keyCode)
        {
            _inputKeyUpActions[_GetOriginalKey (keyCode)]?.Invoke ();
        }

        /// <summary> 鼠标按下 /// </summary>
        private void _ResponseMouseButtonDown (MouseButton mouseButton) 
        {
            _inputButtonDownActions[mouseButton]?.Invoke ();
        }

        /// <summary> 鼠标按住 /// </summary>
        private void _ResponseMouseButton (MouseButton mouseButton)
        {
            _inputButtonActions[mouseButton]?.Invoke ();
        }

        /// <summary> 鼠标抬起 /// </summary>
        private void _ResponseMouseButtonUp (MouseButton mouseButton)
        {
            _inputButtonUpActions[mouseButton]?.Invoke ();
        }

        #endregion

        private KeyCode _GetOriginalKey (KeyCode keyCode)
        {
            for (int i = 0; i < _inputKeysMap.Count; ++i)
            {
                if (_inputKeysMap[i] == keyCode)
                {
                    return _inputKeys[i];
                }
            }
            return KeyCode.None;
        }

        public void Activate ()
        {
            InputManager.Instance.AttachInputResponser (this);
        }

        public void Deactivate ()
        {
            InputManager.Instance.DetachInputResponser (this);
        }

        public void Execute ()
        {
            if (!Executable) { return; }

            for (i = 0; i < ResponseButtons.Count; ++i)
            {
                if (Input.GetMouseButtonDown ( (int)ResponseButtons[i]))
                {
                    _ResponseMouseButtonDown (ResponseButtons[i]);
                }

                if (Input.GetMouseButton ( (int)ResponseButtons[i]))
                {
                    _ResponseMouseButton (ResponseButtons[i]);
                }

                if (Input.GetMouseButtonUp ( (int)ResponseButtons[i]))
                {
                    _ResponseMouseButtonUp (ResponseButtons[i]);
                }
            }

            for (i = 0; i <  ResponseKeys.Count; ++i)
            {
                if (Input.GetKeyDown (ResponseKeys[i]))
                {
                    _ResponseKeyDown (ResponseKeys[i]);
                }

                if (Input.GetKey (ResponseKeys[i]))
                {
                    _ResponseKey (ResponseKeys[i]);
                }

                if (Input.GetKeyUp (ResponseKeys[i]))
                {
                    _ResponseKeyUp (ResponseKeys[i]);
                }
            }
        }
    }
}
