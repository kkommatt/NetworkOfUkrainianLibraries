using System;
using System.Collections.Generic;

namespace NetworkOfLibrariesWebApplication;

public partial class AuthorBook
{
    public int AuthorId { get; set; }

    public int BookId { get; set; }

    public int Id { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual Book Book { get; set; } = null!;
}
