namespace SporSalonuYonetim.Models;

public class Antrenor
{
    public int Id { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefon { get; set; }
    public string? UzmanlikAlanlari { get; set; } // Örn: "Kas geliştirme, Kilo verme, Yoga"
    public string? Aciklama { get; set; }
    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    
    // Foreign Key
    public int SporSalonuId { get; set; }
    
    // Navigation properties
    public virtual SporSalonu? SporSalonu { get; set; }
    public virtual ICollection<Randevu> Randevular { get; set; } = new List<Randevu>();
    public virtual ICollection<AntrenorMüsaitlik> Müsaitlikler { get; set; } = new List<AntrenorMüsaitlik>();
}



