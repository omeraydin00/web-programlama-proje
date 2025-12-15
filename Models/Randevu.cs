namespace SporSalonuYonetim.Models;

public class Randevu
{
    public int Id { get; set; }
    public DateTime RandevuTarihi { get; set; }
    public TimeSpan BaslangicSaati { get; set; }
    public TimeSpan BitisSaati { get; set; }
    public decimal ToplamUcret { get; set; }
    public string Durum { get; set; } = "Beklemede"; // Beklemede, Onaylandı, İptal, Tamamlandı
    public string? Notlar { get; set; }
    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    
    // Foreign Keys
    public string? UyeId { get; set; }
    public int AntrenorId { get; set; }
    public int HizmetId { get; set; }
    
    // Navigation properties
    public virtual ApplicationUser? Uye { get; set; }
    public virtual Antrenor? Antrenor { get; set; }
    public virtual Hizmet? Hizmet { get; set; }
}








