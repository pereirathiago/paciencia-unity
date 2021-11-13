using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UserInput : MonoBehaviour
{
    public GameObject slot1;
    private Solitaire solitaire;
    private float timer;
    private float doubleClickTime = 0.3f;
    private int clickCount = 0;


    // Start is called before the first frame update
    void Start()
    {
        solitaire = FindObjectOfType<Solitaire>();
        slot1 = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (clickCount == 1)
        {
            timer += Time.deltaTime;
        }
        if (clickCount == 3)
        {
            timer = 0;
            clickCount = 1;
        }
        if (timer > doubleClickTime)
        {
            timer = 0;
            clickCount = 0;
        }

        GetMouseClick();
    }

    void GetMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickCount++;

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                if (hit.collider.CompareTag("Deck"))
                {
                    //click deck
                    Deck();
                }
                else if (hit.collider.CompareTag("Card"))
                {
                    // click card
                    Card(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Top"))
                {
                    // click top
                    Top(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Bottom"))
                {
                    // click bottom
                    Bottom(hit.collider.gameObject);
                }
            }
        }
    }

    void Deck()
    {
        // ações de click no baralho
        print("Clicked on deck");
        solitaire.DealFromDeck();
        slot1 = this.gameObject;

    }
    void Card(GameObject selected)
    {
        // ações de click no cards
        print("Clicked on Card");

        if (!selected.GetComponent<Selectable>().faceUp) // se a card clicada tiver face para baixo
        {
            if (!Blocked(selected)) // if nn estiver bloqueada
            {
                // vire ela
                selected.GetComponent<Selectable>().faceUp = true;
                slot1 = this.gameObject;
            }

        }
        else if (selected.GetComponent<Selectable>().inDeckPile) // se a card clicada tiver na pilha 
        {
            // e nn estiver bloqueada
            if (!Blocked(selected))
            {
                if (slot1 == selected) // se a msm carta for clicada duas veze
                {
                    if (DoubleClick())
                    {
                        // auto pilha
                        AutoStack(selected);
                    }
                }
                else
                {
                    slot1 = selected;
                }                
            }

        }
        else
        {

            // se estiver para cima a face
            // se nn tiver outra selecionada
            // selecione a carta

            if (slot1 == this.gameObject) // não é nulo porque passamos neste gameObject ao invés
            {
                slot1 = selected;
            }

            // se ja tem outra selecionadp
            else if (slot1 != selected)
            {
                // se podem empilhar
                if (Stackable(selected))
                {
                    Stack(selected);
                }
                else
                {
                    // selecione a carta nova
                    slot1 = selected;
                }
            }

            else if (slot1 == selected) // se clicada duas vezes
            {
                if (DoubleClick())
                {
                    // auto pilha
                    AutoStack(selected);
                }
            }


        }
    }
    void Top(GameObject selected)
    {
        // top click actions
        print("Clicked on Top");
        if (slot1.CompareTag("Card"))
        {
            // se a carta for um ás e o slot vazio estiver no topo, empilhe
            if (slot1.GetComponent<Selectable>().value == 1)
            {
                Stack(selected);
            }

        }


    }
    void Bottom(GameObject selected)
    {
        // bottom click actions
        print("Clicked on Bottom");
        // se a carta for um rei e o slot vazio estiver no topo, empilhe

        if (slot1.CompareTag("Card"))
        {
            if (slot1.GetComponent<Selectable>().value == 13)
            {
                Stack(selected);
            }
        }



    }

    bool Stackable(GameObject selected)
    {
        Selectable s1 = slot1.GetComponent<Selectable>();
        Selectable s2 = selected.GetComponent<Selectable>();
        // compare e veja se pode empilhar

        if (!s2.inDeckPile)
        {
            if (s2.top) // se tiver no topo emilhar de as ate rei
            {
                if (s1.suit == s2.suit || (s1.value == 1 && s2.suit == null))
                {
                    if (s1.value == s2.value + 1)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else  // se tiver em bottom empilhar de rei ate as
            {
                if (s1.value == s2.value - 1)
                {
                    bool card1Red = true;
                    bool card2Red = true;

                    if (s1.suit == "C" || s1.suit == "S")
                    {
                        card1Red = false;
                    }

                    if (s2.suit == "C" || s2.suit == "S")
                    {
                        card2Red = false;
                    }

                    if (card1Red == card2Red)
                    {
                        print("Not stackable");
                        return false;
                    }
                    else
                    {
                        print("Stackable");
                        return true;
                    }
                }
            }
        }
        return false;
    }

    void Stack(GameObject selected)
    {
        // se estiver no topo do rei ou no fundo vazio, empilhe as cartas no lugar
        // caso contrário, empilhe as cartas com um deslocamento y negativo

        Selectable s1 = slot1.GetComponent<Selectable>();
        Selectable s2 = selected.GetComponent<Selectable>();
        float yOffset = 0.3f;

        if (s2.top || (!s2.top && s1.value == 13))
        {
            yOffset = 0;
        }

        slot1.transform.position = new Vector3(selected.transform.position.x, selected.transform.position.y - yOffset, selected.transform.position.z - 0.01f);
        slot1.transform.parent = selected.transform; // faz com q os filhos mudem de lugar junto

        if (s1.inDeckPile) // remove da pilha superior
        {
            solitaire.tripsOnDisplay.Remove(slot1.name);
        }
        else if (s1.top && s2.top && s1.value == 1) // permite a troca de lugar das cartas
        {
            solitaire.topPos[s1.row].GetComponent<Selectable>().value = 0;
            solitaire.topPos[s1.row].GetComponent<Selectable>().suit = null;
        }
        else if (s1.top) // mantém o controle do valor atual dos decks superiores à medida que uma carta foi removida
        {
            solitaire.topPos[s1.row].GetComponent<Selectable>().value = s1.value - 1;
        }
        else // remove a sequencia da carta de bottom
        {
            solitaire.bottoms[s1.row].Remove(slot1.name);
        }

        s1.inDeckPile = false; //nn pode adicionar cartas ao deck
        s1.row = s2.row;

        if (s2.top) // move a carta ao topo e atribui o valor dela ao topo
        {
            solitaire.topPos[s1.row].GetComponent<Selectable>().value = s1.value;
            solitaire.topPos[s1.row].GetComponent<Selectable>().suit = s1.suit;
            s1.top = true;
        }
        else
        {
            s1.top = false;
        }

        // depois de concluir a movimentação, redefina o slot 1 para ser essencialmente nulo, pois será nulo quebrará a lógica
        slot1 = this.gameObject;

    }

    bool Blocked(GameObject selected)
    {
        Selectable s2 = selected.GetComponent<Selectable>();
        if (s2.inDeckPile == true)
        {
            if (s2.name == solitaire.tripsOnDisplay.Last()) // se a ultima da pilha na tela nn ta bloqueada
            {
                return false;
            }
            else
            {
                print(s2.name + " esta bloqueada por " + solitaire.tripsOnDisplay.Last());
                return true;
            }
        }
        else
        {
            if (s2.name == solitaire.bottoms[s2.row].Last()) // checa se é uma carta do bottom
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    bool DoubleClick()
    {
        if (timer < doubleClickTime && clickCount == 2)
        {
            print("Double Click");
            return true;
        }
        else
        {
            return false;
        }
    }

    void AutoStack(GameObject selected)
    {
        for (int i = 0; i < solitaire.topPos.Length; i++)
        {
            Selectable stack = solitaire.topPos[i].GetComponent<Selectable>();
            if (selected.GetComponent<Selectable>().value == 1) //se é um as
            {
                if (solitaire.topPos[i].GetComponent<Selectable>().value == 0) // e o topo esta vazio
                {
                    slot1 = selected;
                    Stack(stack.gameObject); // coloca as no topo
                    break;                  // na primeira posição vazia encontrada
                }
            }
            else
            {
                if ((solitaire.topPos[i].GetComponent<Selectable>().suit == slot1.GetComponent<Selectable>().suit) && (solitaire.topPos[i].GetComponent<Selectable>().value == slot1.GetComponent<Selectable>().value - 1))
                {
                    // se for o ultimo e nn tiver filhos
                    if (HasNoChildren(slot1))
                    {
                        slot1 = selected;
                        // encontre um ponto superior que corresponda às condições para empilhamento automático
                        string lastCardname = stack.suit + stack.value.ToString();
                        if (stack.value == 1)
                        {
                            lastCardname = stack.suit + "A";
                        }
                        if (stack.value == 11)
                        {
                            lastCardname = stack.suit + "J";
                        }
                        if (stack.value == 12)
                        {
                            lastCardname = stack.suit + "Q";
                        }
                        if (stack.value == 13)
                        {
                            lastCardname = stack.suit + "K";
                        }
                        GameObject lastCard = GameObject.Find(lastCardname);
                        Stack(lastCard);
                        break;
                    }
                }
            }



        }
    }

    bool HasNoChildren(GameObject card)
    {
        int i = 0;
        foreach (Transform child in card.transform)
        {
            i++;
        }
        if (i == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
