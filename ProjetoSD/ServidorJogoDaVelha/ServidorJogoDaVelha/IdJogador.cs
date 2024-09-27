using System.Net.Sockets;

namespace IdJogador
{
    public class Jogador
    {
        public Jogador(TcpClient Conexao, bool EstaJogandoComBola)
        {
            this.EstaJogandoComBola = EstaJogandoComBola;
            this.Conexao = Conexao;
            Fluxo = Conexao.GetStream();
            Reader = new StreamReader(Fluxo);
            Writer = new StreamWriter(Fluxo);
        }
        public Jogador(TcpClient Conexao, bool EstaJogandoComBola, string Nome)
        {
            this.EstaJogandoComBola = EstaJogandoComBola;
            this.Conexao = Conexao;
            Fluxo = Conexao.GetStream();
            Reader = new StreamReader(Fluxo);
            Writer = new StreamWriter(Fluxo);
            this.Nome = Nome;
        }
        public TcpClient Conexao { get; set; }
        public NetworkStream Fluxo;
        public StreamReader Reader { get; set; }
        public StreamWriter Writer { get; set; }
        public string Nome { get; set; } = null!;
        public bool EstaJogandoComBola { get; set; } = false;
        public bool JogadorEstaOcupado { get; set; } = true;

        public void DescartarBuffer()
        {
            byte[] buffer = new byte[1024];
            while (Fluxo.DataAvailable)
            {
                Fluxo.Read(buffer, 0, buffer.Length);
                Console.WriteLine("Descartando buffer: " + buffer);
            }
        }
    }
}
