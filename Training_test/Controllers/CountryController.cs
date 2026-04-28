
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using WebApplication3.Models;
using System.Text.Json;
using WebApplication3.Data;

[ApiController]
[Route("api/[controller]")]
public class CountryController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly string _connectionString;

    public CountryController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }


    [HttpGet]
    public IActionResult GetAll()
    {
        var list = new List<Country>();
        using var con = new NpgsqlConnection(_connectionString);
        con.Open();
        var cmd = new NpgsqlCommand(
            "SELECT \"Id\",\"Name\",\"SortOrder\" FROM \"Countries\" ORDER BY \"SortOrder\"",
            con);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Country
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                SortOrder = reader.GetInt32(2)
            });
        }
        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Country country)
    {
        country.Id = Guid.NewGuid();
        _context.Countries.Add(country);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Country Added", id = country.Id });
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, Country country)
    {
        var existing = await _context.Countries.FindAsync(id);
        if (existing == null) return NotFound("Country not found");
        existing.Name = country.Name;
        existing.SortOrder = country.SortOrder;
        await _context.SaveChangesAsync();
        return Ok(new { message = "Updated Successfully", id });
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(Guid id, [FromBody] JsonElement data)
    {
        var country = await _context.Countries.FindAsync(id);

        if (country == null)
            return NotFound("Country not found");


        if (data.TryGetProperty("name", out var name))
            country.Name = name.GetString();

        if (data.TryGetProperty("sortOrder", out var sortOrder))
            country.SortOrder = sortOrder.GetInt32();

        await _context.SaveChangesAsync();

        return Ok(new { message = "Patched Successfully", id });
    }

    


    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        using var con = new NpgsqlConnection(_connectionString);
        con.Open();
        var checkCmd = new NpgsqlCommand(
            "SELECT COUNT(*) FROM \"Students\" WHERE \"CountryId\"=@id", con);
        checkCmd.Parameters.AddWithValue("@id", id);
        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
        if (count > 0)
        {
            return BadRequest(new { message = "Cannot delete. Country is used by students.", id });
        }
        var cmd = new NpgsqlCommand(
            "DELETE FROM \"Countries\" WHERE \"Id\"=@id", con);
        cmd.Parameters.AddWithValue("@id", id);
        int rows = cmd.ExecuteNonQuery();
        if (rows == 0) return NotFound("Country not found");
        return Ok(new { message = "Deleted Successfully", id });
    }
}


