[![Windows ci build](https://github.com/RRSantos/azureAD-groups-exporter/actions/workflows/CI-Windows.yml/badge.svg)](https://github.com/RRSantos/azureAD-groups-exporter/actions/workflows/CI-Windows.yml)
[![Linux ci build](https://github.com/RRSantos/azureAD-groups-exporter/actions/workflows/CI-Ubuntu.yml/badge.svg)](https://github.com/RRSantos/azureAD-groups-exporter/actions/workflows/CI-Ubuntu.yml)
[![Release](https://github.com/RRSantos/azureAD-groups-exporter/actions/workflows/release.yml/badge.svg)](https://github.com/RRSantos/azureAD-groups-exporter/actions/workflows/release.yml)

# azureAD-groups-exporter
Simple CLI tool to export all Azure AD groups and its members to a single HTML page using [dabeng's OrgChart](https://github.com/dabeng/OrgChart)

## Getting started
### Setup Azure pre-requsites
1. Access [Azure portal](https://portal.azure.com/), switch to Directory (tenant) you want to export Groups and **write down** its ``Directory ID``;
2. [Register an Azure Application in Azure Active Directory](https://docs.microsoft.com/en-us/graph/auth-register-app-v2) and **write down** its ``Application ID`` value;
3. [Set Microsoft.Graph's  **Application permissions**](https://docs.microsoft.com/en-us/graph/auth-v2-service#2-configure-permissions-for-microsoft-graph) ``Group.Read.All`` and ``User.Read.All`` to application created on step #2;
4. [Get administrator consent](https://docs.microsoft.com/en-us/graph/auth-v2-service#3-get-administrator-consent) for the two permissions added on step #3 ;
5. [Generate a client secret](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app#add-a-client-secret) for the application created on step #2  and **write down** its ``Value`` property;

### Download the app
- Todo

### Run the app
To export export groups from Azure Active Directory from your directory, use the command line below.

```
azureAD-groups-exporter.exe --directoryID <directory-ID> --clientID <client-ID> --clientSecret <client-secret> --outputFolder <folder/to/output> --exportUsers
```

|Parameter|Required|Description|Default value| 
|--|--|--|--| 
|directoryID| yes | Directory (tenant) ID from where you want to export Azure AD groups| N/A|
|clientID| yes | Client (application) ID from Application registered on step #2 from [Setup Azure pre-requisites](#Setup-Azure-pre-requisites) section| N/A|
|clientSecret| yes | Client Secret generated on step #4 from [Setup Azure pre-requisites](#Setup-Azure-pre-requisites) section| N/A|
|outputFOlder| no | Folder to save the HTML file and its dependencies as a result of exporting process | export/|
|exportUsers| no | Use this flag (True) to export Users as members from groups | False|

#### Examples
1. Exporting **groups and users** to folder ``C:\temp\myExportResult``:
  - Tenant ID : `693ba13e-f7ab-4bc1-8cee-edd404e8050c`;
  - Client ID : `6d8bf485-d38a-4e14-a5ac-dd278351d2f5`;
  - Client Secret : `W3VvnNW_10yy~aa~wO0.5bB.jj~0NnIiZ9`;

```
azureAD-groups-exporter.exe --directoryID 693ba13e-f7ab-4bc1-8cee-edd404e8050c --clientID 6d8bf485-d38a-4e14-a5ac-dd278351d2f5 --clientSecret W3VvnNW_10yy~aa~wO0.5bB.jj~0NnIiZ9 --outputFolder "C:\temp\myExportResult" --exportUsers
```

2. Exporting **only groups** to folder ``/home/user/myExportResult``:
  - Tenant ID : `693ba13e-f7ab-4bc1-8cee-edd404e8050c`;
  - Client ID : `6d8bf485-d38a-4e14-a5ac-dd278351d2f5`;
  - Client Secret : `W3VvnNW_10yy~aa~wO0.5bB.jj~0NnIiZ9`;

```
azureAD-groups-exporter.exe --directoryID 693ba13e-f7ab-4bc1-8cee-edd404e8050c --clientID 6d8bf485-d38a-4e14-a5ac-dd278351d2f5 --clientSecret W3VvnNW_10yy~aa~wO0.5bB.jj~0NnIiZ9 --outputFolder "/home/user/myExportResult"
```

