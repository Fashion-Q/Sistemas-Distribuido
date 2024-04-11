using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instanceGameController;
    private GameObject[][] bola;
    private GameObject[][] x;
    [SerializeField] private GameObject[] auxBola;
    [SerializeField] private GameObject[] auxX;
    [SerializeField] private GameObject desejaContinuar;
    [SerializeField] private GameObject esperandoOutroJogadorRevanche;
    [SerializeField] private GameObject anunciarProblemaObject;
    [SerializeField] private TextMeshProUGUI anunciarProblemaText;
    [SerializeField] private TextMeshProUGUI ponto1;
    [SerializeField] private TextMeshProUGUI ponto2;
    [SerializeField] private TextMeshProUGUI anunciarrVitoriaOuDerrotaText;
    [SerializeField] private GameObject acharPartida;
    [SerializeField] private GameObject desistirDaPartida;
    public bool EsperandoOutroJogadorAceitarRevanche { get; set; } = false;
    public void AnunciarProbleminha(string str)
    {
        esperandoOutroJogadorRevanche.SetActive(false);
        desejaContinuar.SetActive(false);
        anunciarProblemaText.text = str;
        anunciarProblemaObject.SetActive(true);
    }
    public void AtivarProbleminha(bool ativarProbleminha)
    {
        anunciarProblemaObject.SetActive(ativarProbleminha);
    }
    public bool PodeJogar { get; set; } = false;

    private void Awake()
    {
        corTexto = InstanciaCorDeTexto.Nenhum;
        instanceGameController = this;
        bola = new GameObject[3][];
        x = new GameObject[3][];
        for (int i = 0; i < 3; i++)
        {
            bola[i] = new GameObject[3];
            x[i] = new GameObject[3];
        }
        int contador = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                bola[i][j] = auxBola[contador];
                x[i][j] = auxX[contador];
                contador++;
            }
        }
    }
    private void Update()
    {
        AlternarCorDeTexto();
    }
    public void AlternarCorDeTexto()
    {
        if(TCPController.instanceTCP.PartidaEncontrada)
        {
            if (PodeJogar)
            {
                if(corTexto != InstanciaCorDeTexto.Jogando)
                {
                    corTexto = InstanciaCorDeTexto.Jogando;
                    TCPController.instanceTCP.J1.color = Color.green;
                    TCPController.instanceTCP.J2.color = Color.white;
                }
            }else if (!PodeJogar)
            {
                if (corTexto != InstanciaCorDeTexto.Esperando)
                {
                    corTexto = InstanciaCorDeTexto.Esperando;
                    TCPController.instanceTCP.J1.color = Color.white;
                    TCPController.instanceTCP.J2.color = Color.green;
                }
            }

        }
        else if(corTexto != InstanciaCorDeTexto.Nenhum)
        {
            corTexto = InstanciaCorDeTexto.Nenhum;
            TCPController.instanceTCP.J1.color = Color.white;
            TCPController.instanceTCP.J2.color = Color.white;
        }
    }
    public void Jogar(int x, int y)
    {
        Debug.Log("x: " + x + " | y: " + y);
        if (TCPController.instanceTCP.clienteSocket == null)
            return;
        if (!TCPController.instanceTCP.clienteSocket.Connected)
            return;
        if (TCPController.instanceTCP.Fluxo == null)
            return;
        if (PodeJogar)
        {
            if (!bola[x][y].activeSelf && !this.x[x][y].activeSelf)
            {
                string str = x.ToString() + y.ToString();
                Debug.Log("JOGANDO: " + x + " | " + y);
                PodeJogar = false;
                TCPController.instanceTCP.WriterLine(str);
            }
        }
    }

    public void ResetarJogoDaVelha()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                bola[i][j].SetActive(false);
                x[i][j].SetActive(false);
            }
        }
        ResetarPontuacao();
        AtualizarBotaoPrincipal(true);
    }

    public void ResetarMatch()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                bola[i][j].SetActive(false);
                x[i][j].SetActive(false);
            }
        }
        esperandoOutroJogadorRevanche.SetActive(false);
    }

    public void AnunciarVencedor(string str)
    {
        if (str.ToLower().Contains("empate"))
        {
            str = "### EMPATE ###";
        }
        else if (PvP.instancePvP.EstouJogandoComBola && str.ToLower().Contains("bola"))
        {
            str = "### Parabéns! Você ganhou ###";
            ponto1.text = (int.Parse(ponto1.text) + 1).ToString();
            Debug.Log("+1");
        }
        else if (!PvP.instancePvP.EstouJogandoComBola && str.ToLower().Contains("x"))
        {
            str = "### Parabéns! Você ganhou ###";
            ponto1.text = (int.Parse(ponto1.text) + 1).ToString();
            Debug.Log("+1");
        }
        else
        {
            str = "### Você perdeu! ###";
            Debug.Log("+1 inimigo");
            ponto2.text = (int.Parse(ponto1.text) + 1).ToString();
        }
        anunciarrVitoriaOuDerrotaText.text = str;
        desejaContinuar.SetActive(true);
    }

    public void DesejaContinuar(string str)
    {
        desejaContinuar.SetActive(false);
        if (str.ToLower().Contains("n"))
        {
            TCPController.instanceTCP.CancelarConexa();
        }
        else
        {
            if(TCPController.instanceTCP.clienteSocket.Connected)
            {
                TCPController.instanceTCP.WriterLine(str);
                esperandoOutroJogadorRevanche.SetActive(true);
                EsperandoOutroJogadorAceitarRevanche = false;
            }
            else
            {
                TCPController.instanceTCP.CancelarConexa();
            }
        }
    }

    public void AtualizarJogo(string str)
    {
        int contadorStr = 0, aux;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                aux = int.Parse(str[contadorStr].ToString());
                if (aux == 0)
                {
                    bola[i][j].SetActive(false);
                    x[i][j].SetActive(false);

                }
                else if (aux == 1)
                {
                    bola[i][j].SetActive(true);
                }
                else if (aux == 2)
                {
                    x[i][j].SetActive(true);
                }
                contadorStr++;
            }
        }
    }

    public void AtualizarBotaoPrincipal(bool ativarAcharPartida)
    {
        if(TCPController.instanceTCP.PartidaEncontrada || !ativarAcharPartida)
        {
            acharPartida.SetActive(false);
            desistirDaPartida.SetActive(true);
        }
        else
        {
            acharPartida.SetActive(true);
            desistirDaPartida.SetActive(false);
        }
    }

    public void AnuncinarProblema(string str)
    {
        Debug.Log("ANUNCIAR PROBLEMA");
        anunciarProblemaObject.SetActive(true);
        anunciarProblemaText.text = str;
    }
    public void ResetarPontuacao()
    {
        ponto1.text = "0";
        ponto2.text = "0";
    }

    private InstanciaCorDeTexto corTexto;
    public enum InstanciaCorDeTexto
    {
        Nenhum,
        Jogando,
        Esperando
    }
}
