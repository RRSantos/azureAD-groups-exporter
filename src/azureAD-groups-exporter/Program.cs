using azureAD_groups_exporter.Model;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Text.Json;

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
                       
                       exportHTML(allEntities, o.OutputFolder);
                       Console.WriteLine($"Elapsed time to export all groups and its members: {stopwatch.Elapsed}");
                   }); 
        }

        static void exportHTML(IEnumerable<EntityItem> allEntities, string exportFolder)
        {   
            //Todo: Do not export group if it has parent
            //Todo: Move to specialized class

            Directory.CreateDirectory(exportFolder);
            
            FileInfo htmlFile = new FileInfo("Template/azureAD-group-exporter-results.html");
            htmlFile.CopyTo($"{exportFolder}/{htmlFile.Name}", true);
            
            string[] allFiles = Directory
                .GetFiles("Template", "*.*", SearchOption.AllDirectories)
                .Where( f => f.EndsWith(".css") || f.EndsWith(".js"))
                .ToArray();

            foreach (string f in allFiles)
            {
                FileInfo file = new FileInfo(f);
                string targetFolder = $"{exportFolder}/{file.Directory.Name}";
                Directory.CreateDirectory(targetFolder);
                file.CopyTo($"{exportFolder}/{file.Directory.Name}/{file.Name}", true);
            }

            string allEntityJSON = JsonSerializer.Serialize(new { name = "root", title = "root node", children = allEntities });
            File.WriteAllText($"{exportFolder}/js/data.json", $"datasource = {allEntityJSON}");

            
        }


    }
}
