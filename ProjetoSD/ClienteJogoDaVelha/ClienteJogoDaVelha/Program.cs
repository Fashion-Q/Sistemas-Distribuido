using IdJogador;
using System.Net.Sockets;
using GerenciarPartidaCliente;

namespace ClienteJogoDaVelha
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GerenciarPartida g;
            try
            {
                TcpClient clienteSocket = new TcpClient("127.0.0.1", 8001);
                g = new GerenciarPartida(new Jogador(clienteSocket, true));
                Thread t1 = new Thread(g.Run);
                t1.Start();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("\n Conexao Falhou");
            }
        }
    }
}
