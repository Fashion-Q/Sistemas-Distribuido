using GerenciadorMatchCliente;
using IdJogador;
using System.Net.Sockets;
using System.Net.WebSockets;

namespace GerenciarPartidaCliente
{
    public class GerenciarPartida
    {
        private readonly Jogador jogador;
        public GerenciarPartida(Jogador jogador)
        {
            this.jogador = jogador;
        }


        public void Run()
        {
            Console.WriteLine("Esperando servidor responder...");
            try
            {
                jogador.DescartarBuffer();
                string? str = jogador.ReadLine();
                if (str != null && str.ToLower().Contains("digite seu nome"))
                {
                    Console.Write("Digite seu nome: ");
                    str = Console.ReadLine() ?? "Anonymous";
                    jogador.Nome = str;
                    jogador.WriteLine(jogador.Nome);
                    /*Random random = new Random();
                    int numeroAleatorio = random.Next(1, 21);
                    Thread.Sleep(250);
                    jogador.Nome = "Jogador " + numeroAleatorio;
                    Console.WriteLine("Informando nome para o servidor: " + jogador.Nome);
                    jogador.WriteLine(jogador.Nome);*/
                }
                else
                {
                    Console.WriteLine(str);
                    Exit("Servidor nao pediu o nome... ");
                    return;
                }
                // Recebe mensagem do servidor se a partida vai ser encontrada
                string mensagemDoServidor = "";
                MatchCliente matchCliente = new(jogador);
                while (!mensagemDoServidor.ToLower().Contains("partida encontrada"))
                {
                    mensagemDoServidor = jogador.ReadLine() ?? "";
                    if (mensagemDoServidor.ToLower().Contains("setar partida"))
                    {
                        Console.WriteLine("### SET PLAYER ###");

                        string[] particao = mensagemDoServidor.Split("|");
                        particao[0] = particao[0].Trim();
                        particao[1] = particao[1].Trim();
                        particao[2] = particao[2].Trim();
                        particao[3] = particao[3].Trim();
                        jogador.Nome = particao[1].Trim();
                        matchCliente.NomeJogador2 = particao[2].Trim();
                        jogador.EstaJogandoComBola = particao[3].Trim() == "1" ? true : false;
/*                        Console.WriteLine();
                        Console.WriteLine("Nome1: " + jogador.Nome);
                        Console.WriteLine("Nome2: " + matchCliente.NomeJogador2);
                        Console.WriteLine("Sou Bola: " + jogador.EstaJogandoComBola);*/
                    }
                }
                //Console.WriteLine("PARTIDA ENCONTRADA");
                new Thread(matchCliente.Run).Start();
            }
            catch (IOException)
            {
                //Console.WriteLine(ex.ToString());
                Exit("\n### ERRO DE CONEXAO ###");
            }

            Console.ReadKey();
        }
        public void Exit(string str)
        {
            try
            {
                jogador.Conexao.Close();
                Console.WriteLine("Deu algo errado... \n" + str);
                Console.ReadKey();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("### Exit ###");
            }
            jogador.Conexao.Close();
        }
    }
}
