using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excalibur;
using System;

public class BulletManager : Singleton<BulletManager>, IExecutableBehaviour
{
    private readonly Dictionary<int, ObjectPool<Bullet>> r_BulletPool = new Dictionary<int, ObjectPool<Bullet>>();
    private readonly Dictionary<SignFlag, List<Bullet>> r_SignedBullets = new Dictionary<SignFlag, List<Bullet>>();
    private readonly List<Bullet> r_SignedRemove = new List<Bullet>();
    private readonly ExecutableBehaviourAssistant r_BulletAssistant = new ExecutableBehaviourAssistant();

    public bool Executable { get; set; }

    protected override void OnConstructed()
    {
        GameManager.Instance.AttachExecutableUnit(this);
        SignFlag flag = SignFlag.Nothing;
        while (flag <= SignFlag.Red)
        {
            r_SignedBullets.Add(flag++, new List<Bullet>());
        }
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

    public List<Bullet> GetSignBullets(SignFlag signFlag)
    {
        if (signFlag == SignFlag.Nothing) { return null; }
        if (r_SignedRemove.Count > 0)
        {
            int i = -1;
            while (++i < r_SignedRemove.Count)
            {
                Bullet bullet = r_SignedRemove[i];
                List<Bullet> list = r_SignedBullets[bullet.flag];
                list[list.IndexOf(bullet)] = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
            }
            r_SignedRemove.Clear();
        }
        return r_SignedBullets[signFlag];
    }

    private ObjectPool<Bullet> _CreatePool()
    {
        ObjectPool<Bullet> pool = new ObjectPool<Bullet>(
            item => { r_BulletAssistant.Attach(item); item.SetActive(true); },
            item => 
            {
                r_BulletAssistant.Detach(item); 
                item.SetActive(false);
                r_SignedRemove.Add(item);
            }
        );
        return pool;
    }
}
