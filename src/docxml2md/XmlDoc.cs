using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DocXml2md
{
	public class XmlDoc
	{
		public string AssemblyName { get; private set; }
		public XmlDoc(XDocument xml)
		{
			AssemblyName = xml.Root.Element("assembly").Element("name").Value;
			IEnumerable<XElement> members = xml.Root.Element("members").Elements("member");
			Members = new List<XmlMember>();
			foreach (var xElement in members)
			{
				Members.Add(new XmlMember(xElement));
			}
			FixWithTypeParentUncomment();
		}

		private void FixWithTypeParentUncomment()
		{
			var allTypeComment = Members.Where(x => x.MemberType == MemberType.Type).ToDictionary(k => k.FullName, v => v);
			var newTypeMembers = new Dictionary<string,XmlMember>();
			foreach (var element in Members)
			{
				if (!allTypeComment.ContainsKey(element.ParentTypeName) && !newTypeMembers.ContainsKey(element.ParentTypeName))
				{
					newTypeMembers.Add(element.ParentTypeName,new XmlMember(MemberType.Type, element.ParentTypeName, element.ParentTypeName));
				}
				
			}
			Members.AddRange(newTypeMembers.Select(s=>s.Value).ToList());
		}

		public List<XmlMember> Members { get; set; }


	}
}