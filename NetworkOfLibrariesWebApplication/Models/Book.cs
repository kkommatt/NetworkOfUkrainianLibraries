using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NetworkOfLibrariesWebApplication;

public partial class Book
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Назва")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Рік")]
    public int Year { get; set; }

    [Display(Name = "Видавництво")]
    public int PublisherId { get; set; }

    [Display(Name = "Стиль")]
    public int StyleId { get; set; }

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Кількість сторінок")]
    public int Pages { get; set; }

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Анотація")]
    public string Annotation { get; set; } = null!;

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Тираж")]
    public int Circulation { get; set; }

    public virtual ICollection<AuthorBook> AuthorBooks { get; } = new List<AuthorBook>();

    public ICollection<Author>? Authors => AuthorBooks?.Select(authorBook => authorBook.Author).Where(author => author is not null).ToList();

    public virtual ICollection<BookLibrary> BookLibraries { get; } = new List<BookLibrary>();


    public ICollection<Library>? Libraries => BookLibraries?.Select(bookLibrary => bookLibrary.Library).Where(library => library is not null).ToList();
    [Display(Name = "Видавництво")]
    public virtual Publisher? Publisher { get; set; } = null!;
    [Display(Name = "Стиль")]
    public virtual Style? Style { get; set; } = null!;
}
