using azureAD_groups_exporter.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace azureAD_groups_exporter
{
    class HtmlExporter
    {
        private const string TEMPLATE_ROOT_FOLDER = "Template";
        private const string JS_DATA_FILE_NAME = "data.json";
        private const string JS_FOLDER_NAME = "js";
        private const string HTML_MAIN_FILE = "azureAD-group-exporter-results.html";
        private readonly string outputPath;

        private string[] getCssAndJsFileNamesToCopy()
        {
            string[] allFiles = Directory
                .GetFiles(TEMPLATE_ROOT_FOLDER, "*.*", SearchOption.AllDirectories)
                .Where(f => f.EndsWith(".css") || f.EndsWith(".js"))
                .ToArray();

            return allFiles;
        }

        private void copyTemplateFilesToOutput()
        {
            Directory.CreateDirectory(outputPath);
            

            string[] filesToCopy = getCssAndJsFileNamesToCopy();

            foreach (string file in filesToCopy)
            {
                copyFileToDestination(file);
            }
            copyHtmlFileToOutput();
        }

        private void copyHtmlFileToOutput()
        {
            FileInfo htmlFile = new FileInfo($"{TEMPLATE_ROOT_FOLDER}/{HTML_MAIN_FILE}");
            htmlFile.CopyTo($"{outputPath}/{htmlFile.Name}", true);
        }

        private void copyFileToDestination(string fileName)
        {
            FileInfo file = new FileInfo(fileName);
            string targetFolder = $"{outputPath}/{file.Directory.Name}";
            Directory.CreateDirectory(targetFolder);
            file.CopyTo($"{outputPath}/{file.Directory.Name}/{file.Name}", true);
        }

        private void createDataFile(IEnumerable<EntityItem> allEntities)
        {
            string allEntityJSON = JsonSerializer.Serialize(
                            new
                            {
                                name = "root",
                                title = "root node",
                                email = "This is the root node",
                                children = allEntities
                            });

            string outputDataPath = $"{outputPath}/{JS_FOLDER_NAME}";
            Directory.CreateDirectory(outputDataPath);

            File.WriteAllText($"{outputDataPath}/{JS_DATA_FILE_NAME}", $"datasource = {allEntityJSON}");
        }

        public HtmlExporter(string outputPath)
        {
            this.outputPath = outputPath;
        }

        public void Export(IEnumerable<EntityItem> allEntities)
        {
            copyTemplateFilesToOutput();
            createDataFile(allEntities);

        }
        
    }
}
