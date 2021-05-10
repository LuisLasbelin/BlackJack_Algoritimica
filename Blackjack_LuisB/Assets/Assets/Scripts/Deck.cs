using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text probMessage;
    public Text bancaText;
    public Text apuestaText;
    public int iterations;

    public int[] values = new int[52];
    int cardIndex = 0;
    int banca = 1000;
    int apuesta = 0;

    private void Awake()
    {    
        InitCardValues();        

    }

    private void Start()
    {
        ShuffleCards();
        StartGame();

        apuestaText.text = apuesta.ToString();
        bancaText.text = banca.ToString();
    }

    private void InitCardValues()
    {
        /*DONE:
         * Asignar un valor a cada una de las 52 cartas del atributo "values".
         * En principio, la posición de cada valor se deberá corresponder con la posición de faces. 
         * Por ejemplo, si en faces[1] hay un 2 de corazones, en values[1] debería haber un 2.
         */
        int cartasPalo = 1;
        for (int i = 0; i < values.Length; i++)
        {
            values[i] = cartasPalo;
            cartasPalo++;
            // Cuando se han hecho todas las cartas de un palo, hay 12 cartas por palo con valores del 1 al 13
            if(cartasPalo == 14)
            {
                cartasPalo = 1;
            }
        }
    }

    private void ShuffleCards()
    {
        /*DONE:
         * Barajar las cartas aleatoriamente.
         * El método Random.Range(0,n), devuelve un valor entre 0 y n-1
         * Si lo necesitas, puedes definir nuevos arrays.
         */
        for (int i = 0; i < iterations; i++)
        {
            // Elegimos dos cartas al azar del mazo
            int index = Random.Range(0, 52);
            int indexAlt = Random.Range(0, 52);
            // Siempre que no sean la misma carta, las intercambiamos
            if(index != indexAlt)
            {
                // Cambiamos dos cartas de posicion en faces
                Sprite auxSprite = faces[index];
                faces[index] = faces[indexAlt];
                faces[indexAlt] = auxSprite;
                // Cambiamos sus valores de posicion
                int auxInt = values[index];
                values[index] = values[indexAlt];
                values[indexAlt] = auxInt;
            }
        }
    }

    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();

            CardHand dealerCards = dealer.GetComponent<CardHand>();
            CardHand playerCards = player.GetComponent<CardHand>();
            // Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
            if (playerCards.points == 21)
            {
                finalMessage.text = "BlackJack del jugador!";
                GanaJugador();
            }
            else if (dealerCards.points == 21)
            {
                finalMessage.text = "BlackJack de la casa!";
                PierdeJugador();
            }
            else if (playerCards.points == 21 && dealerCards.points == 21)
            {
                finalMessage.text = "¿¿¿¿DOBLE BLACKJACK????";
                Empate();
            }

            
        } 
    }

    private void CalculateProbabilities()
    {
        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
         * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
         * - Probabilidad de que el jugador obtenga más de 21 si pide una carta          
         */
    }

    void PushDealer()
    {
        dealer.GetComponent<CardHand>().Push(faces[cardIndex],values[cardIndex]);
        cardIndex++;        
    }

    void PushPlayer()
    {

        // Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]);
        cardIndex++;
        CalculateProbabilities();
    }       

    public void Hit()
    {
        /*DONE: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        CardHand dealerCards = dealer.GetComponent<CardHand>();
        if (cardIndex == 4)
        {
            dealerCards.cards[0].GetComponent<CardModel>().ToggleFace(true);
        }
        
        //Repartimos carta al jugador
        PushPlayer();

        // Comprobamos si el jugador ya ha perdido y mostramos mensaje    
        CardHand playerCards = player.GetComponent<CardHand>();
        if (playerCards.points > 21)
        {
            finalMessage.text = "Has perdido";
            PierdeJugador();
        }

    }

    public void Stand()
    {
        /*DONE: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        CardHand dealerCards = dealer.GetComponent<CardHand>();
        if (cardIndex == 4)
        {
            dealerCards.cards[0].GetComponent<CardModel>().ToggleFace(true);
        }

        /*DONE:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */
        CardHand playerCards = player.GetComponent<CardHand>();

        if (dealerCards.points <= 16)
        {
            PushDealer();
            Stand();
        }
        if(dealerCards.points >= 17)
        {
            // Se planta
            if((21 - playerCards.points) > (21 - dealerCards.points))
            {
                finalMessage.text = "Gana la casa >:(";
                PierdeJugador();

            } else if ((21 - playerCards.points) < (21 - dealerCards.points))
            {
                finalMessage.text = "Has ganado!! :D";
                GanaJugador();

            } else
            {
                finalMessage.text = "Es un empate";
                Empate();
            }
        }
         
    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();          
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }

    public void SubirApuesta()
    {
        if(banca >= 10)
        {
            banca -= 10;
            apuesta += 10;
        }

        bancaText.text = banca.ToString();
        apuestaText.text = apuesta.ToString();
    }

    public void GanaJugador()
    {
        banca += apuesta * 2;
        apuesta = 0;

        bancaText.text = banca.ToString();
        apuestaText.text = apuesta.ToString();
    }
    public void PierdeJugador()
    {
        apuesta = 0;

        bancaText.text = banca.ToString();
        apuestaText.text = apuesta.ToString();
    }

    public void Empate()
    {
        banca += apuesta;
        apuesta = 0;

        bancaText.text = banca.ToString();
        apuestaText.text = apuesta.ToString();
    }
}
