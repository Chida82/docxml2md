using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace DocXml2md
{
	public class XmlMember
	{
		public XmlMember(MemberType memberType, string fullName, string parentTypeName)
		{
			MemberType = memberType;
			FullName = fullName;
			ParentTypeName = parentTypeName;
		}
		public XmlMember(XElement xElement)
		{
			Body = xElement;
			var name = xElement.Attribute("name").Value;
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Attribute name is empty");
			var split = name.Split(new char[1] { ':' }, 2);
			MemberType type;
			if (!Configuration.MemberTypeDictionary.TryGetValue($"{split[0]}:", out type))
				throw new ArgumentException("Type not know for {0}", $"{split[0]}:");
			MemberType = type;
			FullName = split[1];
			if (MemberType == MemberType.Type)
			{
				ParentTypeName = FullName;
			}
			else
			{
				var i = FullName.IndexOf('(');
				ParentTypeName = FullName.Substring(0, i>0 ? FullName.LastIndexOf('.', i) : FullName.LastIndexOf('.'));
			}
		}

		public XElement Body { get; set; }
		public string ParentTypeName { get; set; }
		public string FullName { get; set; }

		public string RelativeName
		{
			get { return FullName.Substring(ParentTypeName.Length + 1); }
		}

		public MemberType MemberType { get; set; }

		public StringBuilder Render()
		{
			var sb = new StringBuilder();
			sb.AppendFormat("### {0}\n\n", RelativeName);
			var paramsHeader = "#### Parameters\n\n|Name | Description |\n|---|---|\n";
			var typeparamHeader = "#### TypeParameters\n\n|Name | Description |\n|---|---|\n";
			foreach (var xElement in Body.Elements())
			{
				switch (xElement.Name.LocalName.ToLower())
				{
					case "summary":
						sb.AppendFormat("{0}\n\n", ClearString(xElement.Value));
						break;
					case "param":
						sb.Append(paramsHeader).AppendFormat("|**{0}** |{1}|\n", xElement.Attribute("name").Value, ClearString(xElement.Value));
						paramsHeader = string.Empty;
						break;
					case "typeparam":
						sb.Append(typeparamHeader).AppendFormat("|**{0}** |{1}|\n", xElement.Attribute("name").Value, ClearString(xElement.Value));
						typeparamHeader = string.Empty;
						break;
					case "code":
					case "c":
						sb.AppendFormat("\n###### {0} code\n\n```{0} \n{1}\n```\n\n", xElement.Attribute("lang")?.Value ?? "c#", ClearCode(xElement.Value));
						break;
					default:
						//todo: add log
						break;
				}
			}
			return sb ;
		}

		private StringBuilder ClearString(string s)
		{
			var lines = s.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
			var sb = new StringBuilder();
			for (int i = 0; i < lines.Length; i++)
			{
				sb.Append(lines[i].Trim());
			}
			return sb;
		}

		private StringBuilder ClearCode(string s)
		{
			var lines = s.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
			var sb = new StringBuilder();
			for (int i = 0; i < lines.Length; i++)
			{
				sb.Append(lines[i].TrimEnd());
			}
			return sb;
		}
	}
}