using System;
using System.Collections.Generic;

namespace NetworkOfLibrariesWebApplication;

public partial class Publisher
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int CityId { get; set; }

    public string Owner { get; set; } = null!;

    public string Power { get; set; } = null!;

    public string Adress { get; set; } = null!;

    public virtual ICollection<Book> Books { get; } = new List<Book>();

    public virtual City City { get; set; } = null!;
}
