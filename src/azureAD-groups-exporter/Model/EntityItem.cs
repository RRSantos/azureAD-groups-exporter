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
        
        public int NumberOfChildrenOverHierarchy { get; private set; }

        public EntityType Type { get; private set; }

        public List<EntityItem> Children { get; private set; }

        public void AddChild(EntityItem child)
        {
            NumberOfChildrenOverHierarchy++;
            Children.Add(child);
            NumberOfChildrenOverHierarchy += child.NumberOfChildrenOverHierarchy;
        }

        public EntityItem(string name, string id, EntityType type)
        {
            
            Name = name;
            Id = id;
            Type = type;
            Children = new List<EntityItem>();
            NumberOfChildrenOverHierarchy = 0;
        }  
    }
}
