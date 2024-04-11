using System.Linq;
using System;
using UnityEngine;
using System.Collections;

public class PvP : MonoBehaviour
{
    public static PvP instancePvP;
    delegate void AcaoDeLeitura(string str);
    AcaoDeLeitura acaoLeitura;
    public static string LerDoServidor { get; set; } = "";
    public bool EstouJogandoComBola { get; set; } = false;

    private void Awake()
    {
        instancePvP = this;
        //acaoLeitura = Run;
    }

    private void Update()
    {
        if(LerDoServidor.Length > 0)
        {
            Run();
        }
    }

    public void ThreadManager()
    {
        Debug.Log("Thread Manager iniciou");
        while(TCPController.instanceTCP.PartidaEncontrada)
        {
            if (!TCPController.instanceTCP.PartidaEncontrada)
            {
                return;
            }

            if (TCPController.instanceTCP.clienteSocket == null || !TCPController.instanceTCP.clienteSocket.Connected)
            {
                LerDoServidor = "desistencia";
                //GameController.instanceGameController.AnuncinarProblema("[" + TCPController.instanceTCP.J2.text + "] cancelou a conexão");
                return;
            }
            try
            {
                string str = TCPController.instanceTCP.R.ReadLine();
                LerDoServidor = str ?? "desistencia";
            }
            catch (Exception)
            {
                LerDoServidor = "desistencia";
                return;
            }
        }
        Debug.Log("Thread Manager Finalizou");
    }

    public void Run()
    {
        if (GameController.instanceGameController.EsperandoOutroJogadorAceitarRevanche)
            return;
        Debug.Log("### PvP ### [" + LerDoServidor + "]");
        if (LerDoServidor.ToLower().Contains("exit") || LerDoServidor.ToLower().Contains("desistencia"))
        {
            Debug.Log("EXIT");
            GameController.instanceGameController.AnunciarProbleminha("Oponente desistiu");
            TCPController.instanceTCP.CancelarConexa();
            return;
        }
        else if (LerDoServidor.All(char.IsDigit) && LerDoServidor.Length == 9)
        {
            Debug.Log("AtualizarJogo: " + LerDoServidor);
            GameController.instanceGameController.AtualizarJogo(LerDoServidor);
        }
        else if (LerDoServidor.ToLower().Contains("vez"))
        {
            Debug.Log("[vez]");
            GameController.instanceGameController.PodeJogar = true;
        }
        else if (LerDoServidor.ToLower().Contains("vencedor"))
        {
            Debug.Log("Vencedor");
            GameController.instanceGameController.AnunciarVencedor(LerDoServidor);
        }
        else if (LerDoServidor.ToLower().Contains("match"))
        {
            Debug.Log("MATCH");
            GameController.instanceGameController.ResetarMatch();
        }
        LerDoServidor = "";
    }
}
