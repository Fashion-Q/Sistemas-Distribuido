using IdJogador;

namespace Partida
{
    public class PartidaPvP
    {
        public Jogador GetJogadorAtual => vez == Vez.j1 ? j1 : j2;
        private Jogador j1;
        private Jogador j2;
        private int[][] matrizPartida;
        private EstadoDaPartida estadoPartida;
        private Vez vez;
        private ValidarJogada validarJogada;
        private bool BolaComeca { get; set; } = true;
        private bool JogadorBolaQuerContinuar { get; set; } = false;
        private bool JogadorXQuerContinuar { get; set; } = false;

        public PartidaPvP(Jogador j1, Jogador j2)
        {
            this.j1 = j1;
            this.j2 = j2;
            matrizPartida = new int[3][];
            j1.JogadorEstaOcupado = false;
            j2.JogadorEstaOcupado = false;
            for (int i = 0; i < 3; i++)
                matrizPartida[i] = new int[3];
            ResetarMatriz();
            printarMatriz();
            new Thread(Run).Start();
        }

        public void ResetarMatriz()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    matrizPartida[i][j] = 0;
        }

        public void printarMatriz()
        {
            Console.WriteLine("### PRINTAR MATRIZ ###");
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if(j != 0)
                        Console.Write(" | ");
                    Console.Write(matrizPartida[i][j]);
                }
                Console.WriteLine();
            }
        }
        public void Run()
        {
            estadoPartida = EstadoDaPartida.ContinuarPartida;
            while(estadoPartida == EstadoDaPartida.ContinuarPartida)
            {
                PartidaEmAndamento();
                Lobby();
            }
            FinalizarPartida();
        }
        public void Lobby()
        {
            if (estadoPartida == EstadoDaPartida.Desistencia)
                EnunciarDesistencia();
            if(estadoPartida != EstadoDaPartida.Desistencia)
            {
                j1.JogadorEstaOcupado = true;
                j2.JogadorEstaOcupado = true;
                JogadorBolaQuerContinuar = false;
                JogadorXQuerContinuar = false;
                new Thread(() => EsperandoRevanche(j1)).Start();
                new Thread(() => EsperandoRevanche(j2)).Start();
                while(j1.JogadorEstaOcupado && j2.JogadorEstaOcupado && estadoPartida != EstadoDaPartida.Desistencia)
                {
                    Thread.Sleep(150);
                }
            }
            if(JogadorBolaQuerContinuar && JogadorXQuerContinuar && estadoPartida != EstadoDaPartida.Desistencia)
            {
                BolaComeca = !BolaComeca;
                estadoPartida = EstadoDaPartida.ContinuarPartida;
            }
        }
        public void EsperandoRevanche(Jogador jogador)
        {
            CancellationTokenSource cancellatinoTokenSource = new CancellationTokenSource();
            cancellatinoTokenSource.CancelAfter(TimeSpan.FromSeconds(20));
            try
            {
                string str = "vencedor: " + (estadoPartida == EstadoDaPartida.BolaGanhou ? "bola" : "x");
                Thread.Sleep(150);
                jogador.WriteLine(str);
                jogador.DescartarBuffer();
                str = jogador.ReadLine();
                jogador.JogadorEstaOcupado = false;
                if(str.ToLower().Contains("continuar"))
                {
                    if (jogador.EstaJogandoComBola)
                        JogadorBolaQuerContinuar = true;
                    if (!jogador.EstaJogandoComBola)
                        JogadorXQuerContinuar = true;
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Tempo limite atingido. O Jogador não respondeu a tempo de 20 segundos: " + jogador.Nome);
                jogador.Conexao.Close();
                estadoPartida = EstadoDaPartida.Desistencia;
                EnunciarDesistencia();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("EXCEPTION");
                EnunciarDesistencia();
            }
        }
        public void PartidaEmAndamento()
        {
            vez = BolaComeca ? Vez.j1 : Vez.j2;
            while (estadoPartida == EstadoDaPartida.ContinuarPartida)
            {
                AvisarJogadorVez();
                ReceberJogada();
                EnviarMatrizParaJogadores();
                VerificarSeJogadorGanhou();
                Thread.Sleep(10);
            }
        }
        public void VerificarSeJogadorGanhou()
        {
            if(VerificarVencedorLinhaHorizontalOuVertical(1))
            {
                estadoPartida = EstadoDaPartida.BolaGanhou;
                return;
            }
            if (VerificarVencedorLinhaHorizontalOuVertical(2))
            {
                estadoPartida = EstadoDaPartida.XGanhou;
                return;
            }
            if (VerificarVencedorLinhaCruzada(1))
            {
                estadoPartida = EstadoDaPartida.BolaGanhou;
                return;
            }
            if (VerificarVencedorLinhaCruzada(2))
            {
                estadoPartida = EstadoDaPartida.XGanhou;
                return;
            }
        }

        public bool VerificarVencedorLinhaCruzada(int bolaOuX)
        {
            int contador = 0;
            //verifica linhas:  0 0 | 1 1 | 2 2
            //
            //
            //  1 0 0
            //  0 1 0
            //  0 0 1
            for (int ij=0;ij<3;ij++)
            {
                if (matrizPartida[ij][ij] == bolaOuX)
                    contador++;
                else
                    break;
            }
            if (contador == 3)
                return true;
            contador = 0;
            // verifica linhas:  0 2 || 1 1 || 2 0
            //  0 0 1
            //  0 1 0
            //  1 0 0
            for(int i=0;i<3;i++)
            {
                for(int j=2;j>=0;j--)
                {
                    if (matrizPartida[i][j] == bolaOuX)
                        contador++;
                    else
                        break;
                }
            }
            if (contador == 3)
                return true;
            return false;
        }

        public bool VerificarVencedorLinhaHorizontalOuVertical(int bolaOuX)
        {
            int contador;
            //1 = bola | 2 = X
            //verificar linha horizontal
            for(int i=0;i<3;i++)
            {
                contador = 0;
                for(int j=0;j<3;)
                {
                    if (matrizPartida[i][j] == bolaOuX)
                        contador++;
                }
                if (contador == 3)
                    return true;
            }
            //verificar linha vertical
            for (int i = 0; i < 3; i++)
            {
                contador = 0;
                for (int j = 0; j < 3;)
                {
                    if (matrizPartida[j][i] == bolaOuX)
                        contador++;
                }
                if (contador == 3)
                    return true;
            }
            return false;
        }

        public void EnviarMatrizParaJogadores()
        {
            try
            {
                string str = "";
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        str += matrizPartida[i][j].ToString();
                    }
                }
                printarMatriz();
                Console.Write("Matriz Enviada: " + str);
                Thread.Sleep(150);
                GetJogadorAtual.WriteLine(str);
                InverterVez();
                GetJogadorAtual.WriteLine(str);
                Thread.Sleep(150);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("EXCEPTION");
                EnunciarDesistencia();
            }
        }

        public void InverterVez() => vez = (vez == Vez.j1 ? Vez.j2 : Vez.j1);

        public void ReceberJogada()
        {
            while (validarJogada == ValidarJogada.JogadaInvalida && estadoPartida == EstadoDaPartida.ContinuarPartida)
            {
                try
                {
                    GetJogadorAtual.DescartarBuffer();
                    string str = GetJogadorAtual.ReadLine();
                    if (JogadaFoiBemSucedida(str))
                        validarJogada = ValidarJogada.JogadaValida;
                    else
                    {
                        Thread.Sleep(150);
                        GetJogadorAtual.WriteLine("vez");
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine("EXCEPTION");
                    EnunciarDesistencia();
                }
            }
        }
        public void AvisarJogadorVez()
        {
            Thread.Sleep(150);
            try
            {
                GetJogadorAtual.WriteLine("vez");
                validarJogada = ValidarJogada.JogadaInvalida;
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("EXCEPTION: AvisarJogadorVez");
                EnunciarDesistencia();
            }
        }


        public bool JogadaFoiBemSucedida(string str)
        {
            if (str.Length != 2)
            {
                Console.WriteLine("Erro na JogadaFoiBemSucedida: Tamanho da jogada != 2");
                return false;
            }
            if (!str.All(char.IsDigit))
            {
                Console.WriteLine("Erro na JogadaFoiBemSucedida: Nao é digito");
                return false;
            }
            int i = -1, j = -1;
            _ = int.TryParse(str[0].ToString(), out i);
            _ = int.TryParse(str[1].ToString(), out j);
            if (i != -1 && j != -1)
            {
                if (i >= 0 && i <=  2 && j >= 0 && j <= 2)
                {
                    int BolaOuX = GetJogadorAtual.EstaJogandoComBola ? 1 : 2;
                    if (matrizPartida[i][j] == 0)
                        matrizPartida[i][j] = BolaOuX;
                    else
                    {
                        Console.WriteLine("JogadaFoiBemSucedida: Retornou False 1");
                        return false;
                    }
                    return true;
                }
            }
            Console.WriteLine("JogadaFoiBemSucedida: Retornou False 2");
            return false;
        }

        public void EnunciarDesistencia()
        {
            vez = (vez == Vez.j1 ? Vez.j2 : Vez.j1);
            try
            {
                GetJogadorAtual.WriteLine("desistencia");
            }
            catch (IOException exx)
            {
                Console.WriteLine(exx.ToString());
                Console.WriteLine("Outro Error");

            }
            estadoPartida = EstadoDaPartida.Desistencia;
        }
        public void FinalizarPartida()
        {
            try
            {
                j1.Conexao.Close();
            }
            catch (IOException exx)
            {
                Console.WriteLine(exx.ToString());

            }
            try
            {
                j2.Conexao.Close();
            }
            catch (IOException exx)
            {
                Console.WriteLine(exx.ToString());
            }
        }
    }
    public enum EstadoDaPartida
    {
        ContinuarPartida,
        BolaGanhou,
        XGanhou,
        Desistencia
    }
    public enum Vez
    {
        j1,
        j2
    }
    public enum ValidarJogada
    {
        JogadaValida,
        JogadaInvalida
    }
}