using Microsoft.Graph;
using System.Linq;
using System.Collections.Generic;

using System.Threading.Tasks;

namespace azureAD_groups_exporter
{
    class GroupMemberService
    {
        private readonly IGraphServiceClient graphServiceClient;
        private readonly List<Group> groupsCache = new List<Group>();
        private readonly List<User> usersCache = new List<User>();

        public GroupMemberService(IGraphServiceClient graphServiceClient)
        {
            this.graphServiceClient = graphServiceClient;
        }

        private async Task<Model.EntityItem> convertToEntityItem(DirectoryObject directoryObject, bool exportUsers)
        {
            Model.EntityItem internalEntity = null;

            if (directoryObject is Group actualGroup)
            {
                internalEntity = new Model.EntityItem(
                    actualGroup.DisplayName,
                    actualGroup.Id,
                    actualGroup.Mail,
                    Model.EntityType.Group);

                Group actualGroupWithMembers = groupsCache.FirstOrDefault(g => g.Id == actualGroup.Id);

                if (actualGroupWithMembers == null)
                {
                    actualGroupWithMembers = await getGroupFromAD(actualGroup.Id);
                    groupsCache.Add(actualGroupWithMembers);
                }

                if (actualGroupWithMembers.Members != null)
                {
                    await addAllChildrenForGroup(actualGroupWithMembers, internalEntity, exportUsers);
                }
                
            }
            else if ((directoryObject is User) && exportUsers)
            {
                User user = usersCache.FirstOrDefault(u => u.Id == directoryObject.Id);

                if (user == null)
                {
                    user = await getUserFromAD(directoryObject.Id);
                    usersCache.Add(user);
                }

                internalEntity = new Model.EntityItem(
                    user.DisplayName,
                    user.Id,
                    user.Mail,
                    Model.EntityType.User);
            }

            return internalEntity;

        }

        private async Task<Group> getGroupFromAD(string groupId)
        {
            IGraphServiceGroupsCollectionPage allGroups = await graphServiceClient
                    .Groups
                    .Request()
                    .Filter($"id eq '{groupId}'")
                    .Expand("Members")
                    .GetAsync();

            return allGroups.First();
        }

        private async Task<User> getUserFromAD(string userID)
        {
            IGraphServiceUsersCollectionPage allUsers = await graphServiceClient
                    .Users
                    .Request()
                    .Filter($"id eq '{userID}'")
                    .GetAsync();

            return allUsers.First();
        }

        private async Task addAllChildrenForGroup(Group group, Model.EntityItem entity, bool exportUsers)
        {
            foreach (var m in group.Members)
            {
                Model.EntityItem internalEntity = await convertToEntityItem(m, exportUsers);
                if (internalEntity != null)
                {
                    entity.AddChild(internalEntity);
                }
            }
        }

        public async Task<List<Model.EntityItem>> GetAllGroupsAndMembers(bool exportUsers)
        {
            var allGroups = await graphServiceClient
                .Groups
                .Request()
                .Expand("Members")
                .GetAsync();

            List<Model.EntityItem> allEntities = new List<Model.EntityItem>();
            foreach (var group in allGroups)
            {
                if (!groupsCache.Exists(e => e.Id == group.Id))
                {
                    groupsCache.Add(group);
                    Model.EntityItem nextEntity = new Model.EntityItem(
                        group.DisplayName,
                        group.Id,
                        group.Mail,
                        Model.EntityType.Group);
                    await addAllChildrenForGroup(group, nextEntity, exportUsers);
                    
                    allEntities.Add(nextEntity);
                }

            }

            return allEntities;
        }



    }
}
