﻿using UnityEngine;

namespace Excalibur
{
    public interface IEntity : IActivateBehaviour, IReactivateBehaviour, IDeactivateBehaviour, ITerminateBehaviour, IExecuteBehaviour
    {
        void Attach (GameObject gameObject);
        void Detach ();
    }

    public interface IUnitEntity : IEntity
    {
        string name { get; }
        bool visible { get; set; }
        GameObject gameObject { get; }
        Transform transform { get; }
        Vector3 position { get; set; }
        Vector3 scale { get; set; }
        Vector3 eulerAngle { get; set; }
        Quaternion rotation { get; set; }
        void SetActive (bool active);
        void SetParent (Transform parent);
        Component AddComponent<T> () where T : Component;
        Component GetComponent<T> () where T : Component;
        bool RemoveComponent<T> () where T : Component;
        void Destory ();
    }
}