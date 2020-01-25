﻿using LivrariaRomana.Domain.Validators;
using System;

namespace LivrariaRomana.Domain.Entities
{
    public class Book : Entity, IEntity
    {
        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public string Author { get; set; }
        public string PublishingCompany { get; set; }
        public string ISBN { get; set; }
        public string PublicationYear { get; set; }
        public int Amount { get; set; }

        public Book()
        {
            Validate(this, new BookValidator());
        }

        public Book(string title, string author)
        {
            Title = title;
            Author = author;

            Validate(this, new BookValidator());
        }

        public Book(string title, string author, string originalTitle, string publisingCompany, string isbn, string publicationYear = null, int amount = 0, int id = 0)
        {
            Id = id;
            Title = title;
            OriginalTitle = originalTitle;
            Author = author;
            PublishingCompany = publisingCompany;
            PublicationYear = publicationYear;
            ISBN = isbn;
            Amount = amount;

            Validate(this, new BookValidator());
        }
    }
}
