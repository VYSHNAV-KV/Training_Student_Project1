using Microsoft.AspNetCore.Mvc;
using Npgsql;
using WebApplication3.Models;
using System.Text.Json;
using WebApplication3.Data;

[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly string _connectionString;

    public StudentController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }


    [HttpGet]
    public IActionResult GetAll()
    {
        var list = new List<object>();
        using var con = new NpgsqlConnection(_connectionString);
        con.Open();
        var cmd = new NpgsqlCommand(
            "SELECT s.\"Id\", s.\"Name\", s.\"Mobile\", s.\"Dob\", " +
            "c.\"Name\" AS CountryName, s.\"CountryId\", " +
            "s.\"Gender\", s.\"IsIndian\" " +
            "FROM \"Students\" s " +
            "JOIN \"Countries\" c ON s.\"CountryId\" = c.\"Id\" " +
            "ORDER BY s.\"Name\"", con);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                Mobile = reader.GetString(2),
                Dob = reader.GetDateTime(3),
                Country = reader.GetString(4), // Country Name
                CountryId = reader.GetGuid(5), // FK
                Gender = reader.GetString(6),
                IsIndian = reader.GetBoolean(7)
            });
        }
        return Ok(list);
    }


    [HttpPost]
    public async Task<IActionResult> Create(Student student)
    {
        student.Id = Guid.NewGuid();
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Saved Successfully", id = student.Id });
    }


    //[HttpPut("{id}")]
    //public async Task<IActionResult> Update(Guid id, Student student)
    //{
    //    var existing = await _context.Students.FindAsync(id);
    //    if (existing == null) return NotFound("Student not found");
    //    existing.Name = student.Name;
    //    existing.Mobile = student.Mobile;
    //    existing.Dob = student.Dob;
    //    existing.CountryId = student.CountryId;
    //    existing.Gender = student.Gender;
    //    existing.IsIndian = student.IsIndian;
    //    await _context.SaveChangesAsync();
    //    return Ok(new { message = "Updated Successfully", id });
    //}






    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(Guid id, [FromBody] JsonElement data)
    {
        var student = await _context.Students.FindAsync(id);

        if (student == null)
            return NotFound();


        if (data.TryGetProperty("name", out var name))
            student.Name = name.GetString();

        if (data.TryGetProperty("mobile", out var mobile))
            student.Mobile = mobile.GetString();

        if (data.TryGetProperty("dob", out var dob))
            student.Dob = dob.GetDateTime();

        if (data.TryGetProperty("countryId", out var countryId))
            student.CountryId = countryId.GetGuid();

        if (data.TryGetProperty("gender", out var gender))
            student.Gender = gender.GetString();

        if (data.TryGetProperty("isIndian", out var isIndian))
            student.IsIndian = isIndian.GetBoolean();

        await _context.SaveChangesAsync();

        return Ok(new { message = "Patched Successfully", id });
    }





    //[HttpPatch("{id}")]
    //public IActionResult Patch(Guid id, [FromBody] Student student)
    //{
    //    using var con = new NpgsqlConnection(_connectionString);
    //    con.Open();
    //    var updates = new List<string>();
    //    var cmd = new NpgsqlCommand();
    //    cmd.Connection = con;
    //    if (!string.IsNullOrEmpty(student.Name))
    //    {
    //        updates.Add("\"Name\"=@name");
    //        cmd.Parameters.AddWithValue("@name", student.Name);
    //    }
    //    if (!string.IsNullOrEmpty(student.Mobile))
    //    {
    //        updates.Add("\"Mobile\"=@mobile");
    //        cmd.Parameters.AddWithValue("@mobile", student.Mobile);
    //    }
    //    if (student.Dob != default)
    //    {
    //        updates.Add("\"Dob\"=@dob");
    //        cmd.Parameters.AddWithValue("@dob", student.Dob);
    //    }
    //    if (student.CountryId != Guid.Empty)
    //    {
    //        updates.Add("\"CountryId\"=@countryId");
    //        cmd.Parameters.AddWithValue("@countryId", student.CountryId);
    //    }
    //    if (!string.IsNullOrEmpty(student.Gender))
    //    {
    //        updates.Add("\"Gender\"=@gender");
    //        cmd.Parameters.AddWithValue("@gender", student.Gender);
    //    }
    //    updates.Add("\"IsIndian\"=@isIndian");
    //    cmd.Parameters.AddWithValue("@isIndian", student.IsIndian);
    //    if (updates.Count == 0) return BadRequest("No fields to update");
    //    cmd.CommandText = $"UPDATE \"Students\" SET {string.Join(",", updates)} WHERE \"Id\"=@id";
    //    cmd.Parameters.AddWithValue("@id", id);
    //    int rows = cmd.ExecuteNonQuery();
    //    if (rows == 0) return NotFound("Student not found");
    //    return Ok(new { message = "Updated Successfully", id });
    //}

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        using var con = new NpgsqlConnection(_connectionString);
        con.Open();
        var cmd = new NpgsqlCommand(
            "DELETE FROM \"Students\" WHERE \"Id\"=@id", con);
        cmd.Parameters.AddWithValue("@id", id);
        int rows = cmd.ExecuteNonQuery();
        if (rows == 0) return NotFound("Student not found");
        return Ok(new { message = "Deleted Successfully", id });
    }
}