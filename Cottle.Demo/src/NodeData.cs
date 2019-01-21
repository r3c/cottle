namespace Cottle.Demo
{
    public class NodeData
    {
        #region Constructors

        public NodeData(string key, Value value)
        {
            Key = key;
            Value = value;
        }

        #endregion

        #region Properties

        public int ImageIndex => (int) Value.Type;

        public string Key { get; }

        public Value Value { get; }

        #endregion

        #region Attributes

        #endregion
    }
}