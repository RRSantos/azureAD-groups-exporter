using CommandLine;
using System;

namespace azureAD_groups_exporter
{
    class Program
    {   
        static void Main(string[] args)
        {
            //Todo: Add loggers to console/file
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                   .WithParsed(o =>
                   {
                       Microsoft.Graph.IGraphServiceClient graphServiceClient = GraphServiceClientBuilder.Create(
                           o.DirectoryId,
                           o.ClientId,
                           o.ClientSecret);
                       
                       GroupMemberService service = new GroupMemberService(graphServiceClient);
                       Console.WriteLine("Getting groups and its members from Azure AD...");
                       var allEntities = service.GetAllGroupsAndMembers(o.ExportUsers).Result;
                       Console.WriteLine($"Found {allEntities.Count} root group(s) to export.");

                       Console.WriteLine("Exporting item(s) to HTML...");
                       HtmlExporter exporter = new HtmlExporter(o.OutputFolder);
                       exporter.Export(allEntities);
                       
                       Console.WriteLine($"Items successfully exported to '{o.OutputFolder}' folder!");
                   }); 
        }

        


    }
}
