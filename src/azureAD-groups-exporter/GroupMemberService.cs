using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Linq;
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

        private readonly List<Group>  groupsCache = new List<Group>();
        private readonly List<User> usersCache = new List<User>();

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

                    Group actualGroupWithMembers = groupsCache.FirstOrDefault( g => g.Id == actualGroup.Id);

                    if (actualGroupWithMembers == null)
                    {
                        IGraphServiceGroupsCollectionPage allGroups = await graphServiceClient
                        .Groups
                        .Request()
                        .Filter($"id eq '{actualGroup.Id}'")
                        .Expand("Members")
                        .GetAsync();

                        actualGroupWithMembers = allGroups.First();
                        groupsCache.Add(actualGroupWithMembers);
                    }                    

                    if (actualGroupWithMembers.Members != null)
                    {
                        await setAllChilds(actualGroupWithMembers, internalEntity);
                    }
                }
                //else if (m is User)
                //{
                //    User user = usersCache.FirstOrDefault(u => u.Id == m.Id);

                //    if (user == null)
                //    {
                //        IGraphServiceUsersCollectionPage allUsers = await graphServiceClient
                //        .Users
                //        .Request()
                //        .Filter($"id eq '{m.Id}'")
                //        .GetAsync();

                //        user = allUsers.First();
                //        usersCache.Add(user);
                //    }


                //    Model.EntityItem internalEntity = new Model.EntityItem(user.DisplayName, user.Id, Model.EntityType.User);
                //    entity.AddChild(internalEntity);
                //}
            }
        }

        public async Task<List<Model.EntityItem>> GetAllGroupsAndMembers()
        {
            var allGroups = await graphServiceClient
                .Groups
                .Request()
                .Expand("Members")
                //.Filter($"id eq '0061509b-ed0f-4344-bed4-3621e5e2ea48'")
                .GetAsync();

            List<Model.EntityItem> allEntities = new List<Model.EntityItem>();
            foreach (var group in allGroups)
            {
                groupsCache.Add(group);
                Model.EntityItem newEntity = new Model.EntityItem(group.DisplayName, group.Id, Model.EntityType.Group);
                
                await setAllChilds(group, newEntity);
                allEntities.Add(newEntity);
                
            }

            return allEntities;
        }



    }
}
