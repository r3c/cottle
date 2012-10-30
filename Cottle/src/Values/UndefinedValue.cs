
namespace   Cottle.Values
{
    public sealed class UndefinedValue : Value
    {
        #region Properties

        public override bool            AsBoolean
        {
            get
            {
                return false;
            }
        }

        public override IFunction       AsFunction
        {
            get
            {
                return null;
            }
        }

        public override decimal         AsNumber
        {
            get
            {
                return 0;
            }
        }

        public override string          AsString
        {
            get
            {
                return string.Empty;
            }
        }

        public override FieldMap        Fields
        {
            get
            {
                return FieldMap.Empty;
            }
        }

        public override ValueContent    Type
        {
            get
            {
                return ValueContent.Undefined;
            }
        }

        #endregion

        #region Properties / Static

        public static UndefinedValue    Instance
        {
            get
            {
                return UndefinedValue.instance;
            }
        }

        #endregion

        #region Attributes

        private static readonly UndefinedValue  instance = new UndefinedValue ();

        #endregion

        #region Methods

        public override int CompareTo (Value other)
        {
            if (other == null)
                return 1;
            else if (other.Type != ValueContent.Undefined)
                return -1;
            else
                return 0;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override string  ToString ()
        {
            return "<undefined>";
        }

        #endregion
    }
}
