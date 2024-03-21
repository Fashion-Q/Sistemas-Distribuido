using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using IdJogador;

namespace GerenciadorPartidas
{
    public class GerenciadorPartidasClass
    {
        TcpClient jogador1;
        TcpClient jogador2;
        public void ReceberJogador(TcpClient jogador)
        {
            if (jogador1 == null || (jogador1 != null && !jogador1.Connected))
            {
                jogador1 = jogador;
                Console.WriteLine("Pegou jogador1");
            }
            else if (jogador2 == null || (jogador2 != null && !jogador2.Connected))
            {
                Console.WriteLine("Pegou jogador2");
                jogador2 = jogador;
            }
            if (jogador1 != null && jogador.Connected && jogador2 != null && jogador.Connected)
            {
                Thread j1 = new Thread(() => HandleCliente(jogador1, 1));
                Thread j2 = new Thread(() => HandleCliente(jogador2, 2));
                j1.Start();
                j2.Start();
            }
        }

        public void HandleCliente(TcpClient jogador, int teste)
        {
            NetworkStream fluxo = jogador.GetStream();
            StreamReader reader = new StreamReader(fluxo);
            StreamWriter writer = new StreamWriter(fluxo);
            Console.WriteLine("Mandando mensagem para o jogador: " + teste);
            writer.WriteLine("Digite seu nome: ");
            writer.Flush();
            Console.WriteLine("Esperando jogador responder: " + teste);
            string nome = reader.ReadLine();
            Console.WriteLine("Jogador respondeu: " + nome);
            Jogador auxJogador = new Jogador(jogador, nome);

            ////
            writer.WriteLine("Enviando-o para uma partida: " + nome);
            writer.Flush();
        }

        public void VerificarJogadores()
        {

        }
    }
}