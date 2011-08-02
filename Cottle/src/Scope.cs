using System;
using System.Collections.Generic;
using System.Text;

namespace   Cottle
{
    public class    Scope
    {
        #region Attributes

        private Stack<HashSet<string>>              levels = new Stack<HashSet<string>> ();

        private Dictionary<string, Stack<IValue>>   stacks = new Dictionary<string, Stack<IValue>> ();

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

        public bool Get (string name, out IValue value)
        {
            Stack<IValue>   stack;

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
            Stack<IValue>   stack;

            if (this.levels.Count > 1)
            {
                foreach (string name in this.levels.Pop ())
                {
                    if (this.stacks.TryGetValue (name, out stack))
                        stack.Pop ();
                }
            }
        }

        public void Set (string name, IValue value)
        {
            HashSet<string> level;
            Stack<IValue>   stack;

            if (!this.stacks.TryGetValue (name, out stack))
            {
                stack = new Stack<IValue> ();

                this.stacks[name] = stack;
            }

            level = this.levels.Peek ();

            if (level.Contains (name))
                stack.Pop ();
            else
                level.Add (name);

            stack.Push (value);
        }

        #endregion
    }
}
