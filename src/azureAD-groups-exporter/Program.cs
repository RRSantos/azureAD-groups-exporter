using azureAD_groups_exporter.Model;
using CommandLine;
using System;
using System.Collections.Generic;


namespace azureAD_groups_exporter
{
    class Program
    {   
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                   .WithParsed(o =>
                   {
                       GroupMemberService service = new GroupMemberService(o.TenantId, o.ClientId, o.ClientSecret);
                       var allEntities = service.GetAllGroupsAndMembers().Result;
                       Display(allEntities);
                   }); 
        }

        private static void Display(List<EntityItem> allEntities, int level =0)
        {
            string spaceLevel = new string(' ', level*4);

            foreach (EntityItem entity in allEntities)
            {   
                Console.WriteLine($"{spaceLevel}{entity.Type}: {entity.Name} ({entity.Id})");
                Display(entity.Childs, level + 1);                
            }
        }
    }
}
