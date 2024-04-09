using IdJogador;

namespace GerenciadorMatchCliente
{
    public class MatchCliente
    {
        public MatchCliente(Jogador jogador) 
        {
            this.jogador = jogador;
        }
        private readonly Jogador jogador;
        private EstadoDaPartida estadoPartida;
        public string NomeJogador2 { get; set; } = "";

        public void Run()
        {
            estadoPartida = EstadoDaPartida.Esperando;
            while(estadoPartida != EstadoDaPartida.Desistencia)
            {
                EsperandoMinhaVez();
            }
            Exit();
        }

        public void EsperandoMinhaVez()
        {

        }

        public void Exit()
        {
            try
            {
                jogador.Conexao.Close();
            }catch (Exception ex)
            {
                Console.WriteLine("### EXIT ###");
                Console.WriteLine(ex.ToString());
            }
        }
    }
    public enum EstadoDaPartida
    {
        Desistencia,
        Esperando
    }
}
