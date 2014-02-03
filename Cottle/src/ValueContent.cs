using System;

namespace	Cottle
{
	public enum ValueContent
	{
		Boolean		= 1,
		Function	= 2,
		Map			= 0,
		Number		= 3,
		String		= 4,
		Void		= 5,

		[Obsolete ("Use ValueContent.Map")]
		Array		= 0,

		[Obsolete ("Use ValueContent.Void")]
		Undefined	= 5
	}
}
