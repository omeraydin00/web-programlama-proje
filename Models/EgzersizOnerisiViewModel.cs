using System.ComponentModel.DataAnnotations;

namespace SporSalonuYonetim.Models;

public class EgzersizOnerisiViewModel
{
    [Display(Name = "Vücut Tipi")]
    public string? VucutTipi { get; set; }

    [Display(Name = "Boy (cm)")]
    [Range(100, 250, ErrorMessage = "Boy 100-250 cm arasında olmalıdır.")]
    public double? Boy { get; set; }

    [Display(Name = "Kilo (kg)")]
    [Range(30, 300, ErrorMessage = "Kilo 30-300 kg arasında olmalıdır.")]
    public double? Kilo { get; set; }

    [Display(Name = "Hedef")]
    [StringLength(500)]
    public string? Hedef { get; set; }

    [Display(Name = "Fotoğraf (Opsiyonel)")]
    public IFormFile? Foto { get; set; }

    [Display(Name = "Egzersiz Sonrası Görünüm Resmi İste")]
    public bool GorselSonucIste { get; set; } = false;
}




<<<<<<< Updated upstream

=======
>>>>>>> Stashed changes
