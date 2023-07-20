using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excalibur;
using System;

public enum CharacterType { Player = 1, AI = 2 }

public enum AIFlag { Green = 1, Red = 2 }

public class Character : Unit
{
    private CharacterType _characterType;
    private AIFlag _aiFlag;
    private CharacterData _characterData;
    private InputResponser _playerInput;
    private FinitStateMachine _fsm;

    public CharacterType characterType => _characterType;
    public AIFlag aiFlag => _aiFlag;
    private CharacterData characterData => _characterData;

    public void InitData(int id)
    {
        CharacterCfg.Character cfg = CharacterCfg.TryGetValue(id);
        _characterType = (CharacterType)cfg.type;
        _aiFlag = (AIFlag)cfg.flag;
        _characterData = new CharacterData();
        _fsm = new FinitStateMachine();
        _fsm.Attach(this);
        if (characterType == CharacterType.Player)
        {
            _InitInput();
            CameraController.Instance.SetTarget(transform);
            CameraController.Instance.MoveTo(transform.position);
        }
        else
        {
        }
    }

    private void _InitInput()
    {
        List<PlayerInputCfg.PlayerInput> inputList = new List<PlayerInputCfg.PlayerInput>();
        List<KeyCode> keys = new List<KeyCode>();
        List<MouseButton> mouseButtons = new List<MouseButton>();
        Dictionary<int, PlayerInputCfg.PlayerInput> cfgs = PlayerInputCfg.Config;
        int group = (int)InputGroup.Player;
        foreach (var item in cfgs.Values)
        {
            if (item.group == group)
            {
                inputList.Add(item);
                if (item.inputType == (int)InputType.KeyCode)
                {
                    keys.Add((KeyCode)item.key);
                }
                else
                {
                    mouseButtons.Add((MouseButton)item.key);
                }
            }
        }
        _playerInput = new InputResponser();
        _playerInput.AttachInputKeys(keys);
        _playerInput.AttachInputButtons(mouseButtons);
        for (int i = 0; i < inputList.Count; ++i)
        {
            PlayerInputCfg.PlayerInput input = inputList[i];
            switch ((InputType)input.inputType)
            {
                case InputType.KeyCode:
                    {
                        switch ((InputActionType)input.inputAction)
                        {
                            case InputActionType.MovePlayerUp:
                                _playerInput.AttachKeyAction((KeyCode)input.key, MoveUp);
                                break;
                            case InputActionType.MovePlayerDown:
                                _playerInput.AttachKeyAction((KeyCode)input.key, MoveDown);
                                break;
                            case InputActionType.MovePlayerLeft:
                                _playerInput.AttachKeyAction((KeyCode)input.key, MoveLeft);
                                break;
                            case InputActionType.MovePlayerRight:
                                _playerInput.AttachKeyAction((KeyCode)input.key, MoveRight);
                                break;
                        }
                    }
                    break;
                case InputType.MouseButton:
                    _playerInput.AttachButtonAction((MouseButton)input.key, Attack);
                    break;
            }
        }
        _playerInput.Activate();
    }

    private void MoveUp()
    {
        position += Vector3.up * Timing.deltaTime * 10f;
        CameraController.Instance.UpdatePosition();
    }

    private void MoveDown()
    {
        position += Vector3.down * Timing.deltaTime * 10f;
        CameraController.Instance.UpdatePosition();
    }

    private void MoveLeft()
    {
        position += Vector3.left * Timing.deltaTime * 10f;
        CameraController.Instance.UpdatePosition();
    }

    private void MoveRight()
    {
        position += Vector3.right * Timing.deltaTime * 10f;
        CameraController.Instance.UpdatePosition();
    }

    private void Attack()
    {
        Debug.Log("Attack");
    }

    public void EnablePlayerInput()
    {
        if (characterType == CharacterType.Player)
        {
            _playerInput.Executable = true;
        }
    }
    public void DisablePlayerInput()
    {
        if (characterType == CharacterType.Player)
        {
            _playerInput.Executable = false;
        }
    }
}
