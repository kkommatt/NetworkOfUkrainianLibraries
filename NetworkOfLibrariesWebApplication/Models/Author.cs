using System;
using System.Collections.Generic;

namespace NetworkOfLibrariesWebApplication;

public partial class Author
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Datebirth { get; set; } = null!;

    public string Education { get; set; } = null!;

    public virtual ICollection<AuthorBook> AuthorBooks { get; } = new List<AuthorBook>();
}
