using System.Collections.Generic;
using UnityEngine;
using Excalibur;

public class CharacterManager : Singleton<CharacterManager>, IExecutableBehaviour
{
    public readonly ExecutableBehaviourAssistant fsmAssistant = new ExecutableBehaviourAssistant();
    private Dictionary<CharacterType, List<Character>> _chracters;
    private ObjectPool<Character> _characterPool;
    public Character Player => _chracters[CharacterType.Player][0];

    public bool Executable { get; set; } 

    protected override void OnConstructed()
    {
        _chracters = new Dictionary<CharacterType, List<Character>>();
        _characterPool = new ObjectPool<Character>(null, (target) => target.Detach());
        GameManager.Instance.AttachExecutableUnit(this);
    }

    public Character CreateCharacter(int id)
    {
        Character role = _characterPool.Get();
        CharacterCfg.Character cfg = CharacterCfg.TryGetValue(id);
        AddCharacter((CharacterType)cfg.type, role);
        AssetsManager.Instance.LoadAsset<GameObject>(ConstParam.CHARACTER_PREFAB, gameObject =>
        {
            GameObject go = MonoExtension.InstantiateObject(gameObject);
            role.Attach(go);
            ScenesManager.Instance.MoveObjectToGameScene(go);
            role.InitData(id);
        });
        return role;
    }

    public void AddCharacter(CharacterType type, Character character)
    {
        if (!_chracters.ContainsKey(type))
        {
            _chracters.Add(type, new List<Character>());
        }
        _chracters[type].Add(character);
    }

    public void Execute()
    {
        if (Executable)
        {
            fsmAssistant.Execute();
            if (Player != null)
            {
                _PlayerControl();
            }
            _JudgeCollision();
        }
    }

    private void _PlayerControl()
    {
        _UpdatePlayerDirection();
    }

    private void _UpdatePlayerDirection()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 playerScreenPos = MonoExtension.WorldPos2ScreenPos(Player.position);
        Vector3 playerScale = Player.scale;
        playerScale.x = mousePosition.x >= playerScreenPos.x ? 1f : -1f;
        Weapon weapon = Player.weapon;
        if (playerScale != Player.scale)
        {
            Player.scale = playerScale;
        }

        Vector2 weaponScreenPos = MonoExtension.WorldPos2ScreenPos(weapon.position);
        Vector2 direction = mousePosition - weaponScreenPos;
        float angle = Vector2.Angle(Vector2.right, direction);
        angle = mousePosition.y >= weaponScreenPos.y ? angle : -angle;
        if (playerScale.x < 0f)
        {
            angle -= 180f;
        }
        weapon.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void _JudgeCollision()
    {
        SignFlag flag = SignFlag.Green;
        while (flag <= SignFlag.Red)
        {
            List<Bullet> bullets = BulletManager.Instance.GetSignBullets(flag);
            foreach (CharacterType characterType in _chracters.Keys)
            {
                List<Character> characters = _chracters[characterType];
                int i = -1;
                while (++i < characters.Count)
                {
                    Character character = characters[i];
                    SignFlag characterFlag = character.signFlag;
                    if (characterFlag != flag)
                    {
                        int j = -1;
                        while (++j < bullets.Count)
                        {
                            if (character.JudgeCollision(bullets[j].position))
                            {
                                character.TakeDamage(bullets[j].attackValue);
                            }
                        }
                    }
                }
            }
            ++flag;
        }
    }
}
