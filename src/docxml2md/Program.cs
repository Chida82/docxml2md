using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.Extensions.CommandLineUtils;

namespace DocXml2md
{
    public class Program
    {
        public static int Main(string[] args)
        {
			var app = new CommandLineApplication
			{
				Name = "docxml2md",
				FullName = "Doc To Markdown",
				Description = "Convert xml documentation .net into markdown format"
			};
			app.HelpOption("-?|-h|--help");
			var inputFile = app.Option("-i|--inputfile", "Input xml file to read.", CommandOptionType.SingleValue);
			var outputPath = app.Option("-o|--outputpath", "Root md files to write.", CommandOptionType.SingleValue);

			app.OnExecute(() =>
			{
				if (!inputFile.HasValue() || !outputPath.HasValue())
				{
					//todo: add log
					app.ShowHelp();
					return (int)ExitCode.NoMinimalArgument;
				}
				try
				{
					using (var fs = new FileStream(inputFile.Value(), FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						var xml = XDocument.Load(fs);
						var xmlDoc = new XmlDoc(xml);
						var mdConverter =  new MdConverter(Path.GetFullPath(outputPath.Value()));
						mdConverter.Convert(xmlDoc);

					}
					return (int)ExitCode.Success;

				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
					Console.ReadKey();
					return (int)ExitCode.UnknownError;
				}
			});


			return app.Execute(args);
		}
    }
}
