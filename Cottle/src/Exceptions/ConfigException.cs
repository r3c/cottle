using System;
using System.Globalization;

namespace	Cottle.Exceptions
{
	public class	ConfigException : Exception
	{
		#region Properties

		public string	Name
		{
			get
			{
				return this.name;
			}
		}

		public string	Value
		{
			get
			{
				return this.value;
			}
		}

		#endregion

		#region Attributes

		private string	name;

		private string	value;

		#endregion

		#region Constructors

		public	ConfigException (string name, string value, string message) :
			base (string.Format (CultureInfo.InvariantCulture, "configuration error for option '{0}' with value '{1}' ({2})'", name, value, message))
		{
			this.name = name;
			this.value = value;
		}

		#endregion
	}
}
