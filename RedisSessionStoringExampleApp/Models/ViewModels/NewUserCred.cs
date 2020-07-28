using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisSessionStoringExampleApp.Models.ViewModels
{
    public class NewUserCred
    {
        public string Login { get; set; }

        public string Password { get; set; }
    }
}
