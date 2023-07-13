using UnityEngine;

namespace Excalibur
{
    public interface IActivateBehaviour
    {
        void Activate();
    }

    public interface IReactivateBehaviour
    {
        void Reactivate();
    }

    public interface IDeactivateBehaviour
    {
        void Deactivate();
    }

    public interface ITerminateBehaviour
    {
        void Terminate();
    }

    public interface IExecuteBehaviour
    {
        void Execute (); 
    }

    public interface IExecutableBehaviour : IExecuteBehaviour
    {
        /// <summary> 该属性由Execute内部处理 /// </summary>
        bool Executable { get; set; }
    }
}