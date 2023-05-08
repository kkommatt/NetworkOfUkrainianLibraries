using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NetworkOfLibrariesWebApplication;

public partial class Publisher
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Назва")]
    public string Title { get; set; } = null!;


    [Display(Name = "Місто")]
    public int CityId { get; set; }

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Власник")]
    public string Owner { get; set; } = null!;

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Потужність")]
    public string Power { get; set; } = null!;

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Адреса")]
    public string Adress { get; set; } = null!;

    public virtual ICollection<Book> Books { get; } = new List<Book>();

    [Display(Name = "Місто")]
    public virtual City City { get; set; } = null!;
}
