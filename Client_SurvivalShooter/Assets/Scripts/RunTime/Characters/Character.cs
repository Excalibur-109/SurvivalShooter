using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excalibur;

public enum CharacterType { Player = 1, AI = 2 }

public enum AIFlag { Green = 1, Red = 2 }

public class CharacterData
{
    public HealthComponent health;
    public PositionComponent position;
    public SpeedComponent speed;
}

public class Character : Unit
{
    private CharacterType _characterType;
    private AIFlag _aiFlag;

    public CharacterType characterType => _characterType;
    public AIFlag aiFlag => _aiFlag;
}
