using System;
using System.Collections.Generic;

namespace NetworkOfLibrariesWebApplication;

public partial class Style
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Info { get; set; }

    public virtual ICollection<Book> Books { get; } = new List<Book>();
}
