using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excalibur
{
    public sealed class ExecutableBehaviourAssistant : IExecutableBehaviour
    {
        private readonly List<IExecutableBehaviour> r_units = new List<IExecutableBehaviour> ();
        private readonly List<IExecutableBehaviour> r_toAttach = new List<IExecutableBehaviour> ();
        private readonly List<IExecutableBehaviour> r_toDetach = new List<IExecutableBehaviour> ();
        private int i;

        public bool Executable { get; set; }

        public ExecutableBehaviourAssistant ()
        {
            Executable = true;
        }

        public void Execute ()
        {
            if (!Executable) { return; }

            if (r_toAttach.Count > 0)
            {
                r_units.AddRange (r_toAttach);
                r_toAttach.Clear ();
            }

            if (r_toDetach.Count > 0)
            {
                for (i = 0; i < r_toDetach.Count; ++i)
                {
                    int lastIndex = r_units.Count - 1;
                    r_units[r_units.IndexOf (r_toDetach[i])] = r_units[lastIndex];
                    r_units.RemoveAt (lastIndex);
                }
                r_toDetach.Clear ();
            }

            if (r_units.Count > 0)
            {
                for (i = 0; i < r_units.Count; ++i)
                {
                    r_units[i].Execute();
                }
            }
        }

        public void Attach (IExecutableBehaviour unit)
        {
            r_toAttach.Add (unit);
        }

        public void Detach (IExecutableBehaviour unit)
        {
            r_toDetach.Add (unit);
        }

        public void Attach (List<IExecutableBehaviour> units)
        {
            r_toAttach.AddRange (units);
        }

        public void Detach (List<IExecutableBehaviour> units)
        {
            r_toDetach.AddRange (units);
        }
    }
}
