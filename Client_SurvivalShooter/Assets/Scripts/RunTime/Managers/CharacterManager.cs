using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excalibur;
using System;

public class UnitManager : Singleton<UnitManager>
{
    public static T CreateUnit<T>(GameObject gameObject, Action<T> onCreated) where T : Unit, new()
    {
        T unit = new T();
        unit.Attach(gameObject);
        onCreated(unit);
        return unit;
    }
}
