using System;
using System.Collections.Generic;

namespace NetworkOfLibrariesWebApplication;

public partial class City
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Region { get; set; } = null!;

    public int Population { get; set; }

    public virtual ICollection<Library> Libraries { get; } = new List<Library>();

    public virtual ICollection<Publisher> Publishers { get; } = new List<Publisher>();
}
