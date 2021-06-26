using azureAD_groups_exporter.Model;
using CommandLine;
using OrgChart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;

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
                       GroupMemberService service = new GroupMemberService(o.TenantId, o.ClientId, o.ClientSecret);
                       var allEntities = service.GetAllGroupsAndMembers().Result;
                       stopwatch.Stop();
                       Console.WriteLine($"Elapsed time to get all groups am members: {stopwatch.Elapsed}");
                       
                       stopwatch.Restart();                       
                       ImageExporter exporter = new ImageExporter(allEntities);
                       exporter.Export(o.Filename);                       
                       Console.WriteLine($"Elapsed time to export all groups am members: {stopwatch.Elapsed}");
                   }); 
        }

                
    }
}
