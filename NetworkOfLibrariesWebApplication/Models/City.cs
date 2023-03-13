using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NetworkOfLibrariesWebApplication;

public partial class City
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Назва")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Область/Крим/Київ/Севастополь")]
    public string Region { get; set; } = null!;

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Населення")]
    public int Population { get; set; }

    public virtual ICollection<Library> Libraries { get; } = new List<Library>();

    public virtual ICollection<Publisher> Publishers { get; } = new List<Publisher>();
}
