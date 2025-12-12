namespace SporSalonuYonetim.Models;

public class SporSalonu
{
    public int Id { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string? Adres { get; set; }
    public string? Telefon { get; set; }
    public string? Email { get; set; }
    public TimeSpan AcilisSaati { get; set; }
    public TimeSpan KapanisSaati { get; set; }
    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    
    // Navigation properties
    public virtual ICollection<Hizmet> Hizmetler { get; set; } = new List<Hizmet>();
    public virtual ICollection<Antrenor> Antrenorler { get; set; } = new List<Antrenor>();
}





