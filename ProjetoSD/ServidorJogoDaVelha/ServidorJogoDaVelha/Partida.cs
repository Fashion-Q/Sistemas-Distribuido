using IdJogador;
using System.Diagnostics;

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
            new Thread(Run).Start();
        }

        public void ResetarMatriz()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    matrizPartida[i][j] = 0;
        }

        public void PrintarMatriz()
        {
            Console.WriteLine("### TABULEIRO ###");
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
            estadoPartida = EstadoDaPartida.ContinuarPartida;
            QuemComeca();
            Thread.Sleep(50);
            while (estadoPartida == EstadoDaPartida.ContinuarPartida)
            {
                PartidaEmAndamento();
                Lobby();
            }
            FinalizarPartida();
        }

        public void QuemComeca()
        {
            if (j1.EstaJogandoComBola)
                vez = Vez.j1;
            else
                vez = Vez.j2;
            //Console.WriteLine("### QUEM COMEÇA ### " + vez.ToString());
        }
        public void Lobby()
        {
            if (estadoPartida == EstadoDaPartida.Desistencia)
                EnunciarDesistencia();
            if (estadoPartida != EstadoDaPartida.Desistencia)
            {
                j1.JogadorEstaOcupado = true;
                j2.JogadorEstaOcupado = true;
                JogadorBolaQuerContinuar = false;
                JogadorXQuerContinuar = false;
                string str = "";
                if(estadoPartida == EstadoDaPartida.Empate)
                {
                    str = "vencedor: empate";
                    InverterVez();
                }
                else
                {
                     str = "vencedor: " + (estadoPartida == EstadoDaPartida.BolaGanhou ? "bola" : "x");
                    if (str.ToLower().Contains("bola"))
                        vez = j1.EstaJogandoComBola ? Vez.j2 : Vez.j1;
                    else if (str.ToLower().Contains('x'))
                        vez = !j1.EstaJogandoComBola ? Vez.j2 : Vez.j1;
                }

                new Thread(() => EsperandoRevanche(j1, str)).Start();
                new Thread(() => EsperandoRevanche(j2, str)).Start();
                //Console.WriteLine("ESPERANDO REVANCHE DE JOGADORES...");
                while ((j1.JogadorEstaOcupado || j2.JogadorEstaOcupado) && estadoPartida != EstadoDaPartida.Desistencia)
                {
                    Thread.Sleep(50);
                }
                //Console.WriteLine("### " + j1.JogadorEstaOcupado + " | " + j2.JogadorEstaOcupado + " | " + estadoPartida.ToString());
            }
            if (JogadorBolaQuerContinuar && JogadorXQuerContinuar && estadoPartida != EstadoDaPartida.Desistencia)
            {
                estadoPartida = EstadoDaPartida.ContinuarPartida;
                AvisarRevanche("MATCH");
            }
            else
            {
                AvisarRevanche("N");
            }
        }

        public void AvisarRevanche(string str)
        {
            Thread.Sleep(50);
            try
            {
                j1.WriteLine(str);
                j2.WriteLine(str);
            }
            catch (Exception)
            {
                //Console.WriteLine(ex.ToString());
                EnunciarDesistencia();
            }
        }
        public void EsperandoRevanche(Jogador jogador, string str)
        {
            Thread.Sleep(50);
            try
            {
                jogador.WriteLine(str);
                jogador.DescartarBuffer();
                DateTime startTime = DateTime.Now;
                string? respostaCliente = null;
                while ((DateTime.Now - startTime).TotalSeconds < 15)
                {
                    if (jogador.Fluxo.DataAvailable)
                    {
                        respostaCliente = jogador.ReadLine();
                        break;
                    }
                }
                if (respostaCliente == null)
                {
                    //Console.WriteLine("### Jogador demorou a informar a revanche, encerrando conexao");
                    FinalizarPartida();
                    return;
                }
                jogador.JogadorEstaOcupado = false;
                if (respostaCliente.ToLower().Contains('s'))
                {
                    if (jogador.EstaJogandoComBola)
                        JogadorBolaQuerContinuar = true;
                    if (!jogador.EstaJogandoComBola)
                        JogadorXQuerContinuar = true;
                }
            }
            catch (IOException)
            {
                //Console.WriteLine(ex.ToString());
                EnunciarDesistencia();
            }
        }
        public void PartidaEmAndamento()
        {
            ResetarMatriz();
            EnviarMatrizParaJogadores();
            InverterVez();
            while (estadoPartida == EstadoDaPartida.ContinuarPartida)
            {
                AvisarJogadorVez();
                ReceberJogada();
                EnviarMatrizParaJogadores();
                VerificarSeJogadorGanhou();
            }
        }
        public void VerificarSeJogadorGanhou()
        {
            if (VerificarVencedorLinhaHorizontalOuVertical(1))
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
            if(VerificarEmpate())
            {
                estadoPartida = EstadoDaPartida.Empate;
                return;
            }
        }

        public bool VerificarEmpate()
        {
            for(int i=0;i<3;i++)
            {
                for (int j = 0; j < 3; j++)
                    if (matrizPartida[i][j] == 0)
                        return false;
            }
            Console.WriteLine("### EMPATE ###");
            return true;
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
            for (int ij = 0; ij < 3; ij++)
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
            for (int i = 0, j = 2; i < 3 && j >= 0; i++, j--)
            {
                if (matrizPartida[i][j] == bolaOuX)
                    contador++;
                else
                    break;
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
            for (int i = 0; i < 3; i++)
            {
                contador = 0;
                for (int j = 0; j < 3; j++)
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
                for (int j = 0; j < 3; j++)
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
            Thread.Sleep(50);
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
                //PrintarMatriz();
                //Console.WriteLine("Matriz Enviada: " + str);
                GetJogadorAtual.WriteLine(str);
                InverterVez();
                GetJogadorAtual.WriteLine(str);
            }
            catch (IOException)
            {
                //Console.WriteLine(ex.ToString());
                //Console.WriteLine("EXCEPTION");
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
                    Console.WriteLine("Esperando jogador [" + GetJogadorAtual.Nome + "] jogar...");
                    GetJogadorAtual.DescartarBuffer();
                    string str = GetJogadorAtual.ReadLine();
                    if (JogadaFoiBemSucedida(str))
                    {
                        //Console.WriteLine("Jogada foi [bem] sucedida!");
                        validarJogada = ValidarJogada.JogadaValida;
                    }
                    else
                    {
                        Thread.Sleep(50);
                        //Console.WriteLine("Jogada [não] foi bem sucedida!");
                        GetJogadorAtual.WriteLine("vez");
                    }
                }
                catch (IOException)
                {
                    //Console.WriteLine(ex.ToString());
                    //Console.WriteLine("EXCEPTION");
                    EnunciarDesistencia();
                }
            }
        }
        public void AvisarJogadorVez()
        {
            Thread.Sleep(50);
            try
            {
                string str = vez == Vez.j1 ? j1.Nome : j2.Nome;
                Console.WriteLine("Vez do jogador: " + vez.ToString() + " | " + str);
                GetJogadorAtual.WriteLine("vez");
                validarJogada = ValidarJogada.JogadaInvalida;
            }
            catch (IOException)
            {
                //Console.WriteLine(ex.ToString());
                //Console.WriteLine("EXCEPTION: AvisarJogadorVez");
                EnunciarDesistencia();
            }
        }


        public bool JogadaFoiBemSucedida(string str)
        {
            if (str.Length != 2)
            {
                //Console.WriteLine("Erro na JogadaFoiBemSucedida: Tamanho da jogada != 2: " + str);
                return false;
            }
            if (!str.All(char.IsDigit))
            {
                //Console.WriteLine("Erro na JogadaFoiBemSucedida: Nao é digito");
                return false;
            }
            int i = -1, j = -1;
            _ = int.TryParse(str[0].ToString(), out i);
            _ = int.TryParse(str[1].ToString(), out j);
            if (i != -1 && j != -1)
            {
                if (i >= 0 && i <= 2 && j >= 0 && j <= 2)
                {
                    int BolaOuX = GetJogadorAtual.EstaJogandoComBola ? 1 : 2;
                    if (matrizPartida[i][j] == 0)
                        matrizPartida[i][j] = BolaOuX;
                    else
                    {
                        //Console.WriteLine("JogadaFoiBemSucedida: Retornou False 1");
                        return false;
                    }
                    return true;
                }
            }
            //Console.WriteLine("JogadaFoiBemSucedida: Retornou False 2");
            return false;
        }

        public void EnunciarDesistencia()
        {
            vez = (vez == Vez.j1 ? Vez.j2 : Vez.j1);
            try
            {
                GetJogadorAtual.WriteLine("desistencia");
            }
            catch (IOException)
            {
                //Console.WriteLine(exx.ToString());
                //Console.WriteLine("Outro Error");

            }
            estadoPartida = EstadoDaPartida.Desistencia;
        }
        public void FinalizarPartida()
        {
            //Console.WriteLine("CONEXAO FECHADA");
            try
            {
                j1.Conexao.Close();
            }
            catch (IOException)
            {
                //Console.WriteLine(exx.ToString());
            }
            try
            {
                j2.Conexao.Close();
            }
            catch (IOException)
            {
                //Console.WriteLine(exx.ToString());
            }
        }
    }
    public enum EstadoDaPartida
    {
        ContinuarPartida,
        BolaGanhou,
        XGanhou,
        Desistencia,
        Empate
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