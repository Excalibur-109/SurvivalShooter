public class CharacterData
{
    public IDComponent idCom;
    public HealthComponent hpCom;
    public MagicPowerComponent mpCom;
    public LevelComponent levelCom;
    public PositionComponent positionCom;
    public AttackComponent attackCom;
    public DefenceComponent defenceCom;
    public AttackIntervalComponent attackInvervalCom;
    public ExpComponent expCom;
    public IsMoveComponent isMovingCom;
    public IDComponent weaponIdCom;

    public CharacterData()
    {
        idCom = new IDComponent();
        hpCom = new HealthComponent();
        mpCom = new MagicPowerComponent();
        levelCom = new LevelComponent();
        positionCom = new PositionComponent();
        attackCom = new AttackComponent();
        defenceCom = new DefenceComponent();
        attackInvervalCom = new AttackIntervalComponent();
        expCom = new ExpComponent();
        isMovingCom = new IsMoveComponent();
        weaponIdCom = new IDComponent();
    }
}

public class WeaponData
{
    public IDComponent idCom;
    public AttackComponent attackCom;
    public SpeedComponent speedToBulletCom;
    public AttackIntervalComponent attackInvervalCom;
    public IDComponent bulletIdCom;

    public WeaponData()
    {
        idCom = new IDComponent();
        attackCom = new AttackComponent();
        speedToBulletCom = new SpeedComponent();
        attackInvervalCom = new AttackIntervalComponent();
        bulletIdCom = new IDComponent();
    }
}

public class BulletData
{
    public IDComponent idCom;
    public AttackComponent attackCom;
    public SpeedComponent speedCom;

    public BulletData()
    {
        idCom = new IDComponent();
        attackCom = new AttackComponent();
        speedCom = new SpeedComponent();
    }
}

public class ItemData
{
    public IDComponent idCom;
    public AmountComponent amountCom;

    public ItemData()
    {
        idCom = new IDComponent();
        amountCom = new AmountComponent();
    }
}

