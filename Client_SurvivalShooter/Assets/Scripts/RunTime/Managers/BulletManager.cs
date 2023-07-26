using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excalibur;

public class BulletManager : Singleton<BulletManager>, IExecutableBehaviour
{
    private readonly Dictionary<int, ObjectPool<Bullet>> r_BulletPool = new Dictionary<int, ObjectPool<Bullet>>();

    private readonly ExecutableBehaviourAssistant r_BulletAssistant = new ExecutableBehaviourAssistant();

    public bool Executable { get; set; }

    protected override void OnConstructed()
    {
        GameManager.Instance.AttachExecutableUnit(this);
    }

    public Bullet CreateBullet(int bulletId)
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
                });
            }
        }

        return bullet;
    }

    public void ReleaseBullet(Bullet bullet)
    {
        if (!r_BulletPool.ContainsKey(bullet.bulletId))
        {
            r_BulletPool.Add(bullet.bulletId, new ObjectPool<Bullet>(bt => bt.SetActive(true), bt => bt.SetActive(false)));
        }

        r_BulletPool[bullet.bulletId].Release(bullet);
    }

    public void AttachBullet(Bullet bullet)
    {
        r_BulletAssistant.Attach(bullet);
    }
    

    public void DetachBullet(Bullet bullet)
    {
        r_BulletAssistant.Detach(bullet);
    }

    public void Execute()
    {
        if (Executable)
        {
            r_BulletAssistant.Execute();
        }
    }
}