using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

///<summary>
///Ресурсы и взаимодействие с ними
///</summary>
public class InteractionWithResources : MonoBehaviour
{
    // следующие 6 полей отображают количество ресурсов в инфентаре
    [SerializeField] private TextMeshProUGUI mushroomsCount;
    [SerializeField] private TextMeshProUGUI mushroomsPoisonousCount;
    [SerializeField] private TextMeshProUGUI berriesCount;
    [SerializeField] private TextMeshProUGUI berriesPoisonousCount;
    [SerializeField] private TextMeshProUGUI dressingPartCount;
    [SerializeField] private TextMeshProUGUI dressingCount;
    // отображает текущий % добычи ресурса
    [SerializeField] private TextMeshProUGUI percentageExtractionProgress;
    // отображает текущий % загруженности рюкзака
    [SerializeField] private TextMeshProUGUI percentageBackpackProgress;
    // шкала добычи ресурса
    [SerializeField] private Image extractionBar;
    // шкала загруженности рюкзака
    [SerializeField] private Image backpackBar;
    // экран, появляющийся в конце игры
    [SerializeField] internal GameObject finishScreen;
    // подсчет времени прохождения уровня
    internal float totalTime;
    // вместительность рюкзака
    [SerializeField] private float maxCapacity;
    // текущая загруженность рюкзака
    private float currentCapacity = 0f;
    // кнопка создания перевязки
    [SerializeField] Button dressingButton;
    // кнопка применения перевязки
    [SerializeField] Button dressingButtonApply;
    // кнопка поедания ягод
    [SerializeField] Button startEating;
    // подсчет примененных перевязок
    private byte appliedDressings = 0;
    // минимальное расстояние для возможности добычи ресурса
    [SerializeField] private float extractionDistance;
    // добываемый ресурс
    private GameObject currentResource;
    // хранит количество каждого ресурса
    private Dictionary<string, byte> resourcesCountDictionary;

    private void Awake()
    {
        resourcesCountDictionary = new Dictionary<string, byte>
        {
            {"Mushrooms", 0},
            {"MushroomsPoisonous", 0},
            {"Berries", 0},
            {"BerriesPoisonous", 0},
            {"dressingPart", 0},
            {"dressing", 0}
        };
    }

    private void Update()
    {
        totalTime += Time.deltaTime;
        MonitorCapacity();
        MonitorExtractionProcess();
        MonitorClickOnResource();
        MonitorExtractionDistance();
        FillExtractionBar();
        FillBackpackBar();
        UpdateButtonsState();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // проверяем, выполнены ли условия для завершения уровня
        if (other.gameObject.tag == "Finish" && resourcesCountDictionary["Berries"] >= 15 && resourcesCountDictionary["Mushrooms"] >= 15)
            FinishGame("LEVEL COMPLETE");
    }

    // завершает уровень
    internal void FinishGame(string _text)
    {
        Time.timeScale = 0;
        TimeSpan ts = TimeSpan.FromSeconds(totalTime);
        finishScreen.SetActive(true);
        finishScreen.transform.Find("label").GetComponent<TextMeshProUGUI>().text = _text;
        finishScreen.transform.Find("score").GetComponent<TextMeshProUGUI>().text = "Score: " + CalculateScore().ToString("F1");
        finishScreen.transform.Find("time").GetComponent<TextMeshProUGUI>().text = $"Time: {ts.Minutes} m. {ts.Seconds} s. {ts.Milliseconds} ms.";
        // если уровень завершен успешно, сохраняем статистику о прохождении
        if (_text == "LEVEL COMPLETE")
            StatsManagement.AddStats(new Stats(CurrentUser.User, TargetLevel.LevelName, CalculateScore(), totalTime));
    }

    private void UpdateButtonsState()
    {
        var health = GetComponent<HealthController>();
        // кнопка активна, если хватает материала для создания новой перевязки
        dressingButton.gameObject.SetActive(resourcesCountDictionary["dressingPart"] >= 3);
        // кнопка активна, если есть кровотечение
        dressingButtonApply.gameObject.SetActive(resourcesCountDictionary["dressing"] > 0 && health.isBleeding);
        // кнопка активна, если есть хотя бы одно ягода, а здоровье не полное
        startEating.gameObject.SetActive(resourcesCountDictionary["Berries"] >= 1 && health.currentHealth != health.startingHealth);
    }

    // заполняет шкалу загруженности рюкзака
    private void FillBackpackBar()
    {
        float fullnessBackpack = currentCapacity / maxCapacity;
        backpackBar.fillAmount = fullnessBackpack;
        percentageBackpackProgress.text = $"BP: {(fullnessBackpack * 100).ToString("F1")}%";
    }

    // заполняет шкалу добычи ресурса
    private void FillExtractionBar()
    {
        if (currentResource != null)
        {
            var resource = currentResource.GetComponent<ResourceController>();
            float progress = resource.currentExtractionTime / resource.extractionTime;
            extractionBar.fillAmount = progress;
            percentageExtractionProgress.text = $"Extraction... {(progress * 100).ToString("F1")}%";
        }
    }

    // контролирует загруженность рюкзака и завершает игру, если она становится максимальной
    private void MonitorCapacity()
    {
        if (currentCapacity >= maxCapacity)
        {
            currentCapacity = maxCapacity;
            FinishGame("GAME OVER");
        }
    }

    // обрабатывает клики на ресурсы
    private void MonitorClickOnResource()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            // проверяем, был ли клик на ресурс
            if (hit.collider != null && hit.collider.gameObject.GetComponent<ResourceController>() != null)
            {
                var resource = hit.collider.gameObject.GetComponent<ResourceController>();
                // проверяем расстояние
                if (Vector2.Distance(transform.position, resource.transform.position) <= extractionDistance && currentResource == null)
                {
                    currentResource = resource.gameObject;
                    resource.StartExtraction();
                    extractionBar.transform.parent.gameObject.SetActive(true);
                }
            }
        }
    }

    // контролирует расстояние до добываемого ресурса и прерывает добычу, если есть превышение
    private void MonitorExtractionDistance()
    {
        if (currentResource != null)
        {
            if (Vector2.Distance(transform.position, currentResource.transform.position) > extractionDistance)
            {
                var resource = currentResource.GetComponent<ResourceController>();
                resource.StopExtraction();
                extractionBar.transform.parent.gameObject.SetActive(false);
                currentResource = null;
            }
        }
    }

    // действия при успешном завершении добычи
    private void MonitorExtractionProcess()
    {
        if (currentResource != null)
        {
            var resource = currentResource.GetComponent<ResourceController>();
            if (!resource.isExtraction)
            {
                AddResource(currentResource.tag);
                currentResource = null;
                extractionBar.transform.parent.gameObject.SetActive(false);
            }
        }
    }

    // добаляет ресурс, вес ресурсов отличается
    private void AddResource(string _tag)
    {
        resourcesCountDictionary[_tag]++;
        UpdateResourcesOnScreen();
        switch (_tag)
        {
            case "dressingPart":
                currentCapacity += 5;
                break;
            case "dressing":
                currentCapacity += 15;
                break;
            default:
                currentCapacity += 10;
                break;
        }
    }

    // удаляет ресурс
    // вес уменьшается только при удалении материалов для перевязки, т. к. создается перевязка
    private void RemoveResource(string _tag)
    {
        if (_tag == "dressingPart")
        {
            resourcesCountDictionary[_tag] -= 3;
            currentCapacity -= 15;
        }
        else
            resourcesCountDictionary[_tag]--;
        UpdateResourcesOnScreen();
    }

    // обновляет значения количества ресурсов на экране
    private void UpdateResourcesOnScreen()
    {
        berriesCount.text = resourcesCountDictionary["Berries"].ToString();
        berriesPoisonousCount.text = resourcesCountDictionary["BerriesPoisonous"].ToString();
        mushroomsCount.text = resourcesCountDictionary["Mushrooms"].ToString();
        mushroomsPoisonousCount.text = resourcesCountDictionary["MushroomsPoisonous"].ToString();
        dressingPartCount.text = resourcesCountDictionary["dressingPart"].ToString();
        dressingCount.text = resourcesCountDictionary["dressing"].ToString();
    }

    // создает перевязку
    public void CreateDressing()
    {
        AddResource("dressing");
        RemoveResource("dressingPart");
    }

    // применяет перевязку
    public void ApplyDressing()
    {
        RemoveResource("dressing");
        appliedDressings++;
        GetComponent<HealthController>().DisableBleeding();
    }

    // восстановление здоровья с помощью поедания ягод
    // при наличии ядовитых ягод может съесть одну из них и получить отравление
    public void EatBerries()
    {
        var health = GetComponent<HealthController>();
        int totalBerries = resourcesCountDictionary["BerriesPoisonous"] + resourcesCountDictionary["Berries"];
        byte chance = Convert.ToByte(((float)resourcesCountDictionary["BerriesPoisonous"] / (float)totalBerries) * 100f);
        if (resourcesCountDictionary["BerriesPoisonous"] > 0 && UnityEngine.Random.Range(0, 100) < chance)
        {
            RemoveResource("BerriesPoisonous");
            health.isPoisoning = true;
        }
        else
        {
            health.currentHealth = Math.Min(health.currentHealth + 5, health.startingHealth);
            RemoveResource("Berries");
        }
    }

    // рассчитывает количество очков по заданной формуле 
    public float CalculateScore()
    {
        var health = GetComponent<HealthController>();
        float ratio = 0;
        if (totalTime < 1000) ratio = 1000 - totalTime;
        float score = 100 - appliedDressings * 3 - (resourcesCountDictionary["MushroomsPoisonous"] / 100) * 3 - (resourcesCountDictionary["BerriesPoisonous"] / 100) * 3 + ratio + (health.currentHealth / health.startingHealth) * 100;
        if (score > 0) return score;
        else return 0;
    }
}
