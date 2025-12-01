using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
git remote add origin https://github.com/KamalElsayedJR/AuthModule.git
git branch -M main
git push -u origin main
*/
namespace CORE.Entities
{
    public class User
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string HashPassword { get; set; }
        public bool IsVerified { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public User()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
