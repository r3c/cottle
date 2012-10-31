using System.Text;

namespace   Cottle.Cleaners
{
	class   NothingCleaner : ICleaner
    {
        #region Methods
        
        public string   Clean (StringBuilder buffer)
        {
        	return buffer.ToString ();
        }
        
        #endregion
    }
}
