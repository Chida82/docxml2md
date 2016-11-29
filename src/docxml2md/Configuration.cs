using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocXml2md
{
	public static class Configuration
    {
		public static readonly Dictionary<string, MemberType> MemberTypeDictionary = new Dictionary<string, MemberType>(StringComparer.OrdinalIgnoreCase)
			{
				{"F:", MemberType.Field},
				{"P:", MemberType.Property},
				{"T:", MemberType.Type},
				{"E:", MemberType.Event},
				{"M:", MemberType.Method}
			};


	}
}
