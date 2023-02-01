using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Linq;

///<summary>
///Описывает логику работы меню
///</summary>
public class MenuController : MonoBehaviour
{
    [SerializeField] private RectTransform prefabLevel;
    [SerializeField] private RectTransform prefabStats;
    [SerializeField] private RectTransform contentLevel;
    [SerializeField] private RectTransform contentStats;
    [SerializeField] private GameObject statsWindow;
    [SerializeField] private GameObject procGenForm;

    private void Awake()
    {
        LoadLevelList();
        StatsManagement.LoadStatsList();
    }

    // заполняет список уровней
    private void LoadLevelList()
    {
        // формируем коллекцию с именами файлов, соответствующих уровням
        List<string> levels = new List<string>();
        List<string> files = (from a in Directory.GetFiles("Assets/Levels/") select Path.GetFileName(a)).ToList();
        foreach (var item in files)
            if (item.EndsWith(".json"))
                levels.Add(item);

        foreach (Transform item in contentLevel)
            Destroy(item.gameObject);

        // заполняем список
        foreach (var item in levels)
        {
            GameObject instance = GameObject.Instantiate(prefabLevel.gameObject);
            instance.transform.SetParent(contentLevel, false);
            InitializeLevelView(instance, item);
        }
    }

    // адаптирует переданный экземпляр списка под конкретный уровень
    private void InitializeLevelView(GameObject viewGameObject, string nameLevel)
    {
        LevelView view = new LevelView(viewGameObject.transform);
        view.nameLevel.text = nameLevel.Substring(0, nameLevel.IndexOf("."));

        // запускаем выбранный уровень
        view.startButton.onClick.AddListener(
            () =>
            {
                TargetLevel.LevelName = view.nameLevel.text;
                LoadScenes.StartLevel();
            }
        );

        // выводим статистику прохождений выбранного уровня для текущего пользователя
        view.statsButton.onClick.AddListener(
            () =>
            {
                statsWindow.SetActive(true);
                if (StatsManagement.statsList.Count > 0)
                {
                    foreach (var item in StatsManagement.statsList)
                    {
                        if (item.login == GameObject.Find("Main Camera").GetComponent<AuthorizationManagement>().CurrentUserGUI.text
                            && view.nameLevel.text == item.levelName)
                        {
                            GameObject instance = GameObject.Instantiate(prefabStats.gameObject);
                            instance.transform.SetParent(contentStats, false);
                            instance.GetComponent<TextMeshProUGUI>().text =
                                $"Score: {item.score} Time: {item.totalTime}";
                        }
                    }
                }
            }
        );
    }

    ///<summary>
    ///Запускает процедурную генерацию уровня.
    ///Не срабатывает, если введены некорректные параметры.
    ///Сразу сохраняет уровень в файл и обновляет список.
    ///</summary>
    public void GenLevel()
    {
        if (GetComponent<ReadParamsProcGen>().SetParams())
        {
            procGenForm.SetActive(false);
            ProcGeneration gen = new ProcGeneration(GetComponent<ReadParamsProcGen>().levelSize, GetComponent<ReadParamsProcGen>().levelDifficulty);
            SaveLevelInFile.SaveInFile(gen.Generation());
            LoadLevelList();
        }
    }

    ///<summary>
    ///Запустить редактор уровней
    ///</summary>
    public void LaunchLevelEditor()
    {
        LoadScenes.LaunchLevelEditor();
    }

    ///<summary>
    ///Выйти из игры
    ///</summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}



