using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excalibur;

public class Weapon : Unit
{
    protected WeaponData weaponData;

    public void InitData(int weaponId)
    {
        WeaponCfg.Weapon cfg = WeaponCfg.TryGetValue(weaponId);
        AssetsManager.Instance.LoadAsset<GameObject>(cfg.prefabName, (gameObject) =>
        {

        });
    }

    public void Fire()
    {

    }

    public void UpdateRotation()
    {

    }
}