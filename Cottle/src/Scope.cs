using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Values;

namespace   Cottle
{
    public class    Scope
    {
        #region Properties

        public Value    this[Value name]
        {
            get
            {
                Value   value;

                if (this.Get (name, out value))
                    return value;

                return UndefinedValue.Instance;
            }
            set
            {
                this.Set (name, value, ScopeSet.Anywhere);
            }
        }

        #endregion

        #region Attributes

        private Stack<HashSet<Value>>           levels = new Stack<HashSet<Value>> ();

        private Dictionary<Value, Stack<Value>> stacks = new Dictionary<Value, Stack<Value>> ();

        #endregion

        #region Constructors

        public  Scope ()
        {
            this.levels.Push (new HashSet<Value> ());
        }

        #endregion

        #region Methods

        public void Enter ()
        {
            this.levels.Push (new HashSet<Value> ());
        }

        public bool Get (Value name, out Value value)
        {
            Stack<Value>   stack;

            if (this.stacks.TryGetValue (name, out stack) && stack.Count > 0)
            {
                value = stack.Peek ();

                return true;
            }

            value = null;

            return false;
        }

        public void Leave ()
        {
            Stack<Value>   stack;

            if (this.levels.Count > 1)
            {
                foreach (Value name in this.levels.Pop ())
                {
                    if (this.stacks.TryGetValue (name, out stack))
                        stack.Pop ();
                }
            }
        }

        public bool Set (Value name, Value value, ScopeSet set)
        {
            HashSet<Value>  level;
            Stack<Value>    stack;

            if (!this.stacks.TryGetValue (name, out stack))
            {
                stack = new Stack<Value> ();

                this.stacks[name] = stack;
            }

            level = this.levels.Peek ();

            switch (set)
            {
                case ScopeSet.Anywhere:
                    if (stack.Count > 0)
                        stack.Pop ();

                    break;

                case ScopeSet.Local:
                    if (!level.Add (name))
                        stack.Pop ();

                    break;
            }

            stack.Push (value);

            return true;
        }

        #endregion
    }

    public enum ScopeSet
    {
        Anywhere,
        Local
    }
}
