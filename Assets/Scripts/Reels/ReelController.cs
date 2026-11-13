
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReelController : MonoBehaviour
{
    [Header("Reel References")]
    [SerializeField] private Image[] symbolImages;

    [Header("Available Symbols")]
    [SerializeField] private Sprite[] availableSymbols;

    [Header("Spin Settings")]
    [SerializeField] private float spinSpeed = 900f;

    [Tooltip("How long the reel spends slowing down.")]
    [SerializeField] private float stopDuration = 0.8f;

    private bool isSpinning;

    private readonly List<Image> orderedSymbols = new List<Image>();
    private readonly Dictionary<Image, float> originalGaps =
        new Dictionary<Image, float>();

    private RectTransform reelRect;

    public bool IsSpinning => isSpinning;

    private void Awake()
    {
        reelRect = GetComponent<RectTransform>();

        if (symbolImages == null || symbolImages.Length < 3)
        {
            Debug.LogError($"{name}: Assign all reel symbol Images.", this);
            return;
        }

        if (availableSymbols == null || availableSymbols.Length == 0)
        {
            Debug.LogError($"{name}: Assign available symbols.", this);
            return;
        }

        SetupSymbolOrder();
    }

    private void SetupSymbolOrder()
    {
        orderedSymbols.Clear();

        foreach (Image symbol in symbolImages)
        {
            if (symbol != null)
                orderedSymbols.Add(symbol);
        }

        // Sort from highest Y position to lowest Y position.
        orderedSymbols.Sort((a, b) =>
            b.rectTransform.anchoredPosition.y.CompareTo(
                a.rectTransform.anchoredPosition.y
            ));

        originalGaps.Clear();

        // Store the actual gap between every neighbouring pair.
        for (int i = 0; i < orderedSymbols.Count - 1; i++)
        {
            float currentY =
                orderedSymbols[i].rectTransform.anchoredPosition.y;

            float nextY =
                orderedSymbols[i + 1].rectTransform.anchoredPosition.y;

            originalGaps[orderedSymbols[i]] = currentY - nextY;
        }
    }



    public IEnumerator SpinTo(Sprite targetSymbol, float duration)
    {
        if (isSpinning || targetSymbol == null ||
            orderedSymbols.Count < 3)
            yield break;

        isSpinning = true;

        float requestedStopDuration = Mathf.Min(stopDuration, duration);
        float spinningDuration = Mathf.Max(0f, duration - requestedStopDuration);

        // PHASE 1: Constant-speed spinning.
        float elapsed = 0f;

        while (elapsed < spinningDuration)
        {
            float delta = Mathf.Min(
                Time.deltaTime,
                spinningDuration - elapsed
            );

            MoveSymbols(spinSpeed * delta);

            elapsed += delta;
            yield return null;
        }

        // Select the highest symbol for the final result.
        Image targetImage = GetHighestSymbol();

        if (targetImage == null)
        {
            isSpinning = false;
            yield break;
        }

        // Assign the result while it is above the visible area.
        targetImage.sprite = targetSymbol;

        float startY = targetImage.rectTransform.anchoredPosition.y;

        if (startY <= 0f)
        {
            isSpinning = false;
            Debug.LogError($"{name}: Target symbol is not above the centre.");
            yield break;
        }

        // Calculate enough time to stop without exceeding spinSpeed.
        float minimumStopDuration = (2f * startY) / spinSpeed;

        float actualStopDuration = Mathf.Max(
            requestedStopDuration,
            minimumStopDuration
        );

        // PHASE 2: Controlled deceleration.
        float stopElapsed = 0f;

        while (stopElapsed < actualStopDuration)
        {
            float delta = Mathf.Min(
                Time.deltaTime,
                actualStopDuration - stopElapsed
            );

            stopElapsed += delta;

            float progress = Mathf.Clamp01(
                stopElapsed / actualStopDuration
            );

            // Quadratic ease-out: speed decreases continuously.
            float remainingFraction = 1f - progress;
            float desiredY = startY *
                remainingFraction * remainingFraction;

            float currentY =
                targetImage.rectTransform.anchoredPosition.y;

            float movement = Mathf.Max(0f, currentY - desiredY);

            if (movement > 0f)
                MoveSymbols(movement);

            yield return null;
        }

        // Exact final alignment.
        float finalY =
            targetImage.rectTransform.anchoredPosition.y;

        if (finalY > 0f)
            MoveSymbols(finalY);

        isSpinning = false;
    }



    private Image GetHighestSymbol()
    {
        Image highest = null;
        float highestY = float.MinValue;

        foreach (Image symbol in orderedSymbols)
        {
            if (symbol == null)
                continue;

            float y = symbol.rectTransform.anchoredPosition.y;

            if (y > highestY)
            {
                highestY = y;
                highest = symbol;
            }
        }

        return highest;
    }

    private void MoveSymbols(float distance)
    {
        if (distance <= 0f)
            return;

        // Move every symbol down by the same amount.
        foreach (Image symbol in orderedSymbols)
        {
            if (symbol == null)
                continue;

            RectTransform rect = symbol.rectTransform;
            Vector2 position = rect.anchoredPosition;

            position.y -= distance;
            rect.anchoredPosition = position;
        }

        // Recycle symbols that have moved below the reel.
        float bottomLimit = -reelRect.rect.height / 2f - 100f;

        bool recycled;

        do
        {
            recycled = false;

            Image lowest = null;
            float lowestY = float.MaxValue;

            foreach (Image symbol in orderedSymbols)
            {
                if (symbol == null)
                    continue;

                float y = symbol.rectTransform.anchoredPosition.y;

                if (y < lowestY)
                {
                    lowestY = y;
                    lowest = symbol;
                }
            }

            if (lowest != null && lowestY < bottomLimit)
            {
                float highestY = float.MinValue;

                foreach (Image symbol in orderedSymbols)
                {
                    if (symbol == null || symbol == lowest)
                        continue;

                    highestY = Mathf.Max(
                        highestY,
                        symbol.rectTransform.anchoredPosition.y
                    );
                }

                float gap = originalGaps.ContainsKey(lowest)
                    ? originalGaps[lowest]
                    : 95f;

                Vector2 position =
                    lowest.rectTransform.anchoredPosition;

                position.y = highestY + gap;

                lowest.rectTransform.anchoredPosition = position;

                int randomIndex = Random.Range(
                    0, availableSymbols.Length
                );

                lowest.sprite = availableSymbols[randomIndex];

                recycled = true;
            }

        } while (recycled);
    }
}
