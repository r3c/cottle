using System.IO;

namespace Cottle
{
    internal interface IParser
    {
        Command Parse(TextReader reader);
    }
}