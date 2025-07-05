using Producer;

Console.WriteLine("Iniciando serviços de consumidor Kafka...");

var emailService = new EmailService();

var cts = new CancellationTokenSource();

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
    Console.WriteLine("\nSinal de cancelamento recebido. Aguardando serviços finalizarem...");
};

var emailTask = Task.Run(() => emailService.Start(cts.Token));

await Task.WhenAll(emailTask);

Console.WriteLine("Todos os serviços de consumidor foram finalizados.");
