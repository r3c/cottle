namespace Cottle.Demo
{
    public class NodeData
    {
        public int ImageIndex => (int)Value.Type;

        public string Key { get; }

        public Value Value { get; }

        public NodeData(string key, Value value)
        {
            Key = key;
            Value = value;
        }
    }
}