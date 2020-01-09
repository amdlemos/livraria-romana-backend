using LivrariaRomana.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LivrariaRomana.Test.DataBuilder
{
    public class UserBuilder
    {
        private User user;
        private List<User> userList;
       
        public User CreateUser()
        {
            user = new User() { 
                Username = "User from Builder",
                Email = "user@builder.com",
                Password = "123"
            };
            return user;
        }

        public List<User> CreateUserList(int amount)
        {
            userList = new List<User>();
            for (int i = 0; i < amount; i++)
            {
                userList.Add(CreateUser());
            }

            return userList;
        }
    }
}
