using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Game Buttons
    public Button dealBtn;
    public Button hitBtn;
    public Button standBtn;
    public Button betBtn;
    public Button bet1Btn;

    private int standClicks = 0;

    // Accede al guión del jugador y del crupier
    public PlayerScript playerScript;
    public PlayerScript dealerScript;

    //texto público para acceder y actualizar - hud
    public Text scoreText;
    public Text dealerScoreText;
    public Text betsText;
    public Text cashText;
    public Text mainText;
    public Text standBtnText;

    //Carta que oculta la segunda carta del crupier
    public GameObject hideCard;
    // How much is bet
    int pot = 0;
    int pot1 = 0;
    void Start()
    {
        // Add on click listeners to the buttons
        dealBtn.onClick.AddListener(() => DealClicked());
        hitBtn.onClick.AddListener(() => HitClicked());
        standBtn.onClick.AddListener(() => StandClicked());
        betBtn.onClick.AddListener(() => BetClicked());
        bet1Btn.onClick.AddListener(() => Bet1Clicked());
    }

    private void DealClicked()
    {
        // Reset round, hide text, prep for new hand
        playerScript.ResetHand();
        dealerScript.ResetHand();
        // Hide deal hand score at start of deal
        dealerScoreText.gameObject.SetActive(false);
        mainText.gameObject.SetActive(false);
        dealerScoreText.gameObject.SetActive(false);
        GameObject.Find("Deck").GetComponent<DeckScript>().Shuffle();
        playerScript.StartHand();
        dealerScript.StartHand();
        // Update the scores displayed
        scoreText.text = "Mano: " + playerScript.handValue.ToString();
        dealerScoreText.text = "Mano: " + dealerScript.handValue.ToString();
        // Place card back on dealer card, hide card
        hideCard.GetComponent<Renderer>().enabled = true;
        // Adjust buttons visibility
        dealBtn.gameObject.SetActive(false);
        hitBtn.gameObject.SetActive(true);
        standBtn.gameObject.SetActive(true);
        standBtnText.text = "Plantarse";
        // Establecer tamaño del sitio
        pot = 40;
        pot1 = 100;
        betsText.text = "Mejor: $" + pot.ToString();
        betsText.text = "Mejor: $" + pot1.ToString();
        playerScript.AdjustMoney(-20);
        cashText.text = "Tengo: $" + playerScript.GetMoney().ToString();

    }

    private void HitClicked()
    {
        // Comprueba que todavía hay espacio en la mesa
        if (playerScript.cardIndex <= 10)
        {
            playerScript.GetCard();
            scoreText.text = "Mano: " + playerScript.handValue.ToString();
            if (playerScript.handValue > 20) RoundOver();
        }
    }

    private void StandClicked()
    {
        standClicks++;
        if (standClicks > 1) RoundOver();
        HitDealer();
        standBtnText.text = "Las vemos!";
    }

    private void HitDealer()
    {
        while (dealerScript.handValue < 16 && dealerScript.cardIndex < 10)
        {
            dealerScript.GetCard();
            dealerScoreText.text = "Mano: " + dealerScript.handValue.ToString();
            if (dealerScript.handValue > 20) RoundOver();
        }
    }

    // Compruebe si hay ganador y perdedor, la mano ha terminado
    void RoundOver()
    {
        // Booleanos (verdadero/falso) por pasarse y blackjack/21
        bool playerBust = playerScript.handValue > 21;
        bool dealerBust = dealerScript.handValue > 21;
        bool player21 = playerScript.handValue == 21;
        bool dealer21 = dealerScript.handValue == 21;
        // Si se ha hecho clic en el soporte menos de dos veces, no hay 21 ni pasarse, salga de la función
        if (standClicks < 2 && !playerBust && !dealerBust && !player21 && !dealer21) return;
        bool roundOver = true;
        // Nos pasamos, apuestas devueltas
        if (playerBust && dealerBust)
        {
            mainText.text = "¡Pasados! Devolver apuestas";
            playerScript.AdjustMoney(pot / 2);
            playerScript.AdjustMoney(pot1 / 2);
        }
        // si el jugador se pasa, el crupier no lo hizo, o si el crupier tiene más puntos, el crupier gana
        else if (playerBust || (!dealerBust && dealerScript.handValue > playerScript.handValue))
        {
            mainText.text = "¡Gana la Banca!";
        }
        // si el crupier se pasa, el jugador no lo hizo o el jugador tiene más puntos, el jugador gana
        else if (dealerBust || playerScript.handValue > dealerScript.handValue)
        {
            mainText.text = "¡He ganado!, je...";
            playerScript.AdjustMoney(pot);
            playerScript.AdjustMoney(pot1);
        }
        // Comprobar si hay empate, devolver apuestas
        else if (playerScript.handValue == dealerScript.handValue)
        {
            mainText.text = "Empate:Devolver apuesta";
            playerScript.AdjustMoney(pot / 2);
            playerScript.AdjustMoney(pot1 / 2);
        }
        else
        {
            roundOver = false;
        }
        // Configurar la interfaz de usuario para el próximo movimiento/mano/turno
        if (roundOver)
        {
            hitBtn.gameObject.SetActive(false);
            standBtn.gameObject.SetActive(false);
            dealBtn.gameObject.SetActive(true);
            mainText.gameObject.SetActive(true);
            dealerScoreText.gameObject.SetActive(true);
            hideCard.GetComponent<Renderer>().enabled = false;
            cashText.text = "Tengo: $" + playerScript.GetMoney().ToString();
            standClicks = 0;
        }
    }

    // Agregue dinero al bote si hace clic en la apuesta
    void BetClicked()
    {
        Text newBet = betBtn.GetComponentInChildren(typeof(Text)) as Text;
        int intBet = int.Parse(newBet.text.ToString().Remove(0, 1));
        playerScript.AdjustMoney(-intBet);
        cashText.text = "Acumulo: $" + playerScript.GetMoney().ToString();
        pot += (intBet * 2);
        betsText.text = "Apuesta: $" + pot.ToString();
    }
    void Bet1Clicked()
    {
        Text newBet = betBtn.GetComponentInChildren(typeof(Text)) as Text;
        int intBet = int.Parse(newBet.text.ToString().Remove(0, 1));
        playerScript.AdjustMoney(-intBet);
        cashText.text = "Acumulo: $" + playerScript.GetMoney().ToString();
        pot1 += (intBet * 2);
        betsText.text = "Apuesta: $" + pot1.ToString();
    }
}
