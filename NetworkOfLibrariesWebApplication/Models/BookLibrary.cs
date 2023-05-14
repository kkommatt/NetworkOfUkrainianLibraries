using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetworkOfLibrariesWebApplication;
[Table("BookLibraries")]
public partial class BookLibrary : Entity
{
    public int BookId { get; set; }

    public int LibraryId { get; set; }

    [Key]
    public int Id { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Library Library { get; set; } = null!;
}
