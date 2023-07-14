
using System.Collections.Generic;

namespace Excalibur
{
    public interface IObserver
    {
        void Execute ();
    }

    public interface ISubject
    {
        void AttachObserver (IObserver observer);
        void DetachObserver (IObserver observer);
        void Notify ();
    }
}