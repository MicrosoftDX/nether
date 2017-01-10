namespace Nether.Data.Sql.Identity
{
    public class UserEntity
    {
        public string UserId { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; } // allow multiple roles?

        // username, password are only used for resource owner password login flow, so consider moving to "login" entity
        public string UserName { get; set; }
        public string PasswordHash { get; set; }

        // Going forward, FacebookUserId should be stored as a "login" for the "facebook" provider to allow generalisation to other identity types
        public string FacebookUserId { get; set; }

    }
}