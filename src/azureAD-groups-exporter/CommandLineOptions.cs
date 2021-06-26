using CommandLine;

namespace azureAD_groups_exporter
{
    class CommandLineOptions
    {
        [Option('t', "tenantId", Required = true, HelpText = "Tenant Id from where you want to get information.")]
        public string TenantId { get; set; }

        [Option('c', "clientId", Required = true, HelpText = "Client Id of application with required permissions in your Azure AD.")]
        public string ClientId { get; set; }

        [Option('s', "clientSecret", Required = true, HelpText = "Client secret defined for application defined in 'clientId' parameter.")]
        public string ClientSecret { get; set; }

        [Option('f', "filename", Required = false, Default = "azureAD-group-export.png", HelpText = "Name of image file (.png extension) to export azureAD-group-exporter results.")]
        public string Filename { get; set; }
    }
}
