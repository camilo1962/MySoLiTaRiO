using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // --- Este script es tanto para el jugador como para el crupier.

    // Obtener otros guiones
    public CardScript cardScript;   
    public DeckScript deckScript;

    // Valor total de la mano del jugador/crupier
    public int handValue = 0;

    // Apostando dinero
    private int money = 1000;

    // Matriz de objetos de tarjeta en la mesa
    public GameObject[] hand;
    // Índice de la siguiente carta a dar la vuelta
    public int cardIndex = 0;
    // Seguimiento de ases de 1 a 11 conversiones
    List<CardScript> aceList = new List<CardScript>();

    public void StartHand()
    {
        GetCard();
        GetCard();
    }

    // Añadir una mano a la mano del jugador/crupier
    public int GetCard()
    {
        // Obtenga una tarjeta, use la tarjeta de trato para asignar sprite y valor a la tarjeta en la mesa
        int cardValue = deckScript.DealCard(hand[cardIndex].GetComponent<CardScript>());
        // Mostrar tarjeta en la pantalla del juego
        hand[cardIndex].GetComponent<Renderer>().enabled = true;
        // Agregue el valor de la tarjeta al total acumulado de la mano
        handValue += cardValue;
        // Si el valor es 1, es un as
        if (cardValue == 1)
        {
            aceList.Add(hand[cardIndex].GetComponent<CardScript>());
        }
        // Check si deberíamos usar un 11 en lugar de un 1
        AceCheck();
        cardIndex++;
        return handValue;
    }

    // Busque las conversiones as necesarias, 1 a 11 o viceversa
    public void AceCheck()
    {
        // por cada as en el cheque list
        foreach (CardScript ace in aceList)
        {
            if(handValue + 10 < 22 && ace.GetValueOfCard() == 1)
            {
                // si se convierte, ajuste el valor del objeto de la tarjeta y la mano
                ace.SetValue(11);
                handValue += 10;
            } else if (handValue > 21 && ace.GetValueOfCard() == 11)
            {
                // si se convierte, ajuste el valor del objeto del juego y el valor de la mano
                ace.SetValue(1);
                handValue -= 10;
            }
        }
    }

    // Add or subtract from money, for bets
    public void AdjustMoney(int amount)
    {
        money += amount;
    }

    // Output players current money amount
    public int GetMoney()
    {
        return money;
    }

    // Hides all cards, resets the needed variables
    public void ResetHand()
    {
        for(int i = 0; i < hand.Length; i++)
        {
            hand[i].GetComponent<CardScript>().ResetCard();
            hand[i].GetComponent<Renderer>().enabled = false;
        }
        cardIndex = 0;
        handValue = 0;
        aceList = new List<CardScript>();
    }
}
