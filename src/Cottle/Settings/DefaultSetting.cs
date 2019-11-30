namespace Cottle.Settings
{
    public sealed class DefaultSetting : ISetting
    {
        public static DefaultSetting Instance { get; } = new DefaultSetting();

        public string BlockBegin => "{";

        public string BlockContinue => "|";

        public string BlockEnd => "}";

        public char Escape => '\\';

        public bool Optimize => true;

        public Trimmer Trimmer
        {
            get { return t => t; }
        }
    }
}