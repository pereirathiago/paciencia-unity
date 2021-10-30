using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateSprite : MonoBehaviour
{
    public Sprite cardFace;
    public Sprite cardBack;
    private SpriteRenderer spriteRenderer;
    private Selecionado selecionado;
    private Paciencia paciencia;
    private UserInput userInput;



    // Start is called before the first frame update
    void Start()
    {
        List<string> baralho = Paciencia.GerarBaralho();
        paciencia = FindObjectOfType<Paciencia>();
        userInput = FindObjectOfType<UserInput>();

        int i = 0;
        foreach (string carta in baralho)
        {
            if (this.name == carta)
            {
            print("name: " + this.name + " carta: " + carta);
                cardFace = paciencia.faceCartas[i];
                print("cardface: " + cardFace);
                break;
            }
            i++;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        selecionado = GetComponent<Selecionado>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selecionado.faceCima == true)
        {
            spriteRenderer.sprite = cardFace;
        }
        else
        {
            spriteRenderer.sprite = cardBack;
        }

        if(userInput.slot1)
        {
            if(name == userInput.slot1.name)
            {
                spriteRenderer.color = Color.yellow;
            } else
            {
                spriteRenderer.color = Color.white;
            }
        }

    }
}
