using IdJogador;
using System.Text;

namespace GerenciadorMatchCliente
{
    public class MatchCliente
    {
        public MatchCliente(Jogador jogador)
        {
            this.jogador = jogador;
            matrizPartida = new int[3][];
            for (int i = 0; i < 3; i++)
                matrizPartida[i] = new int[3];
        }
        private readonly Jogador jogador;
        private int[][] matrizPartida;
        private EstadoDaPartida estadoPartida;
        public string NomeJogador2 { get; set; } = "";
        private int Pontuacao1 { get; set; } = 0;
        private int Pontuacao2 { get; set; } = 0;

        private void AtualizarMatriz(string str)
        {
            int contadorStr = 0, aux;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    aux = int.Parse(str[contadorStr].ToString());
                    matrizPartida[i][j] = aux;
                    contadorStr++;
                }
        }

        private void PrintarMatriz()
        {
            Console.WriteLine("### PRINTAR MATRIZ ###");
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (j != 0)
                        Console.Write(" | ");
                    Console.Write(matrizPartida[i][j]);
                }
                Console.WriteLine();
            }
        }

        public void Run()
        {
            estadoPartida = EstadoDaPartida.Continuar;
            while (estadoPartida == EstadoDaPartida.Continuar)
            {
                Console.Clear();
                PrintarMatriz();
                string str = EsperandoRespostaDoServidor();
                Console.WriteLine("SERVIDOR RESPONDEU [" + str + "]");
                if (str.ToLower().Contains("exit"))
                {
                    Exit();
                    continue;
                }
                else if (str.All(char.IsDigit) && str.Length == 9)
                    AtualizarMatriz(str);
                else if (str.ToLower().Contains("vez"))
                    FazerJogada();
                else if (str.ToLower().Contains("vencedor"))
                {
                    Console.WriteLine("### VENCEDOR ###");
                    AnunciarVitoriaOuDerrota(str);
                }
            }
            Exit();
        }

        private void AnunciarVitoriaOuDerrota(string str)
        {
            if (jogador.EstaJogandoComBola && str.ToLower().Contains("bola"))
            {
                str = "Parabéns! Você ganhou";
                Pontuacao1++;
            }
            else if (!jogador.EstaJogandoComBola && str.ToLower().Contains("x"))
            {
                Pontuacao1++;
                str = "Parabéns! Você ganhou";
            }
            else
            {
                Pontuacao2++;
                str = "Você perdeu!";
            }
            Console.WriteLine(str);
            Console.WriteLine("#### PONTUACAO ####");
            Console.WriteLine("Minha pontuacao: [" + jogador.Nome + "] | " + Pontuacao1);
            Console.WriteLine("Oponente:        [" + NomeJogador2 + "] | " + Pontuacao2);
            str = "\n### Deseja Continuar ### [S] | [N] ";
            Console.WriteLine(str);
            LimparBuffDeTeclado();
            str = Console.ReadLine() ?? "N";
            Console.WriteLine("### Minha resposta: [" + str + "]");
            
            try
            {
                Thread.Sleep(1000);
                jogador.WriteLine(str);
                str = jogador.ReadLine();
                if (!str.ToLower().Contains("match"))
                {
                    Console.WriteLine("Partida cancelada");
                    Exit();
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                Exit();
                return;
            }
        }

        private void FazerJogada()
        {
            Thread.Sleep(150);
            try
            {
                Console.WriteLine("Sua vez: Digite dois inteiros entre 0-9 juntos representando a posicao respectivamente da matriz a ser jogada! Ex: 01");
                Console.Write("\nDigite sua jogada: ");
                LimparBuffDeTeclado();
                string str = Console.ReadLine() ?? "";
                jogador.WriteLine(str);
            }
            catch (Exception ex)
            {
                Exit();
                Console.WriteLine(ex.ToString());
            }
        }

        private string EsperandoRespostaDoServidor()
        {
            string str;

            try
            {
                Console.WriteLine("\n### [SUA VEZ] ###\n");
                str = jogador.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Exit();
                return "exit";
            }

            return str;
        }

        private void Exit()
        {
            try
            {
                jogador.Conexao.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("### EXIT ###");
                Console.WriteLine(ex.ToString());
            }
            estadoPartida = EstadoDaPartida.Cancelar;
        }

        private void LimparBuffDeTeclado()
        {
            StringBuilder buffer = new StringBuilder();
            while (Console.KeyAvailable)
            {
                // Lê o próximo caractere do buffer
                char c = Console.ReadKey(true).KeyChar;
                buffer.Append(c);
            }
            if (buffer.Length > 0)
            {
                Console.WriteLine("Conteudo do buffer: " + buffer.ToString());

            }
        }
    }
    public enum EstadoDaPartida
    {
        Continuar,
        Cancelar
    }
}
