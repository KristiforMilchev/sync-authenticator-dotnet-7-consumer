using LoginSample.Models;
using SQLite;

namespace LoginSample.Handlers.Implementation;

public class DatabaseContext
{
    private readonly SQLiteConnection _database;

    public DatabaseContext()
    {
        var dbPath = Path.Combine(Utilities.GetOsSavePath(), "accounts.db");
        _database = new SQLiteConnection(dbPath);
        _database.CreateTable<Account>();
    }
    
    public int Create(Account entity)
    {
        return _database.Insert(entity);
    }
    
    public int Update(Account entity)
    {
        return _database.Update(entity);
    }
    
    public Account Single(string email)
    {
        return _database.Table<Account>().FirstOrDefault(x => x.Email == email);
    }

}