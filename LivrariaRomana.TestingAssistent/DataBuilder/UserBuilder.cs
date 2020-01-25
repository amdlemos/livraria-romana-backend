using LivrariaRomana.Domain.Entities;
using System.Collections.Generic;

namespace LivrariaRomana.TestingAssistent.DataBuilder
{
    public class UserBuilder
    {
        private User user;
        private List<User> userList;

        public User CreateUser()
        {
            user = new User("user", "123", "user@builder.com");            
            return user;
        }

        public User CreateUserWithEmptyUsername()
        {
            user = new User("", "123", "user@builder.com", "");
            return user;
        }

        public User CreateUserWithNullUsername()
        {
            user = new User(null, "123", "user@builder.com", "");
            return user;
        }


        public User CreateUserWithNullPassword()
        {
            user = new User("User from Builder", null, "user@builder.com");
            return user;
        }

        public User CreateUserWithEmptyPassword()
        {
            user = new User("User from Builder", "", "user@builder.com");
            return user;
        }

        public User CreateUserWithNullEmail()
        {
            user = new User("User from Builder","123" , null);
            return user;
        }

        public User CreateUserWithEmptyEmail()
        {
            user = new User("User from Builder", "123", "");
            return user;
        }

        public User CreateUserWithInvalidEmail()
        {
            user = new User("User from Builder", "123", "aaaa");
            return user;
        }

        public User CreateUserWithNonexistentId()
        {
            user = new User("User from Builder", "123", "aaaa@gmail.com", "", 9999999);
            return user;
        }


        public User CreateUserWithId()
        {
            user = new User("User from Builder", "123", "aaaa@gmail.com", "", 1);
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
