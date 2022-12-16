using SQLite;

namespace LoginSample.Models;

public class Account
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    public string? TwoFactorId { get; set; }
    public string? TwoFactorKey { get; set; }
}