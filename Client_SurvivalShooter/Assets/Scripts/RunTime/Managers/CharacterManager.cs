using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excalibur;
using System;

public class CharacterManager : Singleton<CharacterManager>
{
    private Dictionary<CharacterType, List<Character>> chracters;

    public Character player => player;

    protected override void OnConstructed()
    {
        chracters = new Dictionary<CharacterType, List<Character>>();
    }

    public Character CreateCharacter(int id, Action onCreated = default)
    {
        Character role = new Character();
        role.InitData(id);
        AddCharacter(role.characterType, role);
        return role;
    }

    public void AddCharacter(CharacterType type, Character character)
    {
        if (!chracters.ContainsKey(type))
        {
            chracters.Add(type, new List<Character>());
        }
        chracters[type].Add(character);
    }
}
