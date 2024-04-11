using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConfirmarNome : MonoBehaviour
{
    [SerializeField] private TMP_InputField input;
    [SerializeField] private GameObject menuObject;
    [SerializeField] private TextMeshProUGUI nomeJogador;

    public void ValidarNome()
    {
        if (input.text.Length < 3)
            return;
        nomeJogador.text = input.text;
        menuObject.SetActive(false);
    }
}
