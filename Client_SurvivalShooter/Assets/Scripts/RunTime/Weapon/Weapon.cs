using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excalibur;
using System;

public class Weapon : Unit
{
    protected WeaponData weaponData;

    private Color _bulletColor;
    public Color bulletColor
    {
        set { _bulletColor = value; }
    }

    public void InitData(int weaponId, Character owner)
    {
        WeaponCfg.Weapon cfg = WeaponCfg.TryGetValue(weaponId);
        weaponData = new WeaponData();
        AssetsManager.Instance.LoadAsset<GameObject>(cfg.prefabName, (gameObject) =>
        {
            GameObject weaponObj = MonoExtension.InitializeObject(gameObject);
            ScenesManager.Instance.MoveObjectToGameScene(weaponObj);
            Attach(weaponObj);
            SetParent(owner.transform);
            transform.localPosition = Utility.FloatArrToVector3(cfg.spawnPos);
        });
    }

    public void Fire()
    {

    }

    public void UpdateRotation()
    {

    }
}
