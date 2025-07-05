using Producer;

Console.WriteLine("Iniciando serviços de consumidor Kafka...");
var fraudDetectorService1 = new FraudDetectorService();
var fraudDetectorService2 = new FraudDetectorService();

var cts = new CancellationTokenSource();

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
    Console.WriteLine("\nSinal de cancelamento recebido. Aguardando serviços finalizarem...");
};

var fraudTask1 = Task.Run(() => fraudDetectorService1.Start(cts.Token));
var fraudTask2 = Task.Run(() => fraudDetectorService2.Start(cts.Token));

await Task.WhenAll(fraudTask1, fraudTask2);

Console.WriteLine("Todos os serviços de consumidor foram finalizados.");
