using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtons : MonoBehaviour
{
    public GameObject highScorePanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAgain()
    {
        highScorePanel.SetActive(false);
        ResetScene();
    }

    public void ResetScene()
    {
        UpdateSprite[] cards = FindObjectsOfType<UpdateSprite>();
        foreach(UpdateSprite card in cards)
        {
            Destroy(card.gameObject);
        }
        ClearTopValues();
        FindObjectOfType<Paciencia>().JogarCartas();
    }

    void ClearTopValues()
    {
        Selecionado[] selecionados = FindObjectsOfType<Selecionado>();
        foreach(Selecionado selecionado in selecionados)
        {
            if (selecionado.CompareTag("Top"))
            {
                selecionado.suit = null;
                selecionado.value = 0;
            }
        }
    }
}
