using System.Text;

namespace   Cottle.Cleaners
{
	class   BlankCharactersCleaner : ICleaner
    {
        #region Methods
        
        public string   Clean (StringBuilder buffer)
        {
            int start;
            int stop;
            int i;

            // Skip all leading blank characters
            for (i = 0; i < buffer.Length && buffer[i] <= ' '; )
                ++i;

            start = i;

            // Skip all trailing blank characters
            for (i = buffer.Length - 1; i >= 0 && buffer[i] <= ' '; )
                --i;

            stop = i + 1;

            return buffer.ToString (start, stop - start);
        }
        
        #endregion
    }
}
