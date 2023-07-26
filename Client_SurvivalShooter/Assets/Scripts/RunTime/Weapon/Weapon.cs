using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excalibur;
using System;

public class Weapon : Unit
{
    private Transform _fireHole;
    private IDComponent _bullet;
    protected WeaponData weaponData;

    private Color _bulletColor;
    public Color bulletColor { set { _bulletColor = value; } }

    public void InitData(int weaponId, Character owner)
    {
        WeaponCfg.Weapon cfg = WeaponCfg.TryGetValue(weaponId);
        weaponData = new WeaponData();
        AssetsManager.Instance.LoadAsset<GameObject>(cfg.prefabName, (gameObject) =>
        {
            GameObject weaponObj = MonoExtension.InstantiateObject(gameObject);
            ScenesManager.Instance.MoveObjectToGameScene(weaponObj);
            Attach(weaponObj);
            SetParent(owner.transform);
            transform.localPosition = Utility.FloatArrToVector3(cfg.spawnPos);
            if (transform.childCount > 0)
            {
                _fireHole = transform.GetComponentInChildren<Transform>();
            }
        });
    }

    public void SetBullet(int bulletId)
    {
        weaponData.bulletId.id = bulletId;
    }

    public void Fire()
    {

    }

    public void UpdateRotation()
    {

    }
}
