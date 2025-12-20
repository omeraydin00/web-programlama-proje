using System.ComponentModel.DataAnnotations;

namespace SporSalonuYonetim.Models;

public class SporSalonu
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ad alanı zorunludur.")]
    [StringLength(200, ErrorMessage = "Ad en fazla 200 karakter olabilir.")]
    [Display(Name = "Spor Salonu Adı")]
    public string Ad { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Adres en fazla 500 karakter olabilir.")]
    [Display(Name = "Adres")]
    public string? Adres { get; set; }

    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
    [StringLength(20)]
    [Display(Name = "Telefon")]
    public string? Telefon { get; set; }

    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    [StringLength(100)]
    [Display(Name = "E-posta")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Açılış saati zorunludur.")]
    [Display(Name = "Açılış Saati")]
    public TimeSpan AcilisSaati { get; set; }

    [Required(ErrorMessage = "Kapanış saati zorunludur.")]
    [Display(Name = "Kapanış Saati")]
    public TimeSpan KapanisSaati { get; set; }

    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<Hizmet> Hizmetler { get; set; } = new List<Hizmet>();
    public virtual ICollection<Antrenor> Antrenorler { get; set; } = new List<Antrenor>();
}






