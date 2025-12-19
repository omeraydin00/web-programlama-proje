using Microsoft.AspNetCore.Identity;

namespace SporSalonuYonetim.Models;

public class ApplicationUser : IdentityUser
{
    public string? Ad { get; set; }
    public string? Soyad { get; set; }
    public DateTime? DogumTarihi { get; set; }
    public string? Telefon { get; set; }
    public string? Adres { get; set; }
    
    // Navigation properties
    public virtual ICollection<Randevu> Randevular { get; set; } = new List<Randevu>();
}









