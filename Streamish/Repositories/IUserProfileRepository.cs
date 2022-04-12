using Streamish.Models;
using System.Collections.Generic;

namespace Streamish.Repositories
{
    public interface IUserProfileRepository
    {
        void Add(UserProfile userProfile);
        void Delete(int id);
        List<UserProfile> GetAllUsers();
        UserProfile GetUser(int id);
        void Update(UserProfile userProfile);
    }
}