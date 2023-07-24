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
        AssetsManager.Instance.LoadAsset<GameObject>(cfg.prefab, gameObject =>
        {
            GameObject go = MonoExtension.InitializeObject(gameObject);
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
        }
    }

    private void _PlayerControl()
    {
        _UpdatePlayerDir();
    }

    private void _UpdatePlayerDir()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 playerScreenPos = MonoExtension.WorldPos2ScreenPos(Player.position);
        Vector3 playerScale = Player.scale;
        playerScale.x = mousePosition.x >= playerScreenPos.x ? 1f : -1f;
        if (playerScale != Player.scale)
        {
            Player.scale = playerScale;
        }
    }
}
