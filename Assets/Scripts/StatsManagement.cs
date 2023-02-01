using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

///<summary>
///Отвечает за работу со статистикой прохождения уровней
///</summary>
public static class StatsManagement
{
    // хранит всю статистику
    public static List<Stats> statsList = new List<Stats>();
    // true при внесении новой записи
    private static bool isChanged = false;
    /// <summary> Загружает статистику из файла. </summary>
    internal static void LoadStatsList()
    {
        if (File.Exists("Assets/JsonFiles/stats.json"))
        {
            using (FileStream fstream = File.OpenRead("Assets/JsonFiles/stats.json"))
            {
                byte[] array = new byte[fstream.Length];
                fstream.Read(array, 0, array.Length);
                string file = System.Text.Encoding.UTF8.GetString(array);
                var settings = new JsonSerializerSettings
                { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented };
                statsList = JsonConvert.DeserializeObject<List<Stats>>(file, settings);
            }
        }
    }

    /// <summary> Создает новую запись статистики. </summary>
    internal static void AddStats(Stats stats)
    {
        statsList.Add(stats);
        isChanged = true;
    }

    /// <summary> Cохраняет статистику в файл. </summary>
    internal static void SaveStats()
    {
        if (statsList.Count != 0 && isChanged)
        {
            using (StreamWriter file = File.CreateText("Assets/JsonFiles/stats.json"))
            {
                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented };
                JsonSerializer serializer = JsonSerializer.Create(settings);
                serializer.Serialize(file, statsList);
            }
        }
    }
}