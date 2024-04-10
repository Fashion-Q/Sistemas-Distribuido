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
        public Jogador(TcpClient Conexao, bool EstaJogandoComBola, string Nome) : this(Conexao, EstaJogandoComBola)
        {
            this.Nome = Nome;
        }
        public TcpClient Conexao { get; set; }
        public readonly NetworkStream Fluxo;
        private StreamReader Reader { get; set; }
        private StreamWriter Writer { get; set; }
        public string Nome { get; set; } = null!;
        public bool EstaJogandoComBola { get; set; } = false;
        public bool JogadorEstaOcupado { get; set; } = true;

        public void WriteLine(string str)
        {
            Writer.WriteLine(str);
            Writer.Flush();
        }
        public string ReadLine()
        {
            return Reader.ReadLine() ?? "";
        }

        public void DescartarBuffer()
        {
            byte[] buffer = new byte[1024];
            while (Fluxo.DataAvailable)
            {
                Fluxo.Read(buffer, 0, buffer.Length);
            }
        }
    }
}
