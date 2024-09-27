using System;
using UnityEngine;
using System.Threading;

public static class TCPControllerBeforePvP
{
    public static void EsperandoServidorPedirNomeThread()
    {
        Debug.Log("Thread Esperando Outro Jogador [On]");
        try
        {
            string str = TCPController.instanceTCP.R.ReadLine() ?? "desistencia";
            Debug.Log("### Servidor ### | " + str);
            if (str == "desistencia" || str == "error")
            {
                TCPController.instanceTCP.StatusDoJogador = TCPController.StatusDoJogadorEnum.Problema_de_conexão_tente_novamente;
                TCPController.TCPBeforePvPConnectionError = true;
                Debug.Log("Esperando... [Off] " + str);
                return;
            }
            if (str.ToLower().Contains("digite seu nome"))
            {
                TCPController.instanceTCP.WriterLine(TCPController.instanceTCP.GetJogador1);
                Debug.Log("### Nome Enviado ###");
                Debug.Log("EsperandoOutroJogadorThread [Off]");
                new Thread(() => EsperandoMatchThread(1)).Start();
                return;
            }
            TCPController.instanceTCP.StatusDoJogador = TCPController.StatusDoJogadorEnum.Problema_de_conexão_tente_novamente;
            TCPController.TCPBeforePvPConnectionError = true;
            Debug.Log("EsperandoOutroJogadorThread [Off] | SERVIDOR NAO PEDIU NOME");
            return;
        }
        catch (Exception)
        {
            Debug.Log("### 4 ###");
            TCPController.instanceTCP.StatusDoJogador = TCPController.StatusDoJogadorEnum.Problema_de_conexão_tente_novamente;
            TCPController.TCPBeforePvPConnectionError = true;
        }
        Debug.Log("EsperandoOutroJogadorThread [Off] | FINAL DA FUNCAO");
    }

    public static void EsperandoMatchThread(int cont)
    {
        if (cont >= 10)
        {
            Debug.Log("### RECURSIVO CANCELADO ### " + cont);
            return;
        }
        Debug.Log("Thread Match [" + cont + "]");
        try
        {
            string str = TCPController.instanceTCP.R.ReadLine() ?? "desistencia";
            Debug.Log("### MATCH ### [" + str + "]");
            if (str == "desistencia" || str == "error")
            {
                TCPController.instanceTCP.StatusDoJogador = TCPController.StatusDoJogadorEnum.Problema_de_conexão_tente_novamente;
                TCPController.TCPBeforePvPConnectionError = true;
                Debug.Log("Match [Off] desistencia || error: " + str);
                return;
            }
            if (str.ToLower().Contains("setar partida"))
            {
                string[] particao = str.Split('|');
                particao[2] = particao[2].Trim();
                particao[3] = particao[3].Trim();
                PvP.instancePvP.EstouJogandoComBola = particao[3] == "1" ? true : false;
                if (PvP.instancePvP.EstouJogandoComBola)
                {
                    TCPController.SetPlayerNameStr = particao[2].Trim();
                    TCPController.SetPlayerNameBool = true;
                }
                else
                {
                    TCPController.SetPlayerNameStr = particao[1].Trim();
                    TCPController.SetPlayerNameBool = true;
                }
                Debug.Log("### SETANDO PARTIDA ###");
                cont++;
                EsperandoMatchThread(cont);
                return;
            }
            if (str.ToLower().Contains("ignore"))
            {
                Console.WriteLine("### Servidor pediu para ignorar mensagem ###");
                EsperandoMatchThread(cont++);
                return;
            }
            if (str.ToLower().Contains("partida encontrada"))
            {
                TCPController.SetMatchFound = true;
                Debug.Log("### PARTIDA ENCONTRADA -> PvP Thread ### " + str);
                return;
            }
        }
        catch (Exception ex )
        {
            Debug.Log("### exception ### " + ex.ToString());
            TCPController.instanceTCP.StatusDoJogador = TCPController.StatusDoJogadorEnum.Problema_de_conexão_tente_novamente;
            TCPController.TCPBeforePvPConnectionError = true;
        }
        Debug.Log("Match [Off]");
    }
}
