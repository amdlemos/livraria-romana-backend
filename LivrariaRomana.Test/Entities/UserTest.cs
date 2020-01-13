using FluentValidation.TestHelper;
using LivrariaRomana.Test.DataBuilder;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using static LivrariaRomana.Domain.Entities.User;

namespace LivrariaRomana.Test.Entities
{
    public class UserTest
    {
        private UserValidator _userValidator;
        private UserBuilder _userBuilder;

        public UserTest()
        {
            _userValidator = new UserValidator();
            _userBuilder = new UserBuilder();
        }

        #region Propriedade Username
        [Fact]
        public void Should_have_error_when_username_is_empty()
        {
            var user = _userBuilder.CreateUserWithEmptyUsername();
            _userValidator.ShouldHaveValidationErrorFor(x => x.Username, user);
        }

        [Fact]
        public void Should_have_error_when_username_is_null()
        {
            var user = _userBuilder.CreateUserWithNullUsername();
            _userValidator.ShouldHaveValidationErrorFor(x => x.Username, user);
        }

        [Fact]
        public void Should_not_have_error_when_username_is_specified()
        {
            var user = _userBuilder.CreateUser();
            _userValidator.ShouldNotHaveValidationErrorFor(x => x.Username, user);
        }
        #endregion

        #region Propriedade Password
        [Fact]
        public void Should_have_error_when_password_is_empty()
        {
            var user = _userBuilder.CreateUserWithEmptyPassword();
            _userValidator.ShouldHaveValidationErrorFor(x => x.Password, user);
        }

        [Fact]
        public void Should_have_error_when_password_is_null()
        {
            var user = _userBuilder.CreateUserWithNullPassword();
            _userValidator.ShouldHaveValidationErrorFor(x => x.Password, user);
        }

        [Fact]
        public void Should_not_have_error_when_password_is_specified()
        {
            var user = _userBuilder.CreateUser();
            _userValidator.ShouldNotHaveValidationErrorFor(x => x.Password, user);
        }
        #endregion

        #region Propriedade Email
        [Fact]
        public void Should_have_error_when_email_is_empty()
        {
            var user = _userBuilder.CreateUserWithEmptyEmail();
            _userValidator.ShouldHaveValidationErrorFor(x => x.Email, user);
        }

        [Fact]
        public void Should_have_error_when_email_is_null()
        {
            var user = _userBuilder.CreateUserWithNullEmail();
            _userValidator.ShouldHaveValidationErrorFor(x => x.Email, user);
        }

        [Fact]
        public void Should_have_error_when_email_is_invalid()
        {
            var user = _userBuilder.CreateUserWithInvalidEmail();
            _userValidator.ShouldHaveValidationErrorFor(x => x.Email, user);
        }

        [Fact]
        public void Should_not_have_error_when_email_is_specified()
        {
            var user = _userBuilder.CreateUser();
            _userValidator.ShouldNotHaveValidationErrorFor(x => x.Email, user);
        }
        #endregion
    }
}
