using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azureAD_groups_exporter.Model
{
    class EntityItem
    {
        public string Name { get; private set; }
        public string Id { get; private set; }

        public string Type { get; private set; }

        public List<EntityItem> Childs { get; private set; }

        public void AddChild(EntityItem child)
        {
            Childs.Add(child);
        }

        public EntityItem(string name, string id, string type)
        {
            Name = name;
            Id = id;
            Type = type;
            Childs = new List<EntityItem>();
            
        }  
    }
}
