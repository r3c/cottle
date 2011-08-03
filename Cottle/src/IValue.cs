using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace   Cottle
{
    using   ChildList = List<KeyValuePair<IValue, IValue>>;

    public interface    IValue
    {
        #region Properties

        bool        AsBoolean
        {
            get;
        }

        Function    AsFunction
        {
            get;
        }

        decimal     AsNumber
        {
            get;
        }

        string      AsString
        {
            get;
        }

        ChildList   Children
        {
            get;
        }

        #endregion

        #region Methods

        bool    Find (string name, out IValue child);

        bool    Has (string name);

        #endregion
    }
}
