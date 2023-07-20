using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excalibur;
using System.Xml.Serialization;

public class SpawnManager : Singleton<SpawnManager>
{
    public enum SpawnType
    {
        Character = 1,
        Item = 2,
        Player = 3,
    }

    public void SpawnCharacters(int spawnId)
    {

    }
}
