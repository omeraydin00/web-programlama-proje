namespace SporSalonuYonetim.Models;

public class Hizmet
{
    public int Id { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string? Aciklama { get; set; }
    public int Sure { get; set; } // Dakika cinsinden
    public decimal Ucret { get; set; }
    public string? HizmetTuru { get; set; } // Örn: "Fitness", "Yoga", "Pilates"
    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    
    // Foreign Key
    public int SporSalonuId { get; set; }
    
    // Navigation properties
    public virtual SporSalonu? SporSalonu { get; set; }
    public virtual ICollection<Randevu> Randevular { get; set; } = new List<Randevu>();
}


