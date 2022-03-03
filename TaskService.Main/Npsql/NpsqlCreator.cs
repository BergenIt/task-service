using System.Text;

using Npgsql;

using TaskService.Core.Options;

namespace TaskService.Npsql;

internal class NpsqlCreator
{
    private readonly string _sqlFile;

    private readonly string _databaseCreateSql;
    private readonly string _databaseExistSql;

    private readonly string _connectionString;
    private readonly string _dbName;

    public NpsqlCreator(ProjectOptions projectOptions)
    {
        _connectionString = projectOptions.PsqlDbConnectionStringWithoutDbName;
        _dbName = projectOptions.PsqlDbName;

        _sqlFile = projectOptions.PsqlMigrateSqlFile;

        _databaseCreateSql = @$"create database ""{_dbName}""";
        _databaseExistSql = $"select count(*) from pg_database where datname='{_dbName}'";
    }

    public void CreateDatabase()
    {
        using NpgsqlConnection npgsqlConnection = new(_connectionString);

        npgsqlConnection.Open();

        long dbExist;

        using (NpgsqlCommand createCommand = new(_databaseExistSql, npgsqlConnection))
        {
            dbExist = (long)(createCommand.ExecuteScalar() ?? long.MinValue);
        }

        if (dbExist <= 0)
        {
            using NpgsqlCommand createNpgsqlCommand = new(_databaseCreateSql, npgsqlConnection);

            createNpgsqlCommand.ExecuteNonQuery();
        }

        npgsqlConnection.ChangeDatabase(_dbName);

        string sqlMigrate = File.ReadAllText(_sqlFile, Encoding.UTF8);

        using NpgsqlCommand npgsqlCommand = new(sqlMigrate, npgsqlConnection);

        npgsqlCommand.ExecuteNonQuery();
    }
}
