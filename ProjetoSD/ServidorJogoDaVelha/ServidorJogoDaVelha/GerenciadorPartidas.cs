using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using IdJogador;
using Partida;

namespace GerenciadorPartidas
{
    public class GerenciadorPartidasClass
    {
        private Jogador? j1;
        private Jogador? j2;
        private bool TentarMatch { get; set; } = false;
        public void ReceberJogador(TcpClient conexaoCliente)
        {
            if (j1 == null || !j1.Conexao.Connected)
            {
                j1 = new(conexaoCliente, true);
                //Console.WriteLine("Pegou Jogador1");
                Thread aux = new Thread(() => HandleCliente(j1));
                aux.Start();
            }
            else if (j2 == null || !j2.Conexao.Connected)
            {
                j2 = new(conexaoCliente, false);
                //Console.WriteLine("Pegou oJogador2");
                Thread aux = new Thread(() => HandleCliente(j2));
                aux.Start();
            }
            else
            {
                new Thread(() => MatchIsBusy(new Jogador(conexaoCliente, true))).Start();
            }
        }

        public void HandleCliente(Jogador jogador)
        {
            Thread.Sleep(500);
            jogador.WriteLine("Digite seu Nome:");
            Console.WriteLine("Esperando o nome de [" + (jogador.EstaJogandoComBola ? 1 : 2) + "]");

            try
            {
                DateTime startTime = DateTime.Now;
                string? nome = null;
                while ((DateTime.Now - startTime).TotalSeconds < 20)
                {
                    if (jogador.Fluxo.DataAvailable)
                    {
                        nome = jogador.ReadLine();
                        break;
                    }
                }
                if(nome == null)
                {
                    jogador.Conexao.Close();
                    Console.WriteLine("### Jogador demorou a informar o nome, cancelando conexao");
                    return;
                }
                Console.WriteLine("Jogador respondeu: " + nome);
                jogador.Nome = nome;
                jogador.Nome = (jogador.Nome.Length == 0 ? "Anonimous" : jogador.Nome);
                jogador.JogadorEstaOcupado = false;
                AcionarPartida();
            }
            catch (IOException)
            {
                //Console.WriteLine(ex.ToString());
                //Console.WriteLine("EXCEPTION");
                jogador.Conexao.Close();
            }
        }
        public static void MatchIsBusy(Jogador jogador)
        {
            try
            {
                Thread.Sleep(150);
                jogador.WriteLine("Servidor ocupado, tente novamente mais tarde");
            }
            catch (IOException)
            {
                //Console.WriteLine(ex.ToString());
                //Console.WriteLine("### Math Is Busy Exception ###");
                jogador.Conexao.Close();
            }

            try
            {
                Thread.Sleep(150);
                jogador.Conexao.Close();
            }
            catch (IOException)
            {
                //Console.WriteLine(ex.ToString());
                //Console.WriteLine("### Math Is Busy Exception ###");
                jogador.Conexao.Close();
            }
        }

        public void AcionarPartida()
        {
            if (j1 == null || j2 == null)
            {
                //Console.WriteLine("Um dos dois são nulos, esperando outro...");
                return;
            }
            if (!j1.Conexao.Connected || !j2.Conexao.Connected)
            {
                //Console.WriteLine("Um dos dois não está conectado.");
                return;
            }
            if (j1.JogadorEstaOcupado)
            {
                //Console.WriteLine("J1 ocupado ainda");
                return;
            }
            if (j2.JogadorEstaOcupado)
            {
                //Console.WriteLine("J2 ocupado ainda");
                return;
            }
            if (j1.Conexao.Connected && j2.Conexao.Connected)
            {
                /*Timer timer = new Timer(_ =>
                {
                    Console.WriteLine("Função lambda executada após 1 segundo.");
                }, null, 1000, Timeout.Infinite);*/
                Thread.Sleep(150);
                if (TentarVerificarSeOsDoisEstaoRealmenteConectado())
                {
                    Console.Clear();
                    //Console.WriteLine("### Partida inicializada! ###");
                    MatchFound();
                    _ = new PartidaPvP(new Jogador(j1.Conexao, true,j1.Nome), new Jogador(j2.Conexao, false,j2.Nome));
                    Thread.Sleep(250);
                    j1 = null;
                    j2 = null;
                }

            }
        }

        public void MatchFound()
        {
            Thread.Sleep(50);
            try
            {
                j1?.WriteLine("PARTIDA ENCONTRADA");
                j2?.WriteLine("PARTIDA ENCONTRADA");
            }
            catch (IOException)
            {
                //Console.WriteLine(ex.ToString());
                j1?.Conexao.Close();
            }
        }

        public bool TentarVerificarSeOsDoisEstaoRealmenteConectado()
        {
            TentarMatch = false;
            bool auxJ1;
            bool auxJ2;
            Thread.Sleep(150);
            try
            {
                j1?.WriteLine("setar partida |" + j1.Nome + " | " + j2?.Nome + " | 1");
                auxJ1 = true;
            }
            catch (IOException)
            {
                //Console.WriteLine(ex.ToString());
                auxJ1 = false;
                j1?.Conexao.Close();
            }
            try
            {
                j2?.WriteLine("setar partida |" + j1?.Nome + " | " + j2.Nome + " | 2");
                auxJ2 = true;
            }
            catch (IOException)
            {
                //Console.WriteLine(ex.ToString());
                auxJ2 = false;
                j2?.Conexao.Close();
            }
            if (auxJ1 && auxJ2)
                TentarMatch = true;
            return TentarMatch;
        }

        public void CancelarPartida()
        {
            j1 = null;
            j2 = null;
            //Console.WriteLine("Cancelando Partida");
        }


    }
}