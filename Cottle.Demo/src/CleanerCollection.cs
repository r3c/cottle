using System;
using System.Collections.Generic;

using Cottle.Cleaners;

namespace	Cottle.Demo
{
	public static class CleanerCollection
	{
		#region Constants
		
		private const int	DEFAULT_INDEX = 2;
		
		#endregion

		#region Properties
		
		public static IEnumerable<KeyValuePair<string, ICleaner>>	Cleaners
		{
			get
			{
				return CleanerCollection.cleaners;
			}
		}
		
		#endregion

		#region Attributes

		private static readonly KeyValuePair<string, ICleaner>[]	cleaners = new KeyValuePair<string, ICleaner>[]
		{
			new KeyValuePair<string, ICleaner> ("Blank characters",	 new BlankCharactersCleaner ()),
			new KeyValuePair<string, ICleaner> ("First and last lines", new FirstLastLinesCleaner ()),
			new KeyValuePair<string, ICleaner> ("Do not clean",		 new NullCleaner ())
		};

		#endregion
		
		#region Methods
		
		public static ICleaner	GetCleaner (int index)
		{
			if (index >= 0 && index < CleanerCollection.cleaners.Length)
				return CleanerCollection.cleaners[index].Value;

			return CleanerCollection.cleaners[CleanerCollection.DEFAULT_INDEX].Value;
		}
		
		public static int	GetIndex (ICleaner cleaner)
		{
			for (int i = 0; i < CleanerCollection.cleaners.Length; ++i)
			{
				if (object.ReferenceEquals (cleaner, CleanerCollection.cleaners[i].Value))
					return i;
			}

			return CleanerCollection.DEFAULT_INDEX;
		}
		
		#endregion
	}
}
