
namespace Cottle.Settings
{
    public class    CustomSetting : ISetting
    {
        #region Properties

        public string   BlockBegin
        {
            get
            {
                return this.blockBegin;
            }
            set
            {
                this.blockBegin = value;
            }
        }

        public string   BlockContinue
        {
            get
            {
                return this.blockContinue;
            }
            set
            {
                this.blockContinue = value;
            }
        }

        public string   BlockEnd
        {
            get
            {
                return this.blockEnd;
            }
            set
            {
                this.blockEnd = value;
            }
        }

        public ICleaner Cleaner
        {
            get
            {
                return this.cleaner;
            }
            set
            {
                this.cleaner = value;
            }
        }

        #endregion

        #region Attributes

        private string     blockBegin = DefaultSetting.Instance.BlockBegin;

        private string     blockContinue = DefaultSetting.Instance.BlockContinue;

        private string     blockEnd = DefaultSetting.Instance.BlockEnd;

        private ICleaner   cleaner = DefaultSetting.Instance.Cleaner;

        #endregion
    }
}
