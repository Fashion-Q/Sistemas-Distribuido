using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using GerenciadorPartidas;

class ProgramServer
{
    static void Main(string[] args)
    {
        TcpListener servidor = new TcpListener(IPAddress.Any, 8001);
        servidor.Start();
        GerenciadorPartidasClass gerenciadorPartida = new GerenciadorPartidasClass();
        //Thread gerenciadorPartida = new Thread()
        while (true)
        {
            Console.WriteLine("Aguardando uma conexão...");


            TcpClient cliente = servidor.AcceptTcpClient();
            Console.WriteLine("Conexão aceita.");
            gerenciadorPartida.ReceberJogador(cliente);
            //NetworkStream fluxo = cliente.GetStream();

            //StreamReader reader = new StreamReader(fluxo);
            //StreamWriter writer = new StreamWriter(fluxo);

            //// Recebe o nome do arquivo do cliente
            //string arquivo = reader.ReadLine();
            //Console.WriteLine("Nome do arquivo recebido: " + arquivo);

            //// Simula processamento do arquivo e envia uma mensagem de resposta
            //string resposta = "Arquivo '" + arquivo + "' recebido com sucesso!";
            //writer.WriteLine(resposta);
            //writer.Flush();

            //cliente.Close();
            //servidor.Stop();
            //Console.ReadKey();
        }
    }
}



