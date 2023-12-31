using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excalibur;
using System;
using Unity.IO.LowLevel.Unsafe;

public enum CharacterType { Player = 1, AI = 2 }

public enum SignFlag { Nothing = 0, Green = 1, Red = 2 }

public class Character : Unit
{
    private CharacterType _characterType;
    private SignFlag _signFlag;
    private CharacterData _characterData;
    private InputResponser _playerInput;
    private FinitStateMachine _fsm;
    private Weapon _weapon;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _collider2D;

    private CharacterData characterData => _characterData;
    public CharacterType characterType => _characterType;
    public SignFlag signFlag => _signFlag;
    public Weapon weapon => _weapon;

    protected override void OnAttached()
    {
        base.OnAttached();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider2D = GetComponent<BoxCollider2D>();
    }

    public void InitData(int id)
    {
        CharacterCfg.Character cfg = CharacterCfg.TryGetValue(id);
        _characterType = (CharacterType)cfg.type;
        _signFlag = (SignFlag)cfg.flag;
        _characterData = new CharacterData();
        _characterData.idCom.id = id;
        Color color = Utility.FloatArrToColor(cfg.color);
        _spriteRenderer.color = color;
        _weapon = new Weapon();
        _weapon.bulletColor = color;
        _weapon.InitData(cfg.weaponId, this);

        _fsm = new FinitStateMachine();
        _fsm.Attach(this);
        _fsm.LinkState(FinitState.Idle, _GetState(FinitState.Idle));
        _fsm.LinkState(FinitState.Run, _GetState(FinitState.Run));
        _fsm.LinkState(FinitState.Attack, _GetState(FinitState.Attack));
        _fsm.LinkState(FinitState.Dead, _GetState(FinitState.Dead));
        if (characterType == CharacterType.Player)
        {
            _InitInput();
            CameraController.Instance.SetTarget(_characterData.positionCom);
        }
        else
        {
            _fsm.LinkState(FinitState.Chase, _GetState(FinitState.Chase));
            gameObject.name = "AI-" + cfg.id.ToString();
        }
        _fsm.TransitionState(FinitState.Idle);
        _fsm.Start();
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
                                _playerInput.AttachKeyAction((KeyCode)input.key, _MoveUp);
                                _playerInput.AttachKeyUpAction((KeyCode)input.key, _OnMoveOver);
                                break;
                            case InputActionType.MovePlayerDown:
                                _playerInput.AttachKeyAction((KeyCode)input.key, _MoveDown);
                                _playerInput.AttachKeyUpAction((KeyCode)input.key, _OnMoveOver);
                                break;
                            case InputActionType.MovePlayerLeft:
                                _playerInput.AttachKeyAction((KeyCode)input.key, _MoveLeft);
                                _playerInput.AttachKeyUpAction((KeyCode)input.key, _OnMoveOver);
                                break;
                            case InputActionType.MovePlayerRight:
                                _playerInput.AttachKeyAction((KeyCode)input.key, _MoveRight);
                                _playerInput.AttachKeyUpAction((KeyCode)input.key, _OnMoveOver);
                                break;
                        }
                    }
                    break;
                case InputType.MouseButton:
                    _playerInput.AttachButtonAction((MouseButton)input.key, _Attack);
                    break;
            }
        }
        _playerInput.Activate();
    }

    private void _MoveUp()
    {
        Vector3 nextPos = _characterData.positionCom.pos + Vector3.up * Timing.deltaTime * 7f;
        if (ScenesManager.Instance.IsInBound(nextPos))
        {
            SetPosition(nextPos);
            CameraController.Instance.UpdatePosition();
        }
        _characterData.isMovingCom.isMoving = true;
    }

    private void _MoveDown()
    {
        Vector3 nextPos = _characterData.positionCom.pos + Vector3.down * Timing.deltaTime * 7f;
        if (ScenesManager.Instance.IsInBound(nextPos))
        {
            SetPosition(nextPos);
            CameraController.Instance.UpdatePosition();
        }
        _characterData.isMovingCom.isMoving = true;
    }

    private void _MoveLeft()
    {
        Vector3 nextPos = _characterData.positionCom.pos + Vector3.left * Timing.deltaTime * 7f;
        if (ScenesManager.Instance.IsInBound(nextPos))
        {
            SetPosition(nextPos);
            CameraController.Instance.UpdatePosition();
        }
        _characterData.isMovingCom.isMoving = true;
    }

    private void _MoveRight()
    {
        Vector3 nextPos = _characterData.positionCom.pos + Vector3.right * Timing.deltaTime * 7f;
        if (ScenesManager.Instance.IsInBound(nextPos))
        {
            SetPosition(nextPos);
            CameraController.Instance.UpdatePosition();
        }
        _characterData.isMovingCom.isMoving = true;
    }

    private void _OnMoveOver()
    {
        _characterData.isMovingCom.isMoving = false;
    }

    private void _Attack()
    {
        weapon.Fire();
    }

    public void TakeDamage(int damage)
    {

    }

    private BaseFiniteState _GetState(FinitState finitState)
    {
        switch (finitState)
        {
            case FinitState.Idle:
                break;
            case FinitState.Walk:
                break;
            case FinitState.Run:
                {
                    RunState runState = new RunState();
                    runState.SetComponent(_characterData.isMovingCom);
                    return runState;
                }
            case FinitState.Chase:
                {
                    ChaseState chaseState = new ChaseState();
                    chaseState.SetComponent(_characterData.positionCom);
                }
                break;
            case FinitState.Attack:
                {
                    AttackState chaseState = new AttackState();
                    chaseState.SetComponent(_characterData.positionCom);
                }
                break;
            case FinitState.Hurt:
                break;
            case FinitState.Dead:
                break;
        }
        IdleState idleState = new IdleState();
        idleState.SetComponent(_characterData.isMovingCom);
        return idleState;
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

    public void SetPosition(Vector3 position)
    {
        characterData.positionCom.pos = position;
        transform.position = characterData.positionCom.pos;
    }

    protected override void OnDetached()
    {
        base.OnDetached();
        _fsm.Dispose();
    }

    public bool JudgeCollision(Vector3 position)
    {
        return _collider2D.bounds.Contains(position);
    }
}
