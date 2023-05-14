using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetworkOfLibrariesWebApplication;
[Table("AuthorBooks")]
public class AuthorBook : Entity
{
    public int AuthorId { get; set; }

    public int BookId { get; set; }

    [Key]
    public int Id { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual Book Book { get; set; } = null!;
}
