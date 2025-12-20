using System.ComponentModel.DataAnnotations;

namespace SporSalonuYonetim.Models;

public class Hizmet
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ad alanı zorunludur.")]
    [StringLength(200, ErrorMessage = "Ad en fazla 200 karakter olabilir.")]
    [Display(Name = "Hizmet Adı")]
    public string Ad { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
    [Display(Name = "Açıklama")]
    public string? Aciklama { get; set; }

    [Required(ErrorMessage = "Süre zorunludur.")]
    [Range(1, 600, ErrorMessage = "Süre 1-600 dakika arasında olmalıdır.")]
    [Display(Name = "Süre (Dakika)")]
    public int Sure { get; set; } // Dakika cinsinden

    [Required(ErrorMessage = "Ücret zorunludur.")]
    [Range(0, double.MaxValue, ErrorMessage = "Ücret 0'dan büyük olmalıdır.")]
    [Display(Name = "Ücret (₺)")]
    public decimal Ucret { get; set; }

    [StringLength(100, ErrorMessage = "Hizmet türü en fazla 100 karakter olabilir.")]
    [Display(Name = "Hizmet Türü")]
    public string? HizmetTuru { get; set; } // Örn: "Fitness", "Yoga", "Pilates"

    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

    // Foreign Key
    [Required(ErrorMessage = "Spor salonu seçimi zorunludur.")]
    [Display(Name = "Spor Salonu")]
    public int SporSalonuId { get; set; }

    // Navigation properties
    public virtual SporSalonu? SporSalonu { get; set; }
    public virtual ICollection<Randevu> Randevular { get; set; } = new List<Randevu>();
}






