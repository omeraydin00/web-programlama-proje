using System.ComponentModel.DataAnnotations;

namespace SporSalonuYonetim.Models;

public class Antrenor
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ad alanı zorunludur.")]
    [StringLength(100, ErrorMessage = "Ad en fazla 100 karakter olabilir.")]
    [Display(Name = "Ad")]
    public string Ad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad alanı zorunludur.")]
    [StringLength(100, ErrorMessage = "Soyad en fazla 100 karakter olabilir.")]
    [Display(Name = "Soyad")]
    public string Soyad { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    [StringLength(100)]
    [Display(Name = "E-posta")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
    [StringLength(20)]
    [Display(Name = "Telefon")]
    public string? Telefon { get; set; }

    [StringLength(500, ErrorMessage = "Uzmanlık alanları en fazla 500 karakter olabilir.")]
    [Display(Name = "Uzmanlık Alanları")]
    public string? UzmanlikAlanlari { get; set; } // Örn: "Kas geliştirme, Kilo verme, Yoga"

    [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
    [Display(Name = "Açıklama")]
    public string? Aciklama { get; set; }

    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

    // Foreign Key
    [Required(ErrorMessage = "Spor salonu seçimi zorunludur.")]
    [Display(Name = "Spor Salonu")]
    public int SporSalonuId { get; set; }

    // Navigation properties
    public virtual SporSalonu? SporSalonu { get; set; }
    public virtual ICollection<Randevu> Randevular { get; set; } = new List<Randevu>();
    public virtual ICollection<AntrenorMüsaitlik> Müsaitlikler { get; set; } = new List<AntrenorMüsaitlik>();
}






