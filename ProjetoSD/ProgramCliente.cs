using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

class ProgramCliente
{
    static void Main(string[] args)
    {
        TcpClient clienteSocket = new TcpClient("127.0.0.1", 8001);

        NetworkStream networkStream = clienteSocket.GetStream();
        StreamReader reader = new StreamReader(networkStream);
        StreamWriter writer = new StreamWriter(networkStream);
        Console.WriteLine("Aguardando servidor...");
        string leitura = reader.ReadLine();
        Console.WriteLine("Servidor: " + leitura);
        // Envia o nome do arquivo para o servidor
        writer.WriteLine(Console.ReadLine());
        writer.Flush();

        // Recebe mensagem do servidor
        string mensagemDoServidor = reader.ReadLine();
        Console.WriteLine("Mensagem recebida: " + mensagemDoServidor);

        clienteSocket.Close();
        Console.ReadKey();
    }
}
