using CommandLine;
using System;

namespace azureAD_groups_exporter
{
    class Program
    {   
        static void Main(string[] args)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch(); 
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                   .WithParsed(o =>
                   {
                       stopwatch.Start();

                       Microsoft.Graph.IGraphServiceClient graphServiceClient = GraphServiceClientBuilder.Create(
                           o.TenantId,
                           o.ClientId,
                           o.ClientSecret);
                       
                       GroupMemberService service = new GroupMemberService(graphServiceClient);
                       var allEntities = service.GetAllGroupsAndMembers(o.ExportUsers).Result;
                       stopwatch.Stop();
                       Console.WriteLine($"Elapsed time to get all groups and its members: {stopwatch.Elapsed}");
                       
                       stopwatch.Restart();

                       HtmlExporter exporter = new HtmlExporter(o.OutputFolder);
                       exporter.Export(allEntities);
                       
                       Console.WriteLine($"Elapsed time to export all groups and its members: {stopwatch.Elapsed}");
                   }); 
        }

        


    }
}
