namespace Cottle.Settings
{
    public sealed class DefaultSetting : ISetting
    {
        #region Properties / Static

        public static DefaultSetting Instance { get; } = new DefaultSetting();

        #endregion

        #region Properties / Instance

        public string BlockBegin => "{";

        public string BlockContinue => "|";

        public string BlockEnd => "}";

        public char Escape => '\\';

        public bool Optimize => true;

        public Trimmer Trimmer
        {
            get { return t => t; }
        }

        #endregion
    }
}