using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Paciencia : MonoBehaviour
{
    public Sprite[] faceCartas;
    public GameObject cardPrefab;
    public GameObject deckButton;
    public GameObject[] bottomPos;
    public GameObject[] topPos;

    public static string[] naipes = new string[] { "P", "O", "C", "E" };
    public static string[] valores = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    public List<string>[] bottoms;
    public List<string>[] tops;
    public List<string> tripsOnDisplay = new List<string>();
    public List<List<string>> barlhoTrips = new List<List<string>>();

    private List<string> bottom0 = new List<string>();
    private List<string> bottom1 = new List<string>();
    private List<string> bottom2 = new List<string>();
    private List<string> bottom3 = new List<string>();
    private List<string> bottom4 = new List<string>();
    private List<string> bottom5 = new List<string>();
    private List<string> bottom6 = new List<string>();

    public List<string> baralho;
    public List<string> discardPile = new List<string>();
    private int deckLocation;
    private int trips;
    private int tripsRemainder;

    // Start is called before the first frame update
    void Start()
    {
        bottoms = new List<string>[] { bottom0, bottom1, bottom2, bottom3, bottom4, bottom5, bottom6 };
        JogarCartas();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void JogarCartas()
    {
        baralho = GerarBaralho();
        Embaralhar(baralho);
        foreach (string carta in baralho)
        {
            print(carta);
        }
        PacienciaSort();
        StartCoroutine(PacienciaDeal());
        SortDeckIntoTrips();
    }

    public static List<string> GerarBaralho()
    {
        List<string> novoBaralho = new List<string>();
        foreach (string n in naipes)
        {
            foreach (string v in valores)
            {
                novoBaralho.Add(n + v);
            }
        }
        return novoBaralho;
    }

    void Embaralhar<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    IEnumerator PacienciaDeal()
    {
        for (int i = 0; i < 7; i++)
        {
            float yOffset = 0;
            float zOffset = 0.03f;
            foreach (string carta in bottoms[i])
            {
                yield return new WaitForSeconds(0.05f);
                GameObject novaCarta = Instantiate(cardPrefab, new Vector3(bottomPos[i].transform.position.x, bottomPos[i].transform.position.y - yOffset, bottomPos[i].transform.position.z - zOffset), Quaternion.identity, bottomPos[i].transform);
                novaCarta.name = carta;
                novaCarta.GetComponent<Selecionado>().row = i;

                if (carta == bottoms[i][bottoms[i].Count - 1]) 
                {
                    novaCarta.GetComponent<Selecionado>().faceCima = true;
                }

                yOffset = yOffset + 0.5f;
                zOffset = zOffset + 0.03f;
                discardPile.Add(carta);
            }
        }
        foreach(string carta in discardPile)
        {
            if(baralho.Contains(carta))
            {
                baralho.Remove(carta);
            }
        }
        discardPile.Clear();
    }

    void PacienciaSort()
    {
        for(int i = 0; i < 7; i++)
        {
            for(int j = i; j < 7; j++)
            {
                bottoms[j].Add(baralho.Last<string>());
                baralho.RemoveAt(baralho.Count - 1);
            }
        }
    }

    public void SortDeckIntoTrips()
    {
        trips = baralho.Count / 3;
        tripsRemainder = baralho.Count % 3;
        barlhoTrips.Clear();

        int modifier = 0;
        for (int i = 0; i < trips; i++)
        {
            List<string> myTrips = new List<string>();
            for (int j = 0; j < 3; j++)
            {
                myTrips.Add(baralho[j + modifier]);
            }
            barlhoTrips.Add(myTrips);
            modifier += 3;
        }

        if ( tripsRemainder != 0)
        {
            List<string> myRemainders = new List<string>();
            modifier = 0;
            for (int k = 0; k <tripsRemainder; k++)
            {
                myRemainders.Add(baralho[baralho.Count - tripsRemainder + modifier]);
                modifier++;
            }
            barlhoTrips.Add(myRemainders);
            trips++;
        }
        deckLocation = 0;
    }

    public void DealFromDeck()
    {
        foreach (Transform child in deckButton.transform)
        {
            if(child.CompareTag("Card"))
            {
                baralho.Remove(child.name);
                discardPile.Add(child.name);
                Destroy(child.gameObject);
            }
        }

        if(deckLocation < trips)
        {
            // jogue 3 novacartas
            tripsOnDisplay.Clear();
            float xOffset = 2.5f;
            float zOffset = -0.2f;

            foreach (string cartas in barlhoTrips[deckLocation])
            {
                GameObject newTopCard = Instantiate(cardPrefab, new Vector3(deckButton.transform.position.x + xOffset, deckButton.transform.position.y, deckButton.transform.position.z + zOffset), Quaternion.identity, deckButton.transform);
                xOffset += 0.5f;
                zOffset -= 0.2f;
                newTopCard.name = cartas;
                tripsOnDisplay.Add(cartas);
                newTopCard.GetComponent<Selecionado>().faceCima = true;
                newTopCard.GetComponent<Selecionado>().inDeckPile = true;

            }
            deckLocation++;
        }
        else
        {
            RestackTopDeck();
        }
    }

    void RestackTopDeck()
    {
        baralho.Clear();
        foreach(string carta in discardPile)
        {
            baralho.Add(carta);
        }
        discardPile.Clear();
        SortDeckIntoTrips();
    }
}
