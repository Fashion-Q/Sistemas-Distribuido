using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using TMPro;
using UnityEngine;

public class TCPController : MonoBehaviour
{
    public static TCPController instanceTCP;
    [SerializeField] private TMP_InputField inputIP;
    [SerializeField] private TMP_InputField inputPorta;
    delegate void AcaoDeLeitura(string str);
    AcaoDeLeitura acaoLeitura;
    public TcpClient clienteSocket;
    public NetworkStream Fluxo { get; set; }
    public StreamReader R { get; set; }
    public StreamWriter W { get; set; }
    [SerializeField] private GameObject tcpCanvasConexao;
    [SerializeField] private TextMeshProUGUI nomeJogador1;
    [SerializeField] private TextMeshProUGUI nomeJogador2;
    private bool EstaConectando { get; set; } = false;
    public TextMeshProUGUI J1 => nomeJogador1;
    public TextMeshProUGUI J2 => nomeJogador2;
    public bool PartidaEncontrada { get; set; } = false;

    private void Awake()
    {
        instanceTCP = this;
        clienteSocket?.Close();
        clienteSocket = null;
    }

    public void TentarConexao()
    {
        PartidaEncontrada = false;
        GameController.instanceGameController.ResetarJogoDaVelha();
        if (!EstaConectando)
        {
            EstaConectando = true;
            StartCoroutine(TentarConexaoCounrontina());
        }

    }

    IEnumerator TentarConexaoCounrontina()
    {
        GameController.instanceGameController.AnunciarProbleminha("Tentando conectar");
        GameController.instanceGameController.EsperandoOutroJogadorAceitarRevanche = false;
        yield return new WaitForSeconds(0.15f);
        try
        {
            int porta = int.Parse(inputPorta.text);
            clienteSocket = new TcpClient(inputIP.text, porta);
            GameController.instanceGameController.AtivarProbleminha(false);
        }
        catch
        {
            GameController.instanceGameController.AnunciarProbleminha("Falha de conexão com o servidor");
        }
        EstaConectando = false;
        GameController.instanceGameController.AtualizarBotaoPrincipal(false);
        if (clienteSocket != null && clienteSocket.Connected)
        {
            tcpCanvasConexao.SetActive(true);
            Fluxo = clienteSocket.GetStream();
            R = new StreamReader(Fluxo);
            W = new StreamWriter(Fluxo);
            acaoLeitura = EsperandoServidorPedirNome;
        }
    }

    public void EsperandoMatch(string str)
    {
        if (str == "error")
        {
            GameController.instanceGameController.AnunciarProbleminha("Erro de conexão, tente novamente");
            CancelarConexa();
            return;
        }
        if (str.ToLower().Contains("setar partida"))
        {
            string[] particao = str.Split('|');
            particao[0] = particao[0].Trim();
            particao[2] = particao[2].Trim();
            particao[3] = particao[3].Trim();
            PvP.instancePvP.EstouJogandoComBola = particao[3] == "1" ? true : false;
            if (PvP.instancePvP.EstouJogandoComBola)
                nomeJogador2.text = particao[2].Trim();
            else
                nomeJogador2.text = particao[1].Trim();
        }
        if (str.ToLower().Contains("partida encontrada"))
        {
            PartidaEncontrada = true;
            //StartCoroutine(PvP.instancePvP.ThreadManager());
            new Thread(PvP.instancePvP.ThreadManager).Start();
            acaoLeitura = null;
            tcpCanvasConexao.SetActive(false);
        }
    }

    public void EsperandoServidorPedirNome(string str)
    {
        if (str != null && str.ToLower().Contains("digite seu nome"))
        {
            WriterLine(nomeJogador1.text);
            acaoLeitura = EsperandoMatch;
        }
        else
        {
            CancelarConexa();
            Debug.Log("SERVIDOR NAO PEDIU NOME, CANCELANDO CONEXAO");
        }
    }

    private void FixedUpdate()
    {
        if (Fluxo != null)
        {
            if (clienteSocket == null || !clienteSocket.Connected)
            {
                GameController.instanceGameController.AnuncinarProblema("Problema de conexão, tente novamente");
                CancelarConexa();
                return;
            }
            if (Fluxo.DataAvailable)
            {
                string str = R.ReadLine();
                acaoLeitura?.Invoke(str);
            }
        }
    }

    public void WriterLine(string str)
    {
        W.WriteLine(str);
        W.Flush();
    }

    public void CancelarConexa()
    {
        if (clienteSocket != null)
            clienteSocket.Close();
        clienteSocket = null;
        Fluxo = null;
        R = null;
        W = null;
        tcpCanvasConexao.SetActive(false);
        PartidaEncontrada = false;
        nomeJogador2.text = "Jogador 2";
        GameController.instanceGameController.EsperandoOutroJogadorAceitarRevanche = false;
        PvP.LerDoServidor = "";
        GameController.instanceGameController.ResetarJogoDaVelha();
    }
    public void AposDestruir()
    {
        if (clienteSocket != null)
            clienteSocket.Close();
        clienteSocket = null;
        Fluxo = null;
        R = null;
        W = null;
    }
    private void OnDestroy()
    {
        Debug.Log("OnDestroyer");
        AposDestruir();
    }
}
