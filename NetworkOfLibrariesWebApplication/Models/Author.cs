using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NetworkOfLibrariesWebApplication;

public partial class Author : Entity
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Ім'я")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Прізвище")]
    public string Surname { get; set; } = null!;

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Рік народження")]
    public string Datebirth { get; set; } = null!;

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Освіта")]
    public string Education { get; set; } = null!;

    public virtual ICollection<AuthorBook> AuthorBooks { get; } = new List<AuthorBook>();

    public ICollection<Book>? Books => AuthorBooks?.Select(authorBook => authorBook.Book).Where(book => book is not null).ToList();

}
