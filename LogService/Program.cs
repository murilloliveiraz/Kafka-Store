using Producer;

Console.WriteLine("Iniciando serviços de consumidor Kafka...");
var logService = new LogService();

var cts = new CancellationTokenSource();

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
    Console.WriteLine("\nSinal de cancelamento recebido. Aguardando serviços finalizarem...");
};

var logTask = Task.Run(() => logService.Start(cts.Token));

await Task.WhenAll(logTask);

Console.WriteLine("Todos os serviços de consumidor foram finalizados.");
