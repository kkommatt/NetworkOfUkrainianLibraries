using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NetworkOfLibrariesWebApplication;

public partial class Style : Entity
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Назва")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Тип")]
    public string Type { get; set; } = null!;

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Інформація")]
    public string? Info { get; set; }

    public virtual ICollection<Book> Books { get; } = new List<Book>();
}
