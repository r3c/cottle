using System;
using System.Collections.Generic;
using System.Text;

namespace   Cottle
{
    public struct   Function
    {
        #region Constants

        public static readonly Function Undefined = new Function (null);

        #endregion

        #region Properties

        public CallbackDelegate Callback
        {
            get
            {
                return this.callback;
            }
        }

        public int              Max
        {
            get
            {
                return this.max;
            }
        }

        public int              Min
        {
            get
            {
                return this.min;
            }
        }

        #endregion

        #region Attributes

        private CallbackDelegate    callback;

        private int                 max;

        private int                 min;

        #endregion

        #region Constructors

        public  Function (CallbackDelegate callback, int min, int max)
        {
            this.callback = callback;
            this.max = max;
            this.min = min;
        }

        public  Function (CallbackDelegate callback, int exact) :
            this (callback, exact, exact)
        {
        }

        public  Function (CallbackDelegate callback) :
            this (callback, 0, -1)
        {
        }

        #endregion

        #region Types

        public delegate IValue  CallbackDelegate (Argument[] arguments);

        #endregion
    }
}
