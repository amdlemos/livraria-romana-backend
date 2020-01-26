using LivrariaRomana.Domain.Entities;
using System;
using System.Collections.Generic;

namespace LivrariaRomana.Test.Helper
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
                "2015",
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
                "",
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
                "",
                0);
            return book;
        }

        public Book CreateBookWithInvalidPublicationYear()
        {
            book = new Book(
                "",
                "Original Title from Builder",
                "Author from Builder",
                "Publising Company from Builder",
                "","adsf",
                0);
            return book;
        }

        public Book CreateBookWithInvalidPatternPublicationYear()
        {
            book = new Book(
                "",
                "Original Title from Builder",
                "Author from Builder",
                "Publising Company from Builder",
                "", "19877",
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
                "2017",
                "",
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
                "2015", 0);
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
                "2005");
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
                "1998",
                id);
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
