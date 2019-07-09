namespace Cottle
{
    public interface ISetting
    {
        string BlockBegin { get; }

        string BlockContinue { get; }

        string BlockEnd { get; }

        char Escape { get; }

        bool Optimize { get; }

        Trimmer Trimmer { get; }
    }
}