using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excalibur;
using System;
using UnityEngine.UIElements;

public class CharacterManager : Singleton<CharacterManager>
{
    private Dictionary<CharacterType, List<Character>> _chracters;

    private ObjectPool<Character> _characterPool;

    public Character Player => _chracters[CharacterType.Player][0];

    protected override void OnConstructed()
    {
        _chracters = new Dictionary<CharacterType, List<Character>>();
        _characterPool = new ObjectPool<Character>(null, (target) => target.Detach());
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
}
