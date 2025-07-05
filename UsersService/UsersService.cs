using BuildingBlocks.Helpers;
using BuildingBlocks.Interfaces;
using BuildingBlocks.Models;
using BuildingBlocks.Services;
using Confluent.Kafka;
using Dapper;
using Microsoft.Data.Sqlite;
using Producer.Data;

namespace Producer;

public class UsersService : IConsumerFunction<string, Order>
{
    private readonly string _connectionString;
    public UsersService()
    {
        var dbPath = "users_database.db";
        _connectionString = $"Data Source={dbPath}";

        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            var createTableQuery = DatabaseQueries.GetQuery(DbQuery.CreateTableUsers);
            connection.Execute(createTableQuery);
            Console.WriteLine("Tabela 'Users' verificada/criada no banco de dados SQLite.");
        }
    }

    public void Consume(ConsumeResult<string, Order> record)
    {
        var userId = Guid.NewGuid().ToString();
        var order = record.Message.Value;

        Console.WriteLine("---------------------");
        Console.WriteLine("CreateUserService: Processando nova ordem para criar usuário");
        Console.WriteLine($"Key (UserId): {userId}");
        Console.WriteLine($"Order Value: {order.ToString()}");
        Console.WriteLine($"Partição: {record.Partition.Value}");
        Console.WriteLine($"Offset: {record.Offset.Value}");

        var userEmail = record.Message.Key;
        
        try
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                if (IsNewUser(connection, userEmail))
                {
                    InsertNewUser(connection, userId, userEmail);
                }
                else
                {
                    Console.WriteLine($"Usuário com email {userEmail} já existe. Não será criado novamente.");
                }
            }
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"CreateUserService: Erro de banco de dados ao processar usuário: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CreateUserService: Erro inesperado ao processar usuário: {ex.Message}");
        }

        Console.WriteLine("CreateUserService: Processamento de usuário concluído.");
    }

    private bool IsNewUser(SqliteConnection connection, string email)
    {
        var checkUserQuery = DatabaseQueries.GetQuery(DbQuery.CheckUserExists);
        var existingUuid = connection.QuerySingleOrDefault<string>(checkUserQuery, new { Email = email });
        return string.IsNullOrEmpty(existingUuid);
    }

    private void InsertNewUser(SqliteConnection connection, string uuid, string email)
    {
        var insertUserQuery = DatabaseQueries.GetQuery(DbQuery.InsertNewUser);
        connection.Execute(insertUserQuery, new { Uuid = uuid, Email = email });
        Console.WriteLine($"Usuário '{uuid}' com email '{email}' adicionado com sucesso.");
    }

    public async Task Start(CancellationToken cancellationToken)
    {
        using (var kafkaService = new KafkaService<string, Order>(
            "CreateUserServiceGroup",
            "ECOMMERCE_NEW_ORDER",
            this,
            Confluent.Kafka.Deserializers.Utf8,
            new JsonDeserializer<Order>()))
        {
            await kafkaService.Run(cancellationToken);
        }
    }
}