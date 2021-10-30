using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UserInput : MonoBehaviour
{
    public GameObject slot1;

    private Paciencia paciencia;

    // Start is called before the first frame update
    void Start()
    {
        paciencia = FindObjectOfType<Paciencia>();
        slot1 = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        GetMouseClick();
    }

    void GetMouseClick()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit)
            {
                // pode clicar em baralho, carta, vazio
                if(hit.collider.CompareTag("Deck"))
                {
                    // clicou no baralho
                    Baralho();
                } else if (hit.collider.CompareTag("Card"))
                {
                    // clicou na carta
                    Carta(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Top"))
                {
                    // clicou top
                    Top();
                }
                else if (hit.collider.CompareTag("Bottom"))
                {
                    // clicou bottom
                    Bottom();
                }
            }
        }
    }

    void Baralho()
    {
        // click no baralho
        print("clicou no baralho");
        paciencia.DealFromDeck();
    }
    void Carta(GameObject selected)
    {
        // click no carta
        print("clicou no carta");

        if (slot1 == this.gameObject)
        {
            slot1 = selected;
        } 
        else if (slot1 != selected)
        {
            if(Stackable(selected))
            {
                // pilha isso
            } else
            {
                slot1 = selected;
            }
        }
    }
    void Top()
    {
        // click no top
        print("clicou no top");
    }
    void Bottom()
    {
        // click no bottom
        print("clicou no bottom");
    }

    bool Stackable(GameObject selected)
    {
        Selecionado s1 = slot1.GetComponent<Selecionado>();
        Selecionado s2 = selected.GetComponent<Selecionado>();
        if(s2.top)
        {
            if(s1.suit == s2.suit || (s1.value == 1 && s2.suit == null))
            {
                if(s1.value == s2.value + 1)
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        } else
        {
            if(s1.value == s2.value - 1)
            {
                bool card1Red = true;
                bool card2Red = true;

                if(s1.suit == "C" || s1.suit == "S")
                {
                    card1Red = false;
                }
                if (s2.suit == "C" || s2.suit == "S")
                {
                    card2Red = false;
                }

                if(card1Red == card2Red)
                {
                    print("nao pilhavel");
                    return false;
                } else
                {
                    print("pilhavel");
                    return true;
                }
            }
        }
        return false;
    }
}
