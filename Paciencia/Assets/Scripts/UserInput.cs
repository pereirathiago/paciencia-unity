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

        if (!selected.GetComponent<Selecionado>().faceCima)
        {
            if(!Blocked(selected))
            {
                selected.GetComponent<Selecionado>().faceCima = true;
                slot1 = this.gameObject;
            }
        }
        else if(selected.GetComponent<Selecionado>().inDeckPile)
        {
            if(!Blocked(selected))
            {
                slot1 = selected;
            }
            if (!Blocked(selected))
            {
                if (slot1 == selected)
                {
                   slot1 = selected;
                }
            }
        }

        if (slot1 == this.gameObject)
        {
            slot1 = selected;
        } 
        else if (slot1 != selected)
        {
            if(Stackable(selected))
            {
                Stack(selected);
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
        if(!s2.inDeckPile)
        {
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

                    if(s1.suit == "P" || s1.suit == "E")
                    {
                        card1Red = false;
                    }
                    if (s2.suit == "P" || s2.suit == "E")
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
        }
        return false;
    }

    void Stack(GameObject selected)
    {
        Selecionado s1 = slot1.GetComponent<Selecionado>();
        Selecionado s2 = selected.GetComponent<Selecionado>();
        float yOffset = 0.3f;

        if(s2.top || (!s2.top && s1.value == 13))
        {
            yOffset = 0;
        }

        slot1.transform.position = new Vector3(selected.transform.position.x, selected.transform.position.y - yOffset, selected.transform.position.z - 0.01f);
        slot1.transform.parent = selected.transform;
        if (s1.inDeckPile)
        {
            paciencia.tripsOnDisplay.Remove(slot1.name);
        } else if (s1.top && s2.top && s1.value == 1)
        {
            paciencia.topPos[s1.row].GetComponent<Selecionado>().value = 0;
            paciencia.topPos[s1.row].GetComponent<Selecionado>().suit = null;
        } else if (s1.top)
        {
            paciencia.topPos[s1.row].GetComponent<Selecionado>().value = s1.value - 1;
        } else
        {
            paciencia.bottoms[s1.row].Remove(slot1.name);
        }
        s1.inDeckPile = false;
        s1.row = s2.row;

        if(s2.top)
        {
            paciencia.topPos[s1.row].GetComponent<Selecionado>().value = s1.value;
            paciencia.topPos[s1.row].GetComponent<Selecionado>().suit = s1.suit;
            s1.top = true;
        } else
        {
            s1.top = false;
        }
        slot1 = this.gameObject;
    }

    bool Blocked(GameObject selected)
    {
        Selecionado s2 = selected.GetComponent<Selecionado>();
        if (s2.inDeckPile == true)
        {
            if (s2.name == paciencia.tripsOnDisplay.Last()) 
            {
                return false;
            }
            else
            {
                print(s2.name + " esta bloqueado por " + paciencia.tripsOnDisplay.Last());
                return true;
            }
        }
        else
        {
            if (s2.name == paciencia.bottoms[s2.row].Last()) 
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
