using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excalibur;
using System;

public class BulletManager : Singleton<BulletManager>, IExecutableBehaviour
{
    private readonly Dictionary<int, ObjectPool<Bullet>> r_BulletPool = new Dictionary<int, ObjectPool<Bullet>>();

    private readonly ExecutableBehaviourAssistant r_BulletAssistant = new ExecutableBehaviourAssistant();

    public bool Executable { get; set; }

    protected override void OnConstructed()
    {
        GameManager.Instance.AttachExecutableUnit(this);
    }

    public Bullet CreateBullet(int bulletId, Action onCreated)
    {
        Bullet bullet = default;
        if (r_BulletPool.TryGetValue(bulletId, out ObjectPool<Bullet> pool))
        {
            if (pool.countInactive > 0)
            {
                bullet = pool.Get();
            }
            else
            {
                BulletCfg.Bullet cfg = BulletCfg.TryGetValue(bulletId);
                bullet = new Bullet();
                AssetsManager.Instance.LoadAsset<GameObject>(cfg.prefab, gameObject => 
                {
                    GameObject bulletObj = MonoExtension.InstantiateObject(gameObject);
                    bullet.Attach(bulletObj);
                    r_BulletAssistant.Attach(bullet);
                    onCreated?.Invoke();
                });
            }
        }
        else
        {
            r_BulletPool.Add(bulletId, _CreatePool());
        }

        return bullet;
    }

    public void ReleaseBullet(Bullet bullet)
    {
        if (!r_BulletPool.ContainsKey(bullet.bulletId))
        {
            r_BulletPool.Add(bullet.bulletId, _CreatePool());
        }

        r_BulletPool[bullet.bulletId].Release(bullet);
    }

    public void Execute()
    {
        if (Executable)
        {
            r_BulletAssistant.Execute();
        }
    }

    private ObjectPool<Bullet> _CreatePool()
    {
        ObjectPool<Bullet> pool = new ObjectPool<Bullet>(
            item => { r_BulletAssistant.Attach(item); item.SetActive(true); },
            item => { r_BulletAssistant.Detach(item); item.SetActive(false); }
        );
        return pool;
    }
}
