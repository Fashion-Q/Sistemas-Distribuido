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
                    Console.Write("Informando nome para o servidor: ");
                    Random random = new Random();
                    int numeroAleatorio = random.Next(1, 21);
                    Thread.Sleep(250);
                    jogador.Nome = "Jogador " + numeroAleatorio;
                    jogador.WriteLine(jogador.Nome);
                    /*str = Console.ReadLine();
                    jogador.WriteLine(str ?? "");*/
                }
                else
                {
                    Console.WriteLine(str);
                    Exit("Servidor nao pediu o nome... ");
                    return;
                }
                // Recebe mensagem do servidor se a partida vai ser encontrada
                Console.WriteLine("Esperando servidor responder MATCH | VEZ");
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

                        Console.WriteLine(particao[1]);
                        Console.WriteLine(particao[2]);
                        Console.WriteLine(particao[3]);
                        jogador.Nome = particao[1].Trim();
                        matchCliente.NomeJogador2 = particao[2].Trim();
                        jogador.EstaJogandoComBola = particao[3].Trim() == "1" ? true : false;
                        Console.WriteLine();
                        Console.WriteLine(jogador.Nome);
                        Console.WriteLine(matchCliente.NomeJogador2);
                        Console.WriteLine(jogador.EstaJogandoComBola);
                    }
                }
                Console.WriteLine("PARTIDA ENCONTRADA");
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString());
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
