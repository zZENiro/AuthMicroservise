using AuthenticationApp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RedisSessionStoringExampleApp.Models
{
    public class User : IIdentifications
    {
        [Key] public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }


}
