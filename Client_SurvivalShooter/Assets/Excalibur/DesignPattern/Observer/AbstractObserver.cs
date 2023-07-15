using System.Collections.Generic;

namespace Excalibur
{
    public abstract class Observer : IObserver
    {
        public virtual void Execute () {}
    }
}