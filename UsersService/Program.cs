using Producer;

Console.WriteLine("Iniciando serviços de consumidor Kafka...");

var userService = new UsersService();

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
    Console.WriteLine("\nSinal de cancelamento recebido. Aguardando serviços finalizarem...");
};

var createUserTask = Task.Run(() => userService.Start(cts.Token));

await Task.WhenAll(createUserTask);

Console.WriteLine("Todos os serviços de consumidor foram finalizados.");