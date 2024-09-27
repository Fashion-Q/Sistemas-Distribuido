using System.Linq;
using System;
using UnityEngine;

public class PvP : MonoBehaviour
{
    public static PvP instancePvP;
    delegate void AcaoDeLeitura(string str);
    public bool EstouJogandoComBola { get; set; } = false;

    public static string attJogo = "";
    public static bool attJogoBool = false;
    private void Awake()
    {
        instancePvP = this;
        //acaoLeitura = Run;
    }

    private void Update()
    {
        if(attJogoBool)
        {
            attJogoBool = false;
            if (attJogo.Length == 9 && attJogo.All(char.IsDigit))
                GameController.instanceGameController.AtualizarJogo(attJogo);
        }
    }
    public void PvPThread()
    {
        Debug.Log("Thread PvP [On]");
        if (!TCPController.instanceTCP.PartidaEncontrada)
            return;
        string str = "";
        while (TCPController.instanceTCP.PartidaEncontrada)
        {
            try
            {
                str = TCPController.instanceTCP.R.ReadLine() ?? "exit";

                Debug.Log("### PvP ### [" + str + "]");
                if (str.ToLower().Contains("exit") || str.ToLower().Contains("desistencia"))
                {
                    Debug.Log("EXIT");
                    ExitPvP();
                    return;
                }
                else if (str.All(char.IsDigit) && str.Length == 9)
                {
                    Debug.Log("### AtualizarJogo ### [" + str + "]");
                    attJogo = str;
                    attJogoBool = true;
                }
                else if (str.ToLower().Contains("vez"))
                {
                    Debug.Log("[Minha vez]");
                    GameController.instanceGameController.CanPlay = true;
                }
                else if (str.ToLower().Contains("vencedor"))
                {
                    Debug.Log("Vencedor");
                    GameController.anunciarVencedor = str;
                    GameController.instanceGameController.AnnounceWinner = true;
                }
                else if (str.ToLower().Contains("match"))
                {
                    Debug.Log("MATCH");
                    GameController.instanceGameController.RestartMatchBool = true;
                }
            }
            catch (Exception)
            {
                ErroDeConexao();
                return;
            }
        }

        Debug.Log("Thread PvP [Off]");
    }

    public static void ErroDeConexao()
    {
        TCPController.TCPBeforePvPConnectionError = true;
        TCPController.instanceTCP.PartidaEncontrada = false;
        TCPController.instanceTCP.StatusDoJogador = TCPController.StatusDoJogadorEnum.Problema_de_conexão_tente_novamente;
    }

    public static void ExitPvP()
    {
        TCPController.TCPBeforePvPConnectionError = true;
        TCPController.instanceTCP.PartidaEncontrada = false;
        TCPController.instanceTCP.StatusDoJogador = TCPController.StatusDoJogadorEnum.Oponente_desistiu;
    }
}
