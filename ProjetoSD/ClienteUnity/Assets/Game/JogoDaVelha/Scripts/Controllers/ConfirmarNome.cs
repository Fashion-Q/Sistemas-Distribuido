using TMPro;
using UnityEngine;

public class ConfirmarNome : MonoBehaviour
{
    [SerializeField] private GameObject menuObject;
    [SerializeField] private TextMeshProUGUI nomeJogador;

    [SerializeField] private TMP_InputField userName;
    [SerializeField] private TMP_InputField ipAddress;
    [SerializeField] private TMP_InputField port;

    private void Awake()
    {
        userName.text = UserRepository.GetName();
        ipAddress.text = UserRepository.GetIPAdress();
        port.text = UserRepository.GetPort().ToString();
    }

    public void ValidarNome()
    {
        if (userName.text.Length < 3)
            return;
        nomeJogador.text = userName.text;
        menuObject.SetActive(false);

        UserRepository.SaveName(userName.text);
        UserRepository.SaveIPAdress(ipAddress.text);
        if(int.TryParse(port.text,out int p))
        {
            UserRepository.SavePort(p);
        }
    }
}
