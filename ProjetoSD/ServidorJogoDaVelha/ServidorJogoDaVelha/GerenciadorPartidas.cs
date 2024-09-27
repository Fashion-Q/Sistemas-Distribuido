using System.Net.Sockets;
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
                Thread aux = new Thread(() => HandleCliente(j1));
                aux.Start();
                Console.WriteLine("Recebi jogador1");
            }
            else if (j2 == null || !j2.Conexao.Connected)
            {
                j2 = new(conexaoCliente, false);
                Thread aux = new Thread(() => HandleCliente(j2));
                aux.Start();
                Console.WriteLine("Recebi jogador2");
            }
            else
            {
                new Thread(() => MatchIsBusy(new Jogador(conexaoCliente, true))).Start();
            }
        }

        public void HandleCliente(Jogador jogador)
        {
            Thread.Sleep(50);
            jogador.Writer.WriteLine("Digite seu nome:");
            jogador.Writer.Flush();
            Console.WriteLine("Esperando o nome de [" + (jogador.EstaJogandoComBola ? "BOLA" : "X") + "]");

            try
            {
                DateTime startTime = DateTime.Now;
                string? nome = null;
                while ((DateTime.Now - startTime).TotalSeconds < 5)
                {
                    if (jogador.Fluxo.DataAvailable)
                    {
                        nome = jogador.Reader.ReadLine();
                        break;
                    }
                }
                if (nome == null)
                {
                    jogador.Conexao.Close();
                    Console.WriteLine("### Jogador demorou a informar o nome, cancelando conexao");
                    return;
                }
                Console.WriteLine("Jogador respondeu: " + nome);
                jogador.Nome = nome;
                jogador.Nome = (jogador.Nome.Length == 0 ? "Anonimous" : jogador.Nome);
                jogador.JogadorEstaOcupado = false;
                if (!TeveNomeDuplicado())
                    AcionarPartida();
            }
            catch (IOException)
            {
                //Console.WriteLine(ex.ToString());
                //Console.WriteLine("EXCEPTION");
                jogador.Conexao.Close();
            }
        }
        public bool TeveNomeDuplicado()
        {
            bool teveError = false;
            if (j1 != null && j2 != null)
            {
                if (j1.Nome == j2.Nome)
                {
                    teveError = true;
                    try
                    {
                        j1.Writer.WriteLine("error");
                        j1.Writer.Flush();
                        j1.Conexao.Close();
                        j1 = null;
                        Console.WriteLine("Enviei erro para [j1]");
                    }
                    catch (IOException)
                    {
                        j1?.Conexao.Close();
                        j1 = null;
                    }
                    try
                    {
                        Console.WriteLine("Enviei erro para [j2]");
                        j2.Writer.WriteLine("error");
                        j2.Writer.Flush();
                        j2.Conexao.Close();
                        j2 = null;
                    }
                    catch (IOException)
                    {
                        j2?.Conexao.Close();
                        j2 = null;
                    }
                }
            }
            return teveError;
        }
        public static void MatchIsBusy(Jogador jogador)
        {
            try
            {
                jogador.Writer.WriteLine("error");
                jogador.Writer.Flush();
            }
            catch (IOException)
            {
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
            if (!j1.Conexao.Connected)
            {
                j1.Conexao.Close();
                j1 = null;
                return;
            }
            if (!j2.Conexao.Connected)
            {
                j2.Conexao.Close();
                j2 = null;
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
                if (TentarVerificarSeOsDoisEstaoRealmenteConectado(true))
                {
                    //Console.WriteLine("### Partida inicializada! ###");
                    MatchFound();
                    if (!j1.Conexao.Connected || !j2.Conexao.Connected)
                    {
                        Console.WriteLine("Erro Após Match Found");
                        return;
                    }
                    _ = new PartidaPvP(new Jogador(j1.Conexao, true, j1.Nome), new Jogador(j2.Conexao, false, j2.Nome));
                    j1 = null;
                    j2 = null;
                }

            }
        }

        public void MatchFound()
        {
            try
            {
                j1?.Writer.WriteLine("PARTIDA ENCONTRADA");
                j1?.Writer.Flush();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString() + " | ###############");
                j1?.Conexao.Close();
            }

            try
            {
                j2?.Writer.WriteLine("PARTIDA ENCONTRADA");
                j2?.Writer.Flush();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString() + " | ###############");
                j2?.Conexao.Close();
            }
        }

        public bool TentarVerificarSeOsDoisEstaoRealmenteConectado(bool shouldRepeat)
        {
            TentarMatch = false;
            bool auxJ1;
            bool auxJ2;

            try
            {
                j1?.Writer.WriteLine("setar partida |" + j1.Nome + " | " + j2?.Nome + " | 1");
                j1?.Writer.Flush();
                Console.WriteLine("### Setando partida de [J1] ###");
                auxJ1 = true;
                if (j1 == null)
                    auxJ1 = false;
            }
            catch (IOException)
            {
                Console.WriteLine("### ERROR ### SETAR PARTIDA J1");
                auxJ1 = false;
                j1?.Conexao.Close();
            }
            try
            {
                j2?.Writer.WriteLine("setar partida |" + j1?.Nome + " | " + j2?.Nome + " | 2");
                j2?.Writer.Flush();
                Console.WriteLine("### Setando partida de [J2] ###");
                auxJ2 = true;
                if (j2 == null)
                    auxJ2 = false;
            }
            catch (IOException)
            {
                Console.WriteLine("### ERROR ### SETAR PARTIDA J2");
                auxJ2 = false;
                j2?.Conexao.Close();
            }
            if (auxJ1 && auxJ2)
                TentarMatch = true;
            Thread.Sleep(50);
            if (TentarMatch && shouldRepeat)
                return TentarVerificarSeOsDoisEstaoRealmenteConectado(false);
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