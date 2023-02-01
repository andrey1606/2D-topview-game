using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

///<summary>
///Управление построением уровня
///</summary>
public class LevelBuilder : MonoBehaviour
{
    // список объектов уровня
    private LevelObjectsList levelObjectsList;
    // ссылки на префабы объектов, игрока и финиша
    [SerializeField] private GameObject bearPrefab;
    [SerializeField] private GameObject berryBushPrefab;
    [SerializeField] private GameObject berryBushPoisonousPrefab;
    [SerializeField] private GameObject dressingPartPrefab;
    [SerializeField] private GameObject edgeWallPrefab;
    [SerializeField] private GameObject logPrefab;
    [SerializeField] private GameObject mushroomPrefab;
    [SerializeField] private GameObject mushroomPoisonousPrefab;
    [SerializeField] private GameObject wolfPrefab;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject finish;
    private Dictionary<string, GameObject> objectsDictionary;

    private void Awake()
    {
        levelObjectsList = new LevelObjectsList();
        DeserializeJson();

        // задаем соответствие между тегом и соответствующим префабом
        objectsDictionary = new Dictionary<string, GameObject>
        {
            {"Wolf", wolfPrefab},
            {"Bear", bearPrefab},
            {"dressingPart", dressingPartPrefab},
            {"MushroomsPoisonous", mushroomPoisonousPrefab},
            {"BerriesPoisonous", berryBushPoisonousPrefab},
            {"Mushrooms", mushroomPrefab},
            {"Berries", berryBushPrefab},
            {"log", logPrefab},
            {"EdgeWall", edgeWallPrefab},
        };

        // перебираем список объектов уровня
        // игрока и финишную точку перемещаем (т. к. они уже есть на сцене уровня)
        // остальные объекты - создаем
        foreach (var levelObject in levelObjectsList.LevelObjects)
        {
            if (objectsDictionary.TryGetValue(levelObject.tag, out GameObject prefab))
            {
                Instantiate(prefab, levelObject.pos, levelObject.rotation);
            }
            else if (levelObject.tag == "Finish")
            {
                finish.transform.position = levelObject.pos;
                finish.transform.rotation = levelObject.rotation;
            }
            else if (levelObject.tag == "Player")
            {
                player.transform.position = levelObject.pos;
                player.transform.rotation = levelObject.rotation;
            }
        }
    }

    // заполняет список объектов уровня, десериализуя файл .json уровня, указанного в TargetLevel.LevelName
    private void DeserializeJson()
    {
        using (FileStream fstream = File.OpenRead($"Assets/Levels/{TargetLevel.LevelName}.json"))
        {
            byte[] array = new byte[fstream.Length];
            fstream.Read(array, 0, array.Length);
            string file = System.Text.Encoding.UTF8.GetString(array);
            var settings = new JsonSerializerSettings
            { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            levelObjectsList.LevelObjects = JsonConvert.DeserializeObject<List<LevelObject>>(file, settings);
        }
    }
}

