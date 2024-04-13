using GerenciadorPartidas;
using System.Net.Sockets;
using System.Net;

namespace ServidorJogoDaVelha
{
    internal class Program
    {
        static void Main()
        {
            TcpListener servidor = new TcpListener(IPAddress.Any, 8001);
            servidor.Start();
            GerenciadorPartidasClass gerenciadorPartida = new GerenciadorPartidasClass();
            //Thread gerenciadorPartida = new Thread()
            Console.WriteLine("O pai ta ON");
            while (true)
            {
                TcpClient cliente = servidor.AcceptTcpClient();
                gerenciadorPartida.ReceberJogador(cliente);
            }
        }
    }
}