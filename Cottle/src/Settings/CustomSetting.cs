namespace Cottle.Settings
{
    public sealed class CustomSetting : ISetting
    {
        public string BlockBegin { get; set; } = DefaultSetting.Instance.BlockBegin;

        public string BlockContinue { get; set; } = DefaultSetting.Instance.BlockContinue;

        public string BlockEnd { get; set; } = DefaultSetting.Instance.BlockEnd;

        public char Escape { get; set; } = DefaultSetting.Instance.Escape;

        public bool Optimize { get; set; } = DefaultSetting.Instance.Optimize;

        public Trimmer Trimmer { get; set; } = DefaultSetting.Instance.Trimmer;
    }
}