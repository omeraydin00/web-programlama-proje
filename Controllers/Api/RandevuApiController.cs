using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetim.Data;
using SporSalonuYonetim.Models;

namespace SporSalonuYonetim.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RandevuApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<RandevuApiController> _logger;

    public RandevuApiController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger<RandevuApiController> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    // GET: api/RandevuApi
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetRandevular(
        [FromQuery] string? uyeId,
        [FromQuery] int? antrenorId,
        [FromQuery] DateTime? baslangicTarihi,
        [FromQuery] DateTime? bitisTarihi,
        [FromQuery] string? durum)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var query = _context.Randevular
            .Include(r => r.Antrenor)
            .Include(r => r.Hizmet)
            .Include(r => r.Uye)
            .AsQueryable();

        // Admin değilse sadece kendi randevularını görebilir
        if (!User.IsInRole("Admin"))
        {
            query = query.Where(r => r.UyeId == user.Id);
        }
        else if (!string.IsNullOrEmpty(uyeId))
        {
            query = query.Where(r => r.UyeId == uyeId);
        }

        // Antrenör filtresi
        if (antrenorId.HasValue)
        {
            query = query.Where(r => r.AntrenorId == antrenorId.Value);
        }

        // Tarih aralığı filtresi
        if (baslangicTarihi.HasValue)
        {
            query = query.Where(r => r.RandevuTarihi >= baslangicTarihi.Value);
        }

        if (bitisTarihi.HasValue)
        {
            query = query.Where(r => r.RandevuTarihi <= bitisTarihi.Value);
        }

        // Durum filtresi
        if (!string.IsNullOrEmpty(durum))
        {
            query = query.Where(r => r.Durum == durum);
        }

        var randevular = await query
            .OrderByDescending(r => r.RandevuTarihi)
            .ThenByDescending(r => r.BaslangicSaati)
            .Select(r => new
            {
                r.Id,
                RandevuTarihi = r.RandevuTarihi.ToString("yyyy-MM-dd"),
                BaslangicSaati = r.BaslangicSaati.ToString(@"hh\:mm"),
                BitisSaati = r.BitisSaati.ToString(@"hh\:mm"),
                Antrenor = r.Antrenor != null ? r.Antrenor.Ad + " " + r.Antrenor.Soyad : null,
                Hizmet = r.Hizmet != null ? r.Hizmet.Ad : null,
                Uye = r.Uye != null ? r.Uye.Ad + " " + r.Uye.Soyad : null,
                r.ToplamUcret,
                r.Durum,
                r.Notlar,
                OlusturmaTarihi = r.OlusturmaTarihi.ToString("yyyy-MM-dd HH:mm")
            })
            .ToListAsync();

        return Ok(randevular);
    }

    // GET: api/RandevuApi/5
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetRandevu(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var randevu = await _context.Randevular
            .Include(r => r.Antrenor)
            .Include(r => r.Hizmet)
            .Include(r => r.Uye)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (randevu == null)
        {
            return NotFound();
        }

        // Admin değilse sadece kendi randevusunu görebilir
        if (!User.IsInRole("Admin") && randevu.UyeId != user.Id)
        {
            return Forbid();
        }

        var result = new
        {
            randevu.Id,
            RandevuTarihi = randevu.RandevuTarihi.ToString("yyyy-MM-dd"),
            BaslangicSaati = randevu.BaslangicSaati.ToString(@"hh\:mm"),
            BitisSaati = randevu.BitisSaati.ToString(@"hh\:mm"),
            Antrenor = randevu.Antrenor != null ? new
            {
                randevu.Antrenor.Id,
                Ad = randevu.Antrenor.Ad + " " + randevu.Antrenor.Soyad,
                randevu.Antrenor.Email,
                randevu.Antrenor.Telefon
            } : null,
            Hizmet = randevu.Hizmet != null ? new
            {
                randevu.Hizmet.Id,
                randevu.Hizmet.Ad,
                randevu.Hizmet.HizmetTuru,
                randevu.Hizmet.Sure,
                randevu.Hizmet.Ucret
            } : null,
            Uye = randevu.Uye != null ? new
            {
                randevu.Uye.Id,
                Ad = randevu.Uye.Ad + " " + randevu.Uye.Soyad,
                randevu.Uye.Email,
                randevu.Uye.Telefon
            } : null,
            randevu.ToplamUcret,
            randevu.Durum,
            randevu.Notlar,
            OlusturmaTarihi = randevu.OlusturmaTarihi.ToString("yyyy-MM-dd HH:mm")
        };

        return Ok(result);
    }
}


