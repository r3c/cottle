namespace Cottle.Documents.Compiled
{
	public readonly struct Symbol
	{
		public readonly int Index;
		public readonly StoreMode Mode;

		public Symbol(int index, StoreMode mode)
		{
			Index = index;
			Mode = mode;
		}
	}
}