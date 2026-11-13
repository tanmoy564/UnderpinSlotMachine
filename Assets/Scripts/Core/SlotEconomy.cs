
using TMPro;
using UnityEngine;

public class SlotEconomy : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text creditLabel;
    public TMP_Text betLabel;

    [Header("Game Settings")]
    public int startingCredits = 1000;
    public int currentBet = 10;

    public int Credits { get; private set; }

    public int CurrentBet => currentBet;

    private void Awake()
    {
        Credits = startingCredits;
        UpdateUI();
    }

    private void Start()
    {
        UpdateUI();
    }

    public void SelectBet(int amount)
    {
        if (amount != 10 && amount != 50 && amount != 100)
            return;

        if (amount > Credits)
        {
            Debug.Log("Not enough credits!");
            return;
        }

        currentBet = amount;
        UpdateUI();

        Debug.Log("Bet selected: " + currentBet);
    }

    public bool TrySpendBet()
    {
        if (Credits < currentBet)
        {
            Debug.Log("Not enough credits to spin!");
            return false;
        }

        Credits -= currentBet;
        UpdateUI();

        return true;
    }

    public void AddWinnings(int amount)
    {
        if (amount <= 0)
            return;

        Credits += amount;
        UpdateUI();

        Debug.Log("Won " + amount + " credits!");
    }

    public void RefundBet()
    {
        Credits += currentBet;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (creditLabel != null)
            creditLabel.text = "CREDITS: " + Credits;

        if (betLabel != null)
            betLabel.text = "BET: " + currentBet;
    }
}
