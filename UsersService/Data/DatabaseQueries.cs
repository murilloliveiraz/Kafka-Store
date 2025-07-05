namespace Producer.Data
{
    public static class DatabaseQueries
    {
        public static Dictionary<DbQuery, string> Queries { get; } = new Dictionary<DbQuery, string>
        {
            { DbQuery.CreateTableUsers, @"
                CREATE TABLE IF NOT EXISTS Users (
                    uuid VARCHAR(200) PRIMARY KEY,
                    email VARCHAR(200) UNIQUE NOT NULL
                );"
            },
            { DbQuery.CheckUserExists, @"
                SELECT email FROM Users
                WHERE email = @Email LIMIT 1;"
            },
            { DbQuery.InsertNewUser, @"
                INSERT INTO Users (uuid, email)
                VALUES (@Uuid, @Email);"
            }
        };

        public static string GetQuery(DbQuery queryType)
        {
            if (Queries.TryGetValue(queryType, out string query))
            {
                return query;
            }
            throw new ArgumentException($"Query for type {queryType} not found.");
        }
    }
}
