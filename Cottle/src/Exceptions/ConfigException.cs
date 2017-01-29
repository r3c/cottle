using System;
using System.Globalization;

namespace Cottle.Exceptions
{
	public class ConfigException : Exception
	{
		#region Properties

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		#endregion

		#region Attributes

		private readonly string name;

		private readonly string value;

		#endregion

		#region Constructors

		public ConfigException (string name, string value, string message) :
			base (string.Format (CultureInfo.InvariantCulture, "{2} (option '{0}', value '{1}')", name, value, message))
		{
			this.name = name;
			this.value = value;
		}

		#endregion
	}
}
