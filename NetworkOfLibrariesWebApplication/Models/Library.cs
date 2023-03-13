using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NetworkOfLibrariesWebApplication;

public partial class Library
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Назва")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Адреса")]
    public string Adress { get; set; } = null!;

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Веб-сайт")]
    public string Website { get; set; } = null!;

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Розклад роботи")]
    public string Schedule { get; set; } = null!;

    [Display(Name = "Місто")]
    public int CityId { get; set; }

    public virtual ICollection<BookLibrary> BookLibraries { get; } = new List<BookLibrary>();

    public ICollection<Book>? Books => BookLibraries?.Select(bookLibrary => bookLibrary.Book).Where(book => book is not null).ToList();

    public virtual City City { get; set; } = null!;
}
