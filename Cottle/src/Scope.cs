using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Values;

namespace   Cottle
{
    class   Scope
    {
        #region Attributes

        private Stack<HashSet<string>>              levels = new Stack<HashSet<string>> ();

        private Dictionary<string, Stack<Value>>   stacks = new Dictionary<string, Stack<Value>> ();

        #endregion

        #region Constructors

        public  Scope ()
        {
            this.levels.Push (new HashSet<string> ());
        }

        #endregion

        #region Methods

        public void Enter ()
        {
            this.levels.Push (new HashSet<string> ());
        }

        public bool Get (string name, out Value value)
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
                foreach (string name in this.levels.Pop ())
                {
                    if (this.stacks.TryGetValue (name, out stack))
                        stack.Pop ();
                }
            }
        }

        public bool Set (string name, Value value, SetMode mode)
        {
            HashSet<string> level;
            Stack<Value>    stack;

            if (!this.stacks.TryGetValue (name, out stack))
            {
                stack = new Stack<Value> ();

                this.stacks[name] = stack;
            }

            level = this.levels.Peek ();

            switch (mode)
            {
                case SetMode.ANYWHERE:
                    if (stack.Count == 0)
                        level.Add (name);
                    else
                        stack.Pop ();

                    break;

                case SetMode.LOCAL:
                    if (!level.Add (name))
                        stack.Pop ();

                    break;
            }

            stack.Push (value);

            return true;
        }

        #endregion

        #region Types

        public enum SetMode
        {
            ANYWHERE,
            LOCAL
        }

        #endregion
    }
}
