using System;
using System.Collections.Generic;
using System.Text;

namespace Playground_The_Highland_For_Shitty_Code.Classes
{
    class ChainedTuple
    {
        public virtual object Field { get; set; }
        public virtual object Next { get; set; }
        public ChainedTuple(TA a, TB b)
        {
            Field = a;
            Next = b;
        }
        public IEnumerable<KeyValuePair<Type, object>> Fields
        {
            get
            {
                var t = this;
                IList<object> ts = new LinkedList<object>();
                while (t is ChainedTuple)
                {
                    ts.Add(new KeyValuePair(t.GetType(), t));
                    t = t.Next;
                }
                return ts;
            }
        }
    }

    class ChainedTuple<TA, TB> : ChainedTuple
    {
        public override TA Field { get; set; }
        public override TB Next { get; set; }
        public ChainedTuple(TA a, TB b)
        {
            Field = a;
            Next = b;
        }
    }

    class GenericType
    {
        static void lol()
        {
            var lol = new { a, t };
        }

    }
}
