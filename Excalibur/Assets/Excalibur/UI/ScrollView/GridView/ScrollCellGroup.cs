using UnityEngine;
using System.Linq;

namespace Excalibur
{
    public abstract class ScrollCellGroup<TItemData, TContext> : ScrollCell<TItemData[], TContext>
        where TContext : class, IScrollCellGroupContext, new ()
    {
        protected virtual ScrollCell<TItemData, TContext>[] Cells { get; private set; }

        protected virtual ScrollCell<TItemData, TContext>[] InstantiateCells ()
        {
            return Enumerable.Range (0, Context.GetGroupCount ())
                .Select (_ => Instantiate (Context.CellTemplate, transform))
                .Select (x => x.GetComponent<ScrollCell<TItemData, TContext>> ())
                .ToArray ();
        }

        public override void Initialize ()
        {
            Cells = InstantiateCells ();
            Debug.Assert (Cells.Length == Context.GetGroupCount ());

            for (var i = 0; i < Cells.Length; i++)
            {
                Cells[i].SetContext (Context);
                Cells[i].Initialize ();
            }
        }

        public override void UpdateContent (TItemData[] contents)
        {
            var firstCellIndex = Index * Context.GetGroupCount ();

            for (var i = 0; i < Cells.Length; i++)
            {
                Cells[i].Index = i + firstCellIndex;
                Cells[i].SetVisible (i < contents.Length);

                if (Cells[i].IsVisible)
                {
                    Cells[i].UpdateContent (contents[i]);
                }
            }
        }

        public override void UpdatePosition (float position)
        {
            for (var i = 0; i < Cells.Length; i++)
            {
                Cells[i].UpdatePosition (position);
            }
        }
    }
}
