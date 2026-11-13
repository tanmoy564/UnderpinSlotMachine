
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

    [Header("Betting UI")]
    [SerializeField] private GameObject bettingPanel;

    [Header("Payout Multipliers")]
    [SerializeField] private int sevenMultiplier = 10;
    [SerializeField] private int cherriesMultiplier = 5;
    [SerializeField] private int bellMultiplier = 3;
    [SerializeField] private int barMultiplier = 2;

    [Header("Bonus Settings")]
    [SerializeField] private Sprite goldenStarSymbol;
    [SerializeField] private int bonusMultiplier = 5;

    [Header("Spin Settings")]
    [SerializeField] private float spinDuration = 2.5f;
    [SerializeField] private float stopInterval = 0.5f;

    [Header("Win Popup")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TMP_Text winAmountText;
    [SerializeField] private float winPopupDuration = 2f;

    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;

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

        if (bettingPanel != null)
            bettingPanel.SetActive(true);
    }

    public void StartSpin()
    {
        if (isSpinning)
            return;

        if (winPanel != null && winPanel.activeSelf)
            return;

        if (!ValidateReferences())
            return;

        if (!economy.TrySpendBet())
            return;

        // Hide betting options during the spin.
        if (bettingPanel != null)
            bettingPanel.SetActive(false);

        if (audioManager != null)
        {
            audioManager.PlayButtonClick();
            audioManager.StartSpinSound();
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
        bool[] previousReelStates = new bool[reels.Length];

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
            previousReelStates[i] = reels[i].IsSpinning;
        }

        // Detect each reel stopping.
        bool anyReelSpinning = true;

        while (anyReelSpinning)
        {
            anyReelSpinning = false;

            for (int i = 0; i < reels.Length; i++)
            {
                bool currentlySpinning = reels[i].IsSpinning;

                if (previousReelStates[i] && !currentlySpinning)
                {
                    if (audioManager != null)
                        audioManager.PlayReelStop();
                }

                previousReelStates[i] = currentlySpinning;

                if (currentlySpinning)
                    anyReelSpinning = true;
            }

            yield return null;
        }

        if (audioManager != null)
            audioManager.StopSpinSound();

        CheckWin(results);

        // Show betting options again after all reels stop.
        if (bettingPanel != null)
            bettingPanel.SetActive(true);

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

        // Golden Star bonus.
        if (goldenStarSymbol != null && results[0] == goldenStarSymbol)
        {
            int bonusWinnings = economy.CurrentBet * bonusMultiplier;

            economy.AddWinnings(bonusWinnings);

            if (audioManager != null)
                audioManager.PlayBonus();

            Debug.Log("LUCKY BONUS! Won " + bonusWinnings + " credits!");

            ShowWinPopup("LUCKY BONUS!", bonusWinnings);
            return;
        }

        int multiplier = GetMultiplier(results[0]);
        int winnings = economy.CurrentBet * multiplier;

        economy.AddWinnings(winnings);

        string winMessage = economy.CurrentBet switch
        {
            10 => "NICE WIN!",
            50 => "BIG WIN!",
            100 => "JACKPOT!",
            _ => "YOU WIN!"
        };

        if (audioManager != null)
        {
            if (economy.CurrentBet == 100)
                audioManager.PlayJackpot();
            else
                audioManager.PlayCoinPayout();
        }

        Debug.Log("Won " + winnings + " credits!");

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
