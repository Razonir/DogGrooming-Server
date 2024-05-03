using System.ComponentModel.DataAnnotations;

namespace DogGrooming_Server.Data.Models
{
    public class Login
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
