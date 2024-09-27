using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using TMPro;
using UnityEngine;

public class TCPController : MonoBehaviour
{
    public static TCPController instanceTCP;
    public TcpClient clienteSocket;
    public NetworkStream Fluxo { get; set; }
    public StreamReader R { get; set; }
    public StreamWriter W { get; set; }
    [SerializeField] private GameObject tcpCanvasConexao;
    [SerializeField] private TextMeshProUGUI nomeJogador1;
    [SerializeField] private TextMeshProUGUI nomeJogador2;
    [SerializeField] private TextMeshProUGUI testando;
    public GameObject GetTcpCanvasConexao => tcpCanvasConexao;
    public string GetJogador1 => nomeJogador1.text;
    public void SetNomeJogador2(string str) => nomeJogador2.text = str;
    private bool EstaConectando { get; set; } = false;
    public TextMeshProUGUI J1 => nomeJogador1;
    public TextMeshProUGUI J2 => nomeJogador2;
    public bool PartidaEncontrada { get; set; } = false;
    public static bool TCPBeforePvPConnectionError { get; set; } = false;
    public static bool SetPlayerNameBool { get; set; } = false;
    public static string SetPlayerNameStr { get; set; } = "";
    public static bool SetMatchFound { get; set; } = false;
    public StatusDoJogadorEnum StatusDoJogador { get; set; } = StatusDoJogadorEnum.Problema_de_conexão_tente_novamente;
    public enum StatusDoJogadorEnum
    {
        Problema_de_conexão_tente_novamente,
        Oponente_desistiu
    }
    


    private void Awake()
    {
        instanceTCP = this;
        clienteSocket?.Close();
        clienteSocket = null;
    }

    private void Update()
    {
        if(TCPBeforePvPConnectionError)
        {
            tcpCanvasConexao.SetActive(false);
            Debug.Log("### TCPBeforePvPConnectionError ###");
            TCPBeforePvPConnectionError = false;
            GameController.instanceGameController.AnunciarMensagem(StatusDoJogador.ToString().Replace('_',' '));
            CancelarConexa();
        }

        if(SetMatchFound)
        {
            SetMatchFound = false;
            PartidaEncontrada = true;
            GetTcpCanvasConexao.SetActive(false);
            new Thread(PvP.instancePvP.PvPThread).Start();
        }
        if(SetPlayerNameBool)
        {
            SetPlayerNameBool = false;
            nomeJogador2.text = SetPlayerNameStr;
        }
    }

    public void TryConnection()
    {
        PartidaEncontrada = false;
        GameController.instanceGameController.ResetarJogoDaVelha();
        if (!EstaConectando)
        {
            testando.text = "1";
            EstaConectando = true;
            RequestingConnectionToTheServer();
        }

    }

    public void RequestingConnectionToTheServer()
    {
        bool IsCatch = false;
        GameController.instanceGameController.AnunciarMensagem("Tentando conectar");
        GameController.instanceGameController.AnnounceWinner = false;
        testando.text += " 2";
        try
        {
            testando.text += " 3";
            testando.text += " 4";

            //clienteSocket = new TcpClient(UserRepository.GetIPAdress(), UserRepository.GetPort());
            clienteSocket = new TcpClient(UserRepository.GetIPAdress(), UserRepository.GetPort());
            testando.text += " 5";//aa
            GameController.instanceGameController.AtivarProbleminha(false);
            testando.text += " 6";
        }
        catch (Exception ex)
        {
            IsCatch = true;
            testando.text += " 7 | " + ex.ToString();
            Debug.Log(ex.ToString());
            GameController.instanceGameController.AnunciarMensagem("Falha de conexão com o servidor");
            EstaConectando = false;
        }
        EstaConectando = false;
        testando.text += " 8";
        if (IsCatch)
            return;

        if (clienteSocket != null && clienteSocket.Connected)
        {
            testando.text += " 9";

            GameController.instanceGameController.AtualizarBotaoPrincipal(false);
            tcpCanvasConexao.SetActive(true);
            Fluxo = clienteSocket.GetStream();
            R = new StreamReader(Fluxo);
            W = new StreamWriter(Fluxo);
            new Thread(TCPControllerBeforePvP.EsperandoServidorPedirNomeThread).Start();
            return;
        }
        testando.text += " 10";
    }

    public void WriterLine(string str)
    {
        W.WriteLine(str);
        W.Flush();
    }

    public void CancelarConexa()
    {
        if (clienteSocket != null)
        {
            Debug.Log("Fechando Conexao");
            clienteSocket.Close();
        }
        clienteSocket = null;
        Fluxo = null;
        R = null;
        W = null;
        tcpCanvasConexao.SetActive(false);
        PartidaEncontrada = false;
        nomeJogador2.text = "Jogador 2";
        GameController.instanceGameController.AnnounceWinner = false;
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
