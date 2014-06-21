using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using Cottle.Builtins;
using Cottle.Documents;
using Cottle.Functions;
using Cottle.Stores;
using Cottle.Values;

namespace Cottle.Commons
{
	[Obsolete ("Use Cottle.Builtins.BuiltinFunctions")]
	public static class CommonFunctions
	{
		#region Properties

		public static bool	IncludeCacheEnable
		{
			get
			{
				return CommonFunctions.includeCacheEnable;
			}
			set
			{
				CommonFunctions.includeCacheEnable = value;
			}
		}

		public static int	IncludeCacheSize
		{
			get
			{
				return CommonFunctions.includeCacheSize;
			}
			set
			{
				CommonFunctions.includeCacheSize = value;
			}
		}

		#endregion

		#region Attributes

		private static readonly IFunction	functionInclude = new NativeFunction ((values, store, output) =>
		{
			IDocument	document;
			object		entry;
			IStore		inner;
			string		path;
			DateTime	write;

			path = Path.GetFullPath (values[0].AsString);

			if (!File.Exists (path))
				return VoidValue.Instance;

			write = File.GetLastWriteTime (path);

			lock (CommonFunctions.includeCache)
			{
				if (CommonFunctions.includeCacheEnable)
					entry = CommonFunctions.includeCache[path];
				else
					entry = null;

				if (entry != null && ((KeyValuePair<IDocument, DateTime>)entry).Value >= write)
					document = ((KeyValuePair<IDocument, DateTime>)entry).Key;
				else
				{
					using (FileStream stream = File.OpenRead (path))
					{
						document = new SimpleDocument (new StreamReader (stream));
					}

					CommonFunctions.includeCache[path] = new KeyValuePair<IDocument, DateTime> (document, write);

					while (CommonFunctions.includeCache.Count > CommonFunctions.includeCacheSize && CommonFunctions.includeCache.Count > 0)
						CommonFunctions.includeCache.RemoveAt (0);
				}
			}

			inner = new FallbackStore (store, new SimpleStore ());

			for (int i = 1; i < values.Count; ++i)
			{
				foreach (KeyValuePair<Value, Value> pair in values[i].Fields)
					inner.Set (pair.Key, pair.Value, StoreMode.Global);
			}

			return document.Render (inner, output);
		}, 1, -1);

		private static readonly OrderedDictionary	includeCache = new OrderedDictionary ();
		
		private static bool							includeCacheEnable = true;

		private static int							includeCacheSize = 256;

		#endregion

		#region Methods

		public static void Assign (IScope scope, ScopeMode mode = ScopeMode.Closest)
		{
			foreach (KeyValuePair<string, IFunction> pair in BuiltinFunctions.Instances)
				scope.Set (pair.Key, new FunctionValue (pair.Value), mode);

			scope.Set ("include", new FunctionValue (CommonFunctions.functionInclude), mode);
		}

		#endregion
	}
}
