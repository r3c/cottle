using Cottle.Cleaners;

namespace Cottle.Settings
{
    public class    DefaultSetting : ISetting
    {
        #region Properties / Instance

        public string   BlockBegin
        {
            get
            {
                return "{";
            }
        }

        public string   BlockContinue
        {
            get
            {
                return "|";
            }
        }

        public string   BlockEnd
        {
            get
            {
                return "}";
            }
        }

        public ICleaner Cleaner
        {
            get
            {
                return DefaultSetting.cleaner;
            }
        }

        #endregion
        
        #region Properties / Static
        
        public static DefaultSetting    Instance
        {
            get
            {
                return DefaultSetting.instance;
            }
        }
        
        #endregion

        #region Attributes
        
        private static readonly NullCleaner     cleaner = new NullCleaner ();

        private static readonly DefaultSetting  instance = new DefaultSetting ();

        #endregion
    }
}
