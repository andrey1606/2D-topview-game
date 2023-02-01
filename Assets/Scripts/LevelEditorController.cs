using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

///<summary>
///Описывает логику редактора уровней
///</summary>
public class LevelEditorController : MonoBehaviour
{
    // список объектов создаваемого уровня
    private LevelObjectsList levelObjectsList;
    // хранит начальное положение камеры при ее перемещении
    private Vector2 startPos;
    private Camera cam;
    internal GameObject targetObject;
    [SerializeField] private TextMeshProUGUI targetGUI;
    // поля для отображения координат целевого объекта
    [SerializeField] private TMP_InputField xInputField;
    [SerializeField] private TMP_InputField yInputField;
    [SerializeField] private TextMeshProUGUI buttonEdit;
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
    [SerializeField] private GameObject procGenForm;
    private Dictionary<string, GameObject> objectsDictionary;
    // true во время ручного изменения координат целевого объекта
    private bool isEdit = false;
    // параметры зума камеры
    private float zoomSpeed = 5.0f;
    private float minZoom = 1f;
    private float maxZoom = 20f;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        UpdateTargetObjectGUI();
        HandleCameraPan();
        HandleCameraZoom();
    }

    // обновляет информацию о целевом объекте
    private void UpdateTargetObjectGUI()
    {
        if (targetObject != null)
        {
            targetGUI.text = $"Target object: {targetObject.name}";
            if (!isEdit)
            {
                xInputField.text = $"{targetObject.transform.position.x.ToString("F2")}";
                yInputField.text = $"{targetObject.transform.position.y.ToString("F2")}";
            }
        }
        else
        {
            targetGUI.text = "Target object: -";
            yInputField.text = "-";
            xInputField.text = "-";
            if (buttonEdit.text != "Edit")
            {
                buttonEdit.text = "Edit";
                isEdit = false;
            }
        }
    }

    // камеру можно двигать средней кнопкой мыши
    private void HandleCameraPan()
    {
        if (Input.GetMouseButtonDown(2))
        {
            startPos = cam.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(2))
        {
            Vector2 pos = new Vector2(cam.ScreenToWorldPoint(Input.mousePosition).x - startPos.x, cam.ScreenToWorldPoint(Input.mousePosition).y - startPos.y);
            cam.transform.position = new Vector3(transform.position.x - pos.x, transform.position.y - pos.y, transform.position.z);
        }
    }

    // зум камеры
    private void HandleCameraZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cam.orthographicSize -= scroll * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }

    ///<summary>
    ///Изменить коордианты целевого объекта
    ///</summary>
    public void ChangeCoordinates()
    {
        if (targetObject != null)
        {
            if (buttonEdit.text == "Edit")
            {
                buttonEdit.text = "Apply";
                isEdit = true;
            }
            else
            {
                if (float.TryParse(xInputField.text, out float newX) && float.TryParse(yInputField.text, out float newY))
                {
                    targetObject.transform.position = new Vector3(newX, newY);
                    buttonEdit.text = "Edit";
                    isEdit = false;
                }
            }
        }
    }

    ///<summary>
    ///Клонировать целевой объект
    ///</summary>
    public void CloneObject()
    {
        if (targetObject != null)
        {
            Vector3 SpawnPos = new Vector3(targetObject.transform.position.x + 2, targetObject.transform.position.y, targetObject.transform.position.z);
            Instantiate(targetObject, SpawnPos, Quaternion.identity);
        }
    }

    ///<summary>
    ///Сбросить целевой объект
    ///</summary>
    public void NullifytargetObject()
    {
        if (targetObject != null) targetObject = null;
    }

    ///<summary>
    ///Удалить целевой объект.
    ///Не срабатывает, если это единственный объект с данным тегом.
    ///</summary>
    public void DeleteObject()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(targetObject.tag);
        if (objects.Length > 1) Destroy(targetObject);
    }

    ///<summary>
    ///Повернуть целевой объект на 90 градусов
    ///</summary>
    public void RotateObject()
    {
        Quaternion rotationZ90 = Quaternion.AngleAxis(90, Vector3.forward);
        targetObject.transform.rotation *= rotationZ90;
    }

    ///<summary>
    ///Запустить процедурную генерацию уровня.
    ///Не срабатывает, если введены некорректные параметры.
    ///</summary>
    public void ProcGen()
    {
        if (GetComponent<ReadParamsProcGen>().SetParams())
        {
            procGenForm.SetActive(false);
            ProcGeneration gen = new ProcGeneration(GetComponent<ReadParamsProcGen>().levelSize, GetComponent<ReadParamsProcGen>().levelDifficulty);
            levelObjectsList = gen.Generation();

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
                {"EdgeWall", edgeWallPrefab}
            };

            // сначала удаляем имеющиеся объекты
            GameObject[] objects = FindObjectsWithTags(objectsDictionary.Keys.ToArray());
            foreach (var c in objects)
                Destroy(c);

            // перебираем список сгенерированных объектов
            // игрока и финишную точку перемещаем (т. к. они уже есть на сцене)
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
    }

    ///<summary>
    ///Сохранить уровень в файл
    ///</summary>
    public void SaveLevel()
    {
        levelObjectsList = new LevelObjectsList();
        GameObject[] objects = FindObjectsWithTags(new string[] { "Wolf", "Bear", "dressingPart", "MushroomsPoisonous",
            "BerriesPoisonous", "Mushrooms", "Berries", "log", "EdgeWall", "finish", "player" });
        foreach (var levelObject in objects)
            levelObjectsList.LevelObjects.Add(new LevelObject(levelObject.transform.position, levelObject.transform.rotation, levelObject.tag));
        SaveLevelInFile.SaveInFile(levelObjectsList);
    }

    private GameObject[] FindObjectsWithTags(string[] tags)
    {
        List<GameObject> objects = new List<GameObject>();
        foreach (string tag in tags)
            objects.AddRange(GameObject.FindGameObjectsWithTag(tag));
        return objects.ToArray();
    }
}