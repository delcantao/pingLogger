// See https://aka.ms/new-console-template for more information

using System.Net.NetworkInformation;


Console.BackgroundColor = ConsoleColor.Black;



Console.ForegroundColor = ConsoleColor.DarkGray;
Console.WriteLine("Este programa realiza um ping em um endereço IP ou nome de domínio");
Console.WriteLine("e registra apenas os logs de erros em uma pasta específica.");
Console.WriteLine("");
Console.ForegroundColor = ConsoleColor.DarkGray;
Console.WriteLine("Exemplos de input válidos: ");
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("   186.202.137.158");
Console.WriteLine("   thdc2.sistemasth.com.br");
Console.WriteLine("   www.sistemasth.com.br");
Console.WriteLine("   192.168.1.166");
Console.WriteLine("");
Console.ForegroundColor = ConsoleColor.DarkGray;
Console.WriteLine("Exemplos de input inválidos: ");
Console.ForegroundColor = ConsoleColor.Red;
Console.WriteLine("   666202.137.158");
Console.WriteLine("   https://thdc2.sistemasth.com.br");
Console.WriteLine("   sistemasth");
Console.WriteLine("   692.668.1.866");
Console.WriteLine("");
Console.ForegroundColor = ConsoleColor.DarkGray;
Console.WriteLine("Os logs, em caso de erro, serão gravados neste formato:");
Console.WriteLine("");
Console.Write("   ");
Console.BackgroundColor = ConsoleColor.Gray;
Console.ForegroundColor = ConsoleColor.Black;
Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff}] - Ping to [nome-do-host] failed. Status: [status-do-ping]");

Console.BackgroundColor = ConsoleColor.Black;
Console.ForegroundColor = ConsoleColor.White;
Console.WriteLine("");
Console.WriteLine("");

Console.Write("Informe o IP/domínio: ");
var host = Console.ReadLine()?.Trim();

// var host = "186.202.137.158";
var pingSender = new Ping(); 
var buffer = new byte[32]; 
var timeout = 5000;
var options = new PingOptions(64, true);
var filePath =  Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) ?? @"C:\Temp\", $"{host}.LOG");


var status = new Status();
while (true)
{
    var pingResult = "";
    var fileName = Path.Combine(filePath, $"{DateTime.Now:yyyyMMdd}_PING.log");
    Directory.CreateDirectory(filePath);
    
    try
    {
        // Send the ping request
        var reply = pingSender.Send(host, timeout, buffer, options);

        // Check the result
        if (reply.Status == IPStatus.Success)
        {
            status.Success++;
            pingResult = $"Ping to {host} successful. RTT {reply.RoundtripTime} | TTL {reply.Options.Ttl} | Buffer size: {reply.Buffer.Length}";
            
        }
        else
        {
            status.Error++;
            pingResult = $"Ping to {host} failed. Status: {reply.Status}";

            await File.AppendAllTextAsync(fileName, $"[{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff}] - Ping to {host} failed. Status: {reply.Status}\r\n");
            
        }
    }
    catch (PingException ex)
    {
        status.Error++;
        await File.AppendAllTextAsync(fileName, $"[{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff}] - Ping to {host} failed. Message: {ex.Message}\r\n"); 
    }
    catch (Exception ex)
    {
        status.NotPingError++;
        await File.AppendAllTextAsync(fileName + ".err", $"[{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff}] - Ping to {host} failed. General Message: {ex.Message}\r\n"); 
    }
    
    
    Console.Clear();
    Console.BackgroundColor = ConsoleColor.Black;
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine(pingResult);
    Console.WriteLine($"Caminho do log de erros: {fileName}");
    Console.Write($"Sucessos: ");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"{status.Success}");
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write($"Erros: ");
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"{status.Error}");
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine($"Erros não relacionado ao ping: {status.NotPingError}");
    await Task.Delay(1000);
}


public class Status
{
    public long Success { get; set; }
    public long Error { get; set; }
    public long NotPingError { get; set; }
}