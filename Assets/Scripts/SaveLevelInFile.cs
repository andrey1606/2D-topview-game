using System.Collections.Generic;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
using System.IO;
using System.Linq;

///<summary>
///Содержит метод для сохранения уровня в файл
///</summary>
public static class SaveLevelInFile
{
    /// <summary> Сохраняет уровень в файл </summary>
    /// <param name="levelObjectsList"> Коллекция объектов уровня </param>
    public static void SaveInFile(LevelObjectsList levelObjectsList)
    {
        // получаем список всех файлов в папке с уровнями
        List<string> files = (from a in Directory.GetFiles("Assets/Levels/") select Path.GetFileName(a)).ToList();
        // определяем имя для уровня на основе уже имеющихся
        string levelName = Enumerable.Range(1, 1000).First(i => !files.Contains($"Level_{i}.json")).ToString();
        string fullPath = Path.Combine("Assets/Levels/", $"Level_{levelName}.json");
        using (StreamWriter file = File.CreateText(fullPath))
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            JsonSerializer serializer = JsonSerializer.Create(settings);
            serializer.Serialize(file, levelObjectsList.LevelObjects);
        }
    }
}