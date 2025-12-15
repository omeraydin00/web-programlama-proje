namespace SporSalonuYonetim.Models;

public class AntrenorMüsaitlik
{
    public int Id { get; set; }
    public DayOfWeek HaftaninGunu { get; set; }
    public TimeSpan BaslangicSaati { get; set; }
    public TimeSpan BitisSaati { get; set; }
    
    // Foreign Key
    public int AntrenorId { get; set; }
    
    // Navigation properties
    public virtual Antrenor? Antrenor { get; set; }
}








