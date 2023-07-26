using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excalibur;

public class Bullet : Unit
{
    private SpriteRenderer _spriteRenderer;
    private BulletData _bulletData;
    private Vector3 _flyDir;

    public int bulletId => _bulletData.id.id;

    public Color bulletColor
    {
        set { _spriteRenderer.color = value; }
    }

    protected override void OnAttached()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _bulletData = new BulletData();
    }

    public void SetAttackValue(int attackValue)
    {
        _bulletData.attack.attack = attackValue;
    }

    public void SetSpeedValue(int speedValue)
    {
        _bulletData.speed.speed = speedValue;
    }
    
    public void Fly(Vector3 direction)
    {
        _flyDir = direction;
    }

    public override void Execute()
    {
        if (Executable)
        {

        }
    }

    private void _Fly()
    {
        GameManager.Instance.AttachExecutableUnit(this);
    }

    private void _Boom()
    {
        GameManager.Instance.DetachExecutableUnit(this);
    }
}
