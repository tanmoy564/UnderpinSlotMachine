
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SlotLeverController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image leverImage;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite pulledSprite;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Button leverButton;

    [Header("Animation")]
    [SerializeField] private float pullDuration = 0.15f;
    [SerializeField] private float returnDuration = 0.2f;

    private bool isPulling;

    private void Awake()
    {
        if (leverImage == null)
            leverImage = GetComponent<Image>();

        if (leverButton == null)
            leverButton = GetComponent<Button>();

        if (leverImage != null)
            leverImage.sprite = idleSprite;

        if (leverButton != null)
            leverButton.onClick.AddListener(PullLever);
    }

    private void OnDestroy()
    {
        if (leverButton != null)
            leverButton.onClick.RemoveListener(PullLever);
    }

    public void PullLever()
    {
        if (isPulling || gameManager == null)
            return;

        if (gameManager.IsSpinning)
            return;

        StartCoroutine(PullRoutine());
    }

    private IEnumerator PullRoutine()
    {
        isPulling = true;

        if (leverButton != null)
            leverButton.interactable = false;

        // Pull lever down.
        if (leverImage != null)
            leverImage.sprite = pulledSprite;

        yield return new WaitForSeconds(pullDuration);

        // Start the slot machine.
        gameManager.StartSpin();

        // Return lever to idle position.
        yield return new WaitForSeconds(returnDuration);

        if (leverImage != null)
            leverImage.sprite = idleSprite;

        isPulling = false;

        if (leverButton != null)
            leverButton.interactable = true;
    }
}
