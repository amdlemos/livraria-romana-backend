using LivrariaRomana.Domain.Entities;
using System.Collections.Generic;

namespace LivrariaRomana.Test.DataBuilder
{
    public class BookBuilder
    {
        private Book book;
        private List<Book> bookList;

        public Book CreateBook()
        {
            book = new Book()
            {
                Author = "Author from Builder",
                OriginalTitle = "Original Title from Builder",
                Title = "TiTile from builder",
                PublicationYear = 2005,
                PublishingCompany = "Publising Company from Builder"

            };
            return book;
        }

        public List<Book> CreateBookList(int amount)
        {
            bookList = new List<Book>();
            for (int i = 0; i < amount; i++)
            {
                bookList.Add(CreateBook());
            }

            return bookList;
        }
    }
}
