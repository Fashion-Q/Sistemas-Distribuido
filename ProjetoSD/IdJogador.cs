using System;
using System.Net.Sockets;

namespace IdJogador
{
    public class Jogador
    {
        TcpClient jogador;
        string nome;
        public Jogador(TcpClient jogador, string nome)
        {
            this.jogador = jogador;
            this.nome = nome;
        }
    }
}
