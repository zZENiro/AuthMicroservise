using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationApp
{
    /*
     *  Все типы, которые работают с AuthenticationApp.AuthenticationManager 
     *  в качестве данных о пользователе должны быть наследниками 
     *  AuthenticationApp.IIdentifications
     */
    public interface IIdentifications
    {
        string Login { get; set; }

        string Password { get; set; }

        string Email { get; set; }

        string Role { get; set; }
    }
}
