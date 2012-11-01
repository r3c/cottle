using System.Text;

namespace   Cottle
{
    public interface    ICleaner
    {
        #region Methods

        void    GetRange (string text, out int start, out int length);

        #endregion
    }
}
