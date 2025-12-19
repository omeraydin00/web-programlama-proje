using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetim.Data;
using SporSalonuYonetim.Models;

namespace SporSalonuYonetim.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AntrenorApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AntrenorApiController> _logger;

    public AntrenorApiController(ApplicationDbContext context, ILogger<AntrenorApiController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/AntrenorApi
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAntrenorler(
        [FromQuery] int? sporSalonuId,
        [FromQuery] string? uzmanlikAlani,
        [FromQuery] DateTime? tarih,
        [FromQuery] TimeSpan? saat)
    {
        var query = _context.Antrenorler
            .Include(a => a.SporSalonu)
            .Include(a => a.Müsaitlikler)
            .AsQueryable();

        // Spor salonu filtresi
        if (sporSalonuId.HasValue)
        {
            query = query.Where(a => a.SporSalonuId == sporSalonuId.Value);
        }

        // Uzmanlık alanı filtresi
        if (!string.IsNullOrEmpty(uzmanlikAlani))
        {
            query = query.Where(a => a.UzmanlikAlanlari != null && 
                a.UzmanlikAlanlari.Contains(uzmanlikAlani));
        }

        // Tarih ve saat filtresi - Müsait antrenörleri getir
        if (tarih.HasValue && saat.HasValue)
        {
            var gun = tarih.Value.DayOfWeek;
            var bitisSaati = saat.Value.Add(TimeSpan.FromHours(1)); // Varsayılan 1 saatlik randevu

            query = query.Where(a => a.Müsaitlikler.Any(m => 
                m.HaftaninGunu == gun &&
                m.BaslangicSaati <= saat.Value &&
                m.BitisSaati >= bitisSaati));

            // Çakışan randevuları kontrol et
            var cakisanAntrenorIds = await _context.Randevular
                .Where(r => r.RandevuTarihi.Date == tarih.Value.Date &&
                           r.Durum != "İptal" &&
                           ((r.BaslangicSaati <= saat.Value && r.BitisSaati > saat.Value) ||
                            (r.BaslangicSaati < bitisSaati && r.BitisSaati >= bitisSaati) ||
                            (r.BaslangicSaati >= saat.Value && r.BitisSaati <= bitisSaati)))
                .Select(r => r.AntrenorId)
                .Distinct()
                .ToListAsync();

            query = query.Where(a => !cakisanAntrenorIds.Contains(a.Id));
        }

        var antrenorler = await query
            .Select(a => new
            {
                a.Id,
                Ad = a.Ad + " " + a.Soyad,
                a.Email,
                a.Telefon,
                SporSalonu = a.SporSalonu != null ? a.SporSalonu.Ad : null,
                a.UzmanlikAlanlari,
                a.Aciklama,
                Müsaitlikler = a.Müsaitlikler.Select(m => new
                {
                    Gün = m.HaftaninGunu.ToString(),
                    BaslangicSaati = m.BaslangicSaati.ToString(@"hh\:mm"),
                    BitisSaati = m.BitisSaati.ToString(@"hh\:mm")
                }).ToList()
            })
            .ToListAsync();

        return Ok(antrenorler);
    }

    // GET: api/AntrenorApi/5
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetAntrenor(int id)
    {
        var antrenor = await _context.Antrenorler
            .Include(a => a.SporSalonu)
            .Include(a => a.Müsaitlikler)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (antrenor == null)
        {
            return NotFound();
        }

        var result = new
        {
            antrenor.Id,
            Ad = antrenor.Ad + " " + antrenor.Soyad,
            antrenor.Email,
            antrenor.Telefon,
            SporSalonu = antrenor.SporSalonu != null ? antrenor.SporSalonu.Ad : null,
            antrenor.UzmanlikAlanlari,
            antrenor.Aciklama,
            Müsaitlikler = antrenor.Müsaitlikler.Select(m => new
            {
                Gün = m.HaftaninGunu.ToString(),
                BaslangicSaati = m.BaslangicSaati.ToString(@"hh\:mm"),
                BitisSaati = m.BitisSaati.ToString(@"hh\:mm")
            }).ToList()
        };

        return Ok(result);
    }
}




