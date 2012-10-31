using System.Text;

namespace   Cottle.Cleaners
{
	public class    NothingCleaner : ICleaner
    {
        #region Methods
        
        public string   Clean (StringBuilder buffer)
        {
        	return buffer.ToString ();
        }
        
        #endregion
    }
}
