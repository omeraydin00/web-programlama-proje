using System.ComponentModel.DataAnnotations;

namespace SporSalonuYonetim.Models;

public class Randevu
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Randevu tarihi zorunludur.")]
    [Display(Name = "Randevu Tarihi")]
    public DateTime RandevuTarihi { get; set; }

    [Required(ErrorMessage = "Başlangıç saati zorunludur.")]
    [Display(Name = "Başlangıç Saati")]
    public TimeSpan BaslangicSaati { get; set; }

    [Required(ErrorMessage = "Bitiş saati zorunludur.")]
    [Display(Name = "Bitiş Saati")]
    public TimeSpan BitisSaati { get; set; }

    [Required(ErrorMessage = "Toplam ücret zorunludur.")]
    [Range(0, double.MaxValue, ErrorMessage = "Toplam ücret 0'dan büyük olmalıdır.")]
    [Display(Name = "Toplam Ücret")]
    public decimal ToplamUcret { get; set; }

    [StringLength(50)]
    [Display(Name = "Durum")]
    public string Durum { get; set; } = "Beklemede"; // Beklemede, Onaylandı, İptal, Tamamlandı

    [StringLength(1000, ErrorMessage = "Notlar en fazla 1000 karakter olabilir.")]
    [Display(Name = "Notlar")]
    public string? Notlar { get; set; }

    [Display(Name = "Oluşturma Tarihi")]
    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

    // Foreign Keys
    public string? UyeId { get; set; }

    [Required(ErrorMessage = "Antrenör seçimi zorunludur.")]
    [Display(Name = "Antrenör")]
    public int AntrenorId { get; set; }

    [Required(ErrorMessage = "Hizmet seçimi zorunludur.")]
    [Display(Name = "Hizmet")]
    public int HizmetId { get; set; }

    // Navigation properties
    public virtual ApplicationUser? Uye { get; set; }
    public virtual Antrenor? Antrenor { get; set; }
    public virtual Hizmet? Hizmet { get; set; }
}








