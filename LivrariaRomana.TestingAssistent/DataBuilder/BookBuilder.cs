using LivrariaRomana.Domain.Entities;
using System;
using System.Collections.Generic;

namespace LivrariaRomana.TestingAssistent.DataBuilder
{
    public class BookBuilder
    {
        private Book book;
        private List<Book> bookList;

        public Book CreateValidBook()
        {
            book = new Book(
                "TiTile from builder",
                "Author from Builder",
                "Original Title from Builder", 
                "Publising Company from Builder",
                "ISBN 978-85-333-0227-3", 
                new DateTime(2005,1,1), 
                0);
            return book;
        }

        public Book CreateAuthorlessBook()
        {
            book = new Book(
                "TiTile from builder",
                "",
                "Original Title from Builder",
                "Publising Company from Builder",
                "",
                new DateTime(),
                0);
            return book;
        }

        public Book CreateUntitledBook()
        {
            book = new Book(
                "",
                "Original Title from Builder",
                "Author from Builder",
                "Publising Company from Builder",
                "",
                new DateTime(),
                0);
            return book;
        }

        public Book CreateBookWithInvalidISBN()
        {
            book = new Book(
                "Title from builder",
                "Original Title from Builder",
                "Author from Builder",
                "Publising Company from Builder",
                "123",
                new DateTime(),
                0);
            return book;
        }

        public Book CreateBookWithSmallerAmount0()
        {
            book = new Book(
                "TiTile from builder",
                "Author from Builder",
                "Original Title from Builder", 
                "Publising Company from Builder", 
                "", 
                new DateTime(), 
                -1);
            return book;
        }

        public Book CreateBookWithId()
        {
            book = new Book(                
                "TiTile from builder",
                "Author from Builder",
                "Original Title from Builder",
                "Publising Company from Builder",
                "ISBN 1-56389-668-0",
                new DateTime(2005, 1, 1),
                0, 
                4);
            return book;
        }

        public Book CreateBookWithNonexistentId(int id)
        {
            book = new Book(
                "TiTile from builder",
                "Author from Builder",
                "Original Title from Builder",
                "Publising Company from Builder",
                "978-85-333-0227-3",
                new DateTime(2005, 1, 1),
                id,
                9999999);
            return book;
        }

        public List<Book> CreateBookList(int amount)
        {
            bookList = new List<Book>();
            for (int i = 0; i < amount; i++)
            {
                bookList.Add(CreateValidBook());
            }

            return bookList;
        }
    }
}
