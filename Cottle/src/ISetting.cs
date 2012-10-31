
namespace  Cottle
{
    public interface    ISetting
    {
        #region Methods
        
        string          BlockBegin
        {
            get;
        }

        string          BlockContinue
        {
            get;
        }

        string          BlockEnd
        {
            get;
        }

        SettingClean    Clean
        {
            get;
        }
        
        #endregion
    }
}
