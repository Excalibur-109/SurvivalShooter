public class CharacterData
{
    public IDComponent id;
    public HealthComponent hp;
    public MagicPowerComponent mp;
    public LevelComponent level;
    public PositionComponent position;
    public SpeedComponent speed;
    public AttackComponent attack;
    public DefenceComponent defence;
    public AttackIntervalComponent attackInverval;
    public ExpComponent exp;

    public CharacterData()
    {
        id = new IDComponent();
        hp = new HealthComponent();
        mp = new MagicPowerComponent();
        level = new LevelComponent();
        position = new PositionComponent();
        speed = new SpeedComponent();
        attack = new AttackComponent();
        defence = new DefenceComponent();
        attackInverval = new AttackIntervalComponent();
        exp = new ExpComponent();
    }
}

public class WeaponData
{
    public IDComponent id;
    public AttackComponent attack;
    public SpeedComponent speedToBullet;
    public AttackIntervalComponent attackInverval;
}

public class BulletData
{
    public IDComponent id;
    public AttackComponent attack;
    public SpeedComponent speed;
}

public class ItemData
{
    public IDComponent id;
    public AmountComponent amount;
}

