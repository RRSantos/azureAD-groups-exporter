using CommandLine;

namespace azureAD_groups_exporter
{
    class CommandLineOptions
    {
        [Option('d', "directoryID", Required = true, HelpText = "Directory (tenant) ID from where you want to get information.")]
        public string DirectoryId { get; set; }

        [Option('c', "clientID", Required = true, HelpText = "Client ID of application with required permissions in your Azure AD.")]
        public string ClientId { get; set; }

        [Option('s', "clientSecret", Required = true, HelpText = "Client secret defined for application defined in 'clientId' parameter.")]
        public string ClientSecret { get; set; }

        [Option('o', "outputFolder", Required = false, Default = "export/", HelpText = "Folder to export HTML/JS/CSS files.")]
        public string OutputFolder { get; set; }

        [Option('u', "exportUsers", Required = false, Default = false, HelpText = "Export users as members (true) or only groups (false).")]
        public bool ExportUsers { get; set; }
    }
}
