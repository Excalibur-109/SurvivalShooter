using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excalibur;

public class SpawnManager : Singleton<SpawnManager>
{
    public enum SpawnType
    {
        Player = 1,
        AI = 2,
        Item = 3,
    }

    public void SpawnUnit(int spawnId)
    {
        SpawnCfg.Spawn cfg = SpawnCfg.TryGetValue(spawnId);
        
        Vector3 center = Vector3.zero;
        for (int i = 0; i < cfg.position.Length; i++)
        {
            center[i] = cfg.position[i];
        }
        SpawnType spawnType = (SpawnType)cfg.spawnType;
        float radius = cfg.radius;
        for (int i = 0; i  < cfg.targets.Length; ++i)
        {
            int targetId = cfg.targets[i];
            if (spawnType != SpawnType.Item)
            {
                Character character = CharacterManager.Instance.CreateCharacter(targetId);
                float x = Random.Range(-radius,radius);
                float y = Mathf.Sqrt(radius * radius - x * x);
                int sign = Random.value >= 0.5f ? 1 : -1;
                y *= sign;
                x += center.x;
                y += center.y;
                character.SetPosition(new Vector3(x, y, 0f));
            }
        }
    }
}
