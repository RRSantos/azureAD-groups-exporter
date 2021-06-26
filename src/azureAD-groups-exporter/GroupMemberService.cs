using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace azureAD_groups_exporter
{
    class GroupMemberService
    {
        private const string BASE_URL = "https://login.microsoftonline.com";
        private const string SCOPE = "https://graph.microsoft.com/.default";
        private GraphServiceClient graphServiceClient;
        private readonly string tenantID;
        private readonly string clientId;
        private readonly string clientSecret;

        public GroupMemberService(string tenantID, string clientId, string clientSecret)
        {
            this.tenantID = tenantID;
            this.clientId = clientId;
            this.clientSecret = clientSecret;

            initializeInternalFields();
        }

        private void initializeInternalFields()
        {
            string instanceUrl = $"{BASE_URL}/{tenantID}";
            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(clientId)
                    .WithClientSecret(clientSecret)
                    .WithAuthority(new Uri(instanceUrl))
                    .Build();

            string[] scopes = new string[] { SCOPE };
            AuthenticationResult result = app.AcquireTokenForClient(scopes).ExecuteAsync().Result;

            graphServiceClient = new GraphServiceClient(new DelegateAuthenticationProvider((requestMessage) => {
                requestMessage
                    .Headers
                    .Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

                return Task.CompletedTask;
            }));
        }
        private async Task setAllChilds(Group group, Model.EntityItem entity)
        {
            foreach (var m in group.Members)
            {
                if (m is Group actualGroup)
                {
                    Model.EntityItem internalEntity = new Model.EntityItem(actualGroup.DisplayName, actualGroup.Id, Model.EntityType.Group);
                    entity.AddChild(internalEntity);

                    IGraphServiceGroupsCollectionPage allGroups = await graphServiceClient
                        .Groups
                        .Request()
                        .Filter($"id eq '{actualGroup.Id}'")
                        .Expand("Members")
                        .GetAsync();

                    Group actualGroupWithMembers = allGroups[0];

                    if (actualGroupWithMembers.Members != null)
                    {
                        await setAllChilds(actualGroupWithMembers, internalEntity);
                    }
                }
                else if (m is User)
                {
                    IGraphServiceUsersCollectionPage allUsers = await graphServiceClient
                        .Users
                        .Request()
                        .Filter($"id eq '{m.Id}'")
                        .GetAsync();

                    User user = allUsers[0];
                    //User user = allUsers.CurrentPage[0];

                    Model.EntityItem internalEntity = new Model.EntityItem(user.DisplayName, user.Id, Model.EntityType.User);
                    entity.AddChild(internalEntity);
                }
            }
        }

        public async Task<List<Model.EntityItem>> GetAllGroupsAndMembers()
        {
            var allGroups = await graphServiceClient.Groups.Request().Expand("Members").GetAsync();

            List<Model.EntityItem> allEntities = new List<Model.EntityItem>();
            foreach (var group in allGroups)
            {
                Model.EntityItem newEntity = new Model.EntityItem(group.DisplayName, group.Id, Model.EntityType.Group);
                await setAllChilds(group, newEntity);
                allEntities.Add(newEntity);
            }

            return allEntities;
        }



    }
}
