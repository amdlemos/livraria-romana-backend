using Xunit;
using FluentValidation;
using FluentValidation.TestHelper;
using LivrariaRomana.Domain.Entities;
using static LivrariaRomana.Domain.Entities.Book;
using LivrariaRomana.Test.DataBuilder;

namespace LivrariaRomana.Test.Entities
{
    public class BookTest
    {
        private BookValidator _bookValidator;
        private BookBuilder _bookBuilder;        

        public BookTest()
        {
            _bookValidator = new BookValidator(null);
            _bookBuilder = new BookBuilder();
        }

        #region Propriedade Author
        [Fact]
        public void Should_have_error_when_author_is_null_or_empty()
        {
            var book = _bookBuilder.CreateAuthorlessBook();
            _bookValidator.ShouldHaveValidationErrorFor(x => x.Author, book);
        }

        [Fact]
        public void Should_not_have_error_when_author_is_specified()
        {
            var book = _bookBuilder.CreateValidBook();
            _bookValidator.ShouldNotHaveValidationErrorFor(x => x.Author, book);
        }
        #endregion

        #region Propriedade Title
        [Fact]
        public void Should_have_error_when_title_is_null_or_empty()
        {
            var book = _bookBuilder.CreateUntitledBook();
            _bookValidator.ShouldHaveValidationErrorFor(x => x.Title, book);
        }

        [Fact]
        public void Should_not_have_error_when_title_is_specified()
        {
            var book = _bookBuilder.CreateValidBook();
            _bookValidator.ShouldNotHaveValidationErrorFor(x => x.Title, book);
        }
        #endregion

        #region Propriedade ISBN
        [Fact]
        public void Should_have_error_when_isbn_is_not_valid()
        {
            var book = _bookBuilder.CreateBookWithInvalidISBN();
            //var result = _bookValidator.Validate(book);
            Assert.True(book.Invalid);
            Assert.True(book.ValidationResult.Errors[0].PropertyName == "ISBN");
        }

        [Fact]
        public void Should_not_have_error_when_isbn_is_valid()
        {
            var book = _bookBuilder.CreateValidBook();
            //var result = _bookValidator.Validate(book);
            Assert.True(book.Valid);
        }
        #endregion

        #region Propriedade Amount
        [Fact]
        public void Should_have_error_when_amount_is_null_or_less_than_0()
        {
            var book = _bookBuilder.CreateBookWithSmallerAmount0();
            _bookValidator.ShouldHaveValidationErrorFor(x => x.Amount, book);
        }

        [Fact]
        public void Should_not_have_error_when_amount_is_greater_than_or_equal_0()
        {
            var book = _bookBuilder.CreateValidBook();
            _bookValidator.ShouldNotHaveValidationErrorFor(x => x.Amount, book);
        }
        #endregion
    }
}
