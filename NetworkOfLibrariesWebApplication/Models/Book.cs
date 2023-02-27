using System;
using System.Collections.Generic;

namespace NetworkOfLibrariesWebApplication;

public partial class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int Year { get; set; }

    public int PublisherId { get; set; }

    public int StyleId { get; set; }

    public int Pages { get; set; }

    public string Annotation { get; set; } = null!;

    public int Circulation { get; set; }

    public virtual ICollection<AuthorBook> AuthorBooks { get; } = new List<AuthorBook>();

    public virtual ICollection<BookLibrary> BookLibraries { get; } = new List<BookLibrary>();

    public virtual Publisher Publisher { get; set; } = null!;

    public virtual Style Style { get; set; } = null!;
}
