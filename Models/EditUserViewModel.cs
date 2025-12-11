using System.ComponentModel.DataAnnotations;

namespace SporSalonuYonetim.Models;

public class EditUserViewModel
{
    [Required(ErrorMessage = "Ad alanı zorunludur.")]
    [Display(Name = "Ad")]
    [StringLength(100)]
    public string Ad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad alanı zorunludur.")]
    [Display(Name = "Soyad")]
    [StringLength(100)]
    public string Soyad { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta alanı zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    [Display(Name = "E-posta")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Telefon")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
    public string? Telefon { get; set; }

    [Display(Name = "Doğum Tarihi")]
    [DataType(DataType.Date)]
    public DateTime? DogumTarihi { get; set; }

    [Display(Name = "Adres")]
    [StringLength(500)]
    public string? Adres { get; set; }
}

