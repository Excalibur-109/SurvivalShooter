using UnityEngine;
using Excalibur;

public class HealthComponent : IComponent
{
	public int hp;
}

public class MagicPowerComponent : IComponent
{
	public int mp;
}

public class PositionComponent : IComponent
{
	public Vector3 pos;
}

public class SpeedComponent : IComponent
{
	public float speed;
}

public class AttackComponent : IComponent
{
	public int attack;
}

public class DefenceComponent : IComponent
{
	public int defence;
}

public class SightComponent : IComponent
{
	public float sight;
}

public class LevelComponent : IComponent
{
	public int level;
}

public class ExpComponent : IComponent
{
	public int exp;
}

public class IDComponent : IComponent
{
	public int id;
}

public class AmountComponent : IComponent
{
	public int amount;
}

public class AttackIntervalComponent : IComponent
{
	public int interval;
}

public class IsMoveComponent : IComponent
{
	public bool isMoving;
}

