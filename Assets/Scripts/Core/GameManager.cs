
using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Reel References")]
    [SerializeField] private ReelController[] reels;

    [Header("Available Symbols")]
    [SerializeField] private Sprite[] availableSymbols;

    [Header("Economy")]
    [SerializeField] private SlotEconomy economy;

    [Header("Payout Multipliers")]
    [SerializeField] private int sevenMultiplier = 10;
    [SerializeField] private int cherriesMultiplier = 5;
    [SerializeField] private int bellMultiplier = 3;
    [SerializeField] private int barMultiplier = 2;

    [Header("Spin Settings")]
    [SerializeField] private float spinDuration = 2.5f;
    [SerializeField] private float stopInterval = 0.5f;

    [Header("Win Popup")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TMP_Text winAmountText;
    [SerializeField] private float winPopupDuration = 2f;

    private bool isSpinning;

    public bool IsSpinning => isSpinning;

    private Coroutine winPopupCoroutine;

    private void Awake()
    {
        if (reels == null || reels.Length != 3)
            Debug.LogError("GameManager requires exactly three reels!", this);

        if (availableSymbols == null || availableSymbols.Length < 2)
            Debug.LogError("At least two symbols are required!", this);

        if (economy == null)
            Debug.LogError("SlotEconomy reference is missing!", this);

        if (winPanel != null)
            winPanel.SetActive(false);
    }

    public void StartSpin()
    {
        if (isSpinning)
            return;

        if (!ValidateReferences())
            return;

        if (!economy.TrySpendBet())
            return;

        if (winPanel != null)
            winPanel.SetActive(false);

        if (winPopupCoroutine != null)
        {
            StopCoroutine(winPopupCoroutine);
            winPopupCoroutine = null;
        }

        StartCoroutine(SpinRoutine());
    }

    private bool ValidateReferences()
    {
        if (reels == null || reels.Length != 3)
            return false;

        if (availableSymbols == null || availableSymbols.Length < 2)
            return false;

        if (economy == null)
            return false;

        foreach (ReelController reel in reels)
        {
            if (reel == null || reel.IsSpinning)
                return false;
        }

        foreach (Sprite symbol in availableSymbols)
        {
            if (symbol == null)
                return false;
        }

        return spinDuration > 0f && stopInterval >= 0f;
    }

    private IEnumerator SpinRoutine()
    {
        isSpinning = true;

        Sprite[] results = new Sprite[reels.Length];

        for (int i = 0; i < reels.Length; i++)
        {
            int randomIndex = Random.Range(0, availableSymbols.Length);
            results[i] = availableSymbols[randomIndex];
        }

        Debug.Log("Starting slot machine spin!");

        for (int i = 0; i < reels.Length; i++)
        {
            float reelDuration = spinDuration + (i * stopInterval);

            StartCoroutine(reels[i].SpinTo(results[i], reelDuration));
        }

        bool anyReelSpinning = true;

        while (anyReelSpinning)
        {
            anyReelSpinning = false;

            foreach (ReelController reel in reels)
            {
                if (reel.IsSpinning)
                {
                    anyReelSpinning = true;
                    break;
                }
            }

            yield return null;
        }

        CheckWin(results);

        isSpinning = false;
    }

    private void CheckWin(Sprite[] results)
    {
        bool isWin =
            results[0] == results[1] &&
            results[1] == results[2];

        if (!isWin)
        {
            Debug.Log(
                $"No win! Results: {results[0].name}, " +
                $"{results[1].name}, {results[2].name}"
            );

            return;
        }

        int multiplier = GetMultiplier(results[0]);
        int winnings = economy.CurrentBet * multiplier;

        economy.AddWinnings(winnings);

        Debug.Log($"JACKPOT! Won {winnings} credits!");

        string winMessage = economy.CurrentBet switch
        {
            10 => "NICE WIN!",
            50 => "BIG WIN!",
            100 => "JACKPOT!",
            _ => "YOU WIN!"
        };

        ShowWinPopup(winMessage, winnings);
    }

    private int GetMultiplier(Sprite winningSymbol)
    {
        string symbolName = winningSymbol.name.ToLower();

        if (symbolName.Contains("seven"))
            return sevenMultiplier;

        if (symbolName.Contains("cherr"))
            return cherriesMultiplier;

        if (symbolName.Contains("bell"))
            return bellMultiplier;

        if (symbolName.Contains("bar"))
            return barMultiplier;

        return 2;
    }

    private void ShowWinPopup(string message, int amount)
    {
        if (winPanel == null)
            return;

        if (winAmountText != null)
            winAmountText.text = message + "\n+" + amount + " CREDITS";

        winPanel.SetActive(true);

        if (winPopupCoroutine != null)
            StopCoroutine(winPopupCoroutine);

        winPopupCoroutine = StartCoroutine(HideWinPopup());
    }

    private IEnumerator HideWinPopup()
    {
        yield return new WaitForSeconds(winPopupDuration);

        if (winPanel != null)
            winPanel.SetActive(false);

        winPopupCoroutine = null;
    }
}
