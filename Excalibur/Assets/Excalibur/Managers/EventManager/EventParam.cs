using System.Collections.Generic;

namespace Excalibur
{
    public class EventParam
    {
        public EventTrigger trigger;
        List<object> parameters;

        public EventParam (EventTrigger trigger = EventTrigger.None)
        {
            this.trigger = trigger;
            parameters = new List<object> ();
        }

        public void AddParam (object parameter)
        {
            parameters.Add (parameter);
        }

        public T GetParam<T> (int index)
        {
            if (index >= 0 && index < parameters.Count)
            {
                return (T)parameters[index];
            }
            return default (T);
        }

        ~EventParam () { parameters = null; }
    }
}