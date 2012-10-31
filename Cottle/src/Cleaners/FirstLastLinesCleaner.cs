using System.Text;

namespace   Cottle.Cleaners
{
	class   FirstLastLinesCleaner : ICleaner
    {
        #region Methods
        
        public string   Clean (StringBuilder buffer)
        {
            int start;
            int stop;
            int i;

            // Skip first line if any
            for (i = 0; i < buffer.Length && buffer[i] <= ' ' && buffer[i] != '\n' && buffer[i] != '\r'; )
                ++i;

            if (i >= buffer.Length || (buffer[i] != '\n' && buffer[i] != '\r'))
                start = 0;
            else if (i + 1 >= buffer.Length || buffer[i] == buffer[i + 1] || (buffer[i + 1] != '\n' && buffer[i + 1] != '\r'))
                start = i + 1;
            else
                start = i + 2;

            // Skip last line if any
            for (i = buffer.Length - 1; i >= 0 && buffer[i] <= ' ' && buffer[i] != '\n' && buffer[i] != '\r'; )
                --i;

            if (i < 0 || (buffer[i] != '\n' && buffer[i] != '\r'))
                stop = buffer.Length;
            else if (i < 1 || buffer[i] == buffer[i - 1] || (buffer[i - 1] != '\n' && buffer[i - 1] != '\r'))
                stop = i;
            else
                stop = i - 1;

            return buffer.ToString (start, stop - start);
        }
        
        #endregion
    }
}
