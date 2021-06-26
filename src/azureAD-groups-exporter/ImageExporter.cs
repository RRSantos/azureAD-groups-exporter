using OrgChart;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azureAD_groups_exporter
{
    class ImageExporter
    {
        private OrgChartOption defaultOption;
        private OrgChartOption option;
        private readonly IEnumerable<Model.EntityItem> entitiesToExport;

        private void initializeInternalFields()
        {
            defaultOption = new OrgChartOption
            {
                BoxFillColor = ColorTranslator.FromHtml("#A7E7FC"),
                BoxBorderColor = ColorTranslator.FromHtml("#A7E7FC"),
                ConnectLineColor = ColorTranslator.FromHtml("#424242")
            };

            option = new OrgChartOption()
            {
                BoxFillColor = defaultOption.BoxFillColor,
                BoxBorderColor = defaultOption.BoxBorderColor,
                ConnectLineColor = defaultOption.ConnectLineColor,
                FontSize = 15,
                BoxHeight = 100,
                BoxWidth = 230,
                FontName = "Calibri",
            };
        }
        public ImageExporter(IEnumerable<Model.EntityItem> entitiesToExport)
        {
            this.entitiesToExport = entitiesToExport;
            initializeInternalFields();

        }

        public void Export(string imageFilename)
        {
            using (OrgChartGenerator orgChartGenerator = getOrgChartGenerator())
            {   
                using (FileStream fs = File.Create(imageFilename))
                {
                    MemoryStream ms = orgChartGenerator.Generate();
                    ms.WriteTo(fs);
                    fs.Flush();
                };
            }
        }

        private OrgChartGenerator getOrgChartGenerator()
        {
            var groupsOrdersBySize = entitiesToExport.OrderByDescending(e => e.NumberOfChildrenOverHierarchy);
            List<OrgChartNode> allExportedGroupsAndUsers = getOrgChartNodes(groupsOrdersBySize);
            OrgChartNode rootNode = new OrgChartNode("1", "Root", allExportedGroupsAndUsers.ToArray());
            List<OrgChartNode> root = new List<OrgChartNode>();
            root.Add(rootNode);
            OrgChartGenerator orgChartGenerator = new OrgChartGenerator(root, option) { DefaultOption = defaultOption };

            return orgChartGenerator;
        }

        private  List<OrgChartNode> getOrgChartNodes(IEnumerable<Model.EntityItem> allEntities)
        {
            List<OrgChartNode> nodes = new List<OrgChartNode>();

            foreach (Model.EntityItem entity in allEntities)
            {
                List<OrgChartNode> childNodes = new List<OrgChartNode>();
                if (entity.Children.Count > 0)
                {
                    childNodes.AddRange(getOrgChartNodes(entity.Children));
                }
                OrgChartNode actualNode = new OrgChartNode(entity.Id, $"{entity.Name}\n({entity.Type})", childNodes.ToArray());

                nodes.Add(actualNode);
            }

            return nodes;
        }
    }
}
