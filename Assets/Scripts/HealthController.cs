using UnityEngine;
using UnityEngine.UI;
using TMPro;

///<summary>
///Здоровье игрока и статус-эффекты
///</summary>
public class HealthController : MonoBehaviour
{
    [SerializeField] internal float startingHealth;
    internal float currentHealth;
    // шкала здоровья
    [SerializeField] private Image healthBar;
    // % здоровья на шкале
    [SerializeField] private TextMeshProUGUI percentageHealthProgress;
    // нижний и верхний слоты статус-эффектов
    [SerializeField] private TextMeshProUGUI downStatus;
    [SerializeField] private TextMeshProUGUI topStatus;
    // флаги статус-эффектов
    internal bool isBleeding = false;
    internal bool isPoisoning = false;
    // считает, сколько прошло с начала отравления
    private float poisoningTimer;
    // длительность отравления
    [SerializeField] private float poisoningDuration;
    // скорость, с которой уменьшается здоровье при отравлении
    [SerializeField] private float poisonSpeed;
    // скорость, с которой уменьшается здоровье при кровотечении
    [SerializeField] private float bleedingSpeed;

    private void Awake() => currentHealth = startingHealth;

    private void Update()
    {
        FillHealthBar();
        UpdateStatusEffects();
        if (currentHealth <= 0)
            GetComponent<InteractionWithResources>().FinishGame("GAME OVER");
    }

    // заполняет шкалу здоровья
    private void FillHealthBar()
    {
        float progress = currentHealth / startingHealth;
        healthBar.fillAmount = progress;
        percentageHealthProgress.text = $"HP: {(progress * 100).ToString("F1")}%";
    }

    // отслеживает статус-эффекты
    private void UpdateStatusEffects()
    {
        if (!downStatus.gameObject.activeSelf && topStatus.gameObject.activeSelf)
            HideTopStatus();
        if (isPoisoning)
            UpdatePoisoning();
        if (isBleeding)
            UpdateBleeding();
    }

    // скрывает верхний слот 
    private void HideTopStatus()
    {
        downStatus.gameObject.SetActive(true);
        topStatus.gameObject.SetActive(false);
        downStatus.text = topStatus.text;
    }

    // логика работы отравления
    private void UpdatePoisoning()
    {
        if (poisoningTimer < poisoningDuration)
        {
            SetStatusEffectGUI("Poisoning");
            poisoningTimer += Time.deltaTime;
            currentHealth -= Time.deltaTime * poisonSpeed;
        }
        else
            DisablePoisoning();
    }

    // логика работы кровотечения
    private void UpdateBleeding() => currentHealth -= Time.deltaTime * bleedingSpeed;

    // отключает статус-эффект
    private void DisableStatus(string status, ref bool statusFlag)
    {
        statusFlag = false;
        if (downStatus.gameObject.activeSelf && downStatus.text == status)
            downStatus.gameObject.SetActive(false);
        else
            topStatus.gameObject.SetActive(false);
    }

    // отключает отравление
    private void DisablePoisoning() => DisableStatus("Poisoning", ref isPoisoning);

    // отключает кровотечение
    internal void DisableBleeding() => DisableStatus("Bleeding", ref isBleeding);

    // получение урона извне
    internal void TakeDamage(float _damage)
    {
        if ((currentHealth - _damage) > 0)
        {
            currentHealth -= _damage;
            float x = UnityEngine.Random.Range(0, 100);
            if (x < 20 && isBleeding == false)
            {
                isBleeding = true;
                SetStatusEffectGUI("Bleeding");
            }
        }
        else currentHealth = 0;
    }

    // выводит статус-эффект в слот
    private void SetStatusEffectGUI(string _status)
    {
        if (downStatus.gameObject.activeSelf && downStatus.text != _status)
        {
            topStatus.gameObject.SetActive(true);
            topStatus.text = _status;
        }
        else
        {
            downStatus.gameObject.SetActive(true);
            downStatus.text = _status;
        }
    }
}
