using System;
using System.Collections.Generic;

namespace NetworkOfLibrariesWebApplication;

public partial class Library
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Adress { get; set; } = null!;

    public string Website { get; set; } = null!;

    public string Schedule { get; set; } = null!;

    public int CityId { get; set; }

    public virtual ICollection<BookLibrary> BookLibraries { get; } = new List<BookLibrary>();

    public virtual City City { get; set; } = null!;
}
