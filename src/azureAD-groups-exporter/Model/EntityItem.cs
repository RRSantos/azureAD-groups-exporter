using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace azureAD_groups_exporter.Model
{
    class EntityItem
    {
        [JsonIgnore]
        public string Id { get; private set; }

        [JsonPropertyName("name")]
        public string Name { get; private set; }

        [JsonIgnore]
        public EntityType Type { get; private set; }

        [JsonPropertyName("title")]
        public string Title { get { return Type.ToString("f"); } }

        [JsonPropertyName("children")]
        public List<EntityItem> Children { get; private set; }

        public void AddChild(EntityItem child)
        {   
            Children.Add(child);
        }

        public EntityItem(string name, string id, EntityType type)
        {
            
            Name = name;
            Id = id;
            Type = type;
            Children = new List<EntityItem>();
        }  
    }
}
