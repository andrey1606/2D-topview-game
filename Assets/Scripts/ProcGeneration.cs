using System;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

///<summary>
///Отвечает за процедурную генерацию
///</summary>
public class ProcGeneration
{
    private int levelSize;
    private int levelDifficulty;
    private LevelObjectsList levelObjectsList = new LevelObjectsList();

    public ProcGeneration(int size, int difficulty)
    {
        levelSize = size;
        levelDifficulty = difficulty;
    }

    // генерирует границы уровня
    private void AddWalls()
    {
        // ширина одной стены, из которых создаются границы
        float wallWidth = 2.56f;
        Vector3 wallPos = Vector3.zero;
        Quaternion wallRot = Quaternion.identity;
        Quaternion rotZ90 = Quaternion.AngleAxis(90, Vector3.forward);
        Vector3[] wallDirs = { Vector3.right, Vector3.up, -Vector3.right, -Vector3.up };

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < levelSize; j++)
            {
                levelObjectsList.LevelObjects.Add(new LevelObject(wallPos, wallRot, "EdgeWall"));
                if (j != levelSize - 1)
                    wallPos += (i < 2) ? wallDirs[i % 2] * wallWidth : wallDirs[i % 2 + 2] * wallWidth;
            }
            wallRot *= rotZ90;
        }
    }

    // добавляет брёвна со случайным поворотом
    private void AddLogs()
    {
        int logCount = levelSize / 4;
        for (int i = 0; i < logCount; i++)
        {
            var logRotation = Quaternion.AngleAxis(Random.Range(0, 90), Vector3.forward);
            levelObjectsList.LevelObjects.Add(new LevelObject(GenRandomPoint(false), logRotation, "log"));
        }
    }

    private void AddDressingParts()
    {
        int dressingPartsCount = 3;
        switch (levelDifficulty)
        {
            case 1:
                dressingPartsCount = (int)(levelSize * 0.6);
                break;
            case 2:
                dressingPartsCount = (int)(levelSize * 0.45);
                break;
            case 3:
                dressingPartsCount = (int)(levelSize * 0.3);
                break;
            case 4:
                dressingPartsCount = (int)(levelSize * 0.225);
                break;
            case 5:
                dressingPartsCount = (int)(levelSize * 0.15);
                break;
        }
        if (dressingPartsCount < 3)
            dressingPartsCount = 3;
        for (int i = 0; i < dressingPartsCount; i++)
            levelObjectsList.LevelObjects.Add(new LevelObject(GenRandomPoint(false), Quaternion.identity, "dressingPart"));
    }

    private void AddResources()
    {
        int resourcesCount = levelSize / 5;
        if (resourcesCount < 3)
            resourcesCount = 3;
        int poisonousResourcesCount = (int)Math.Ceiling(resourcesCount * levelDifficulty * 0.3);
        for (int i = 0; i < resourcesCount; i++)
        {
            levelObjectsList.LevelObjects.Add(new LevelObject(GenRandomPoint(false), Quaternion.identity, "Mushrooms"));
            levelObjectsList.LevelObjects.Add(new LevelObject(GenRandomPoint(false), Quaternion.identity, "Berries"));
        }
        for (int i = 0; i < poisonousResourcesCount; i++)
        {
            levelObjectsList.LevelObjects.Add(new LevelObject(GenRandomPoint(false), Quaternion.identity, "MushroomsPoisonous"));
            levelObjectsList.LevelObjects.Add(new LevelObject(GenRandomPoint(false), Quaternion.identity, "BerriesPoisonous"));
        }
    }

    private void AddEnemies()
    {
        int wolfCount = (int)(levelSize / 10 * new System.Random().Next(0, levelDifficulty));
        int bearCount = (int)(levelSize / 10 * levelDifficulty) - wolfCount;
        for (int i = 0; i < wolfCount; i++)
            levelObjectsList.LevelObjects.Add(new LevelObject(GenRandomPoint(true), Quaternion.identity, "Wolf"));
        for (int i = 0; i < bearCount; i++)
            levelObjectsList.LevelObjects.Add(new LevelObject(GenRandomPoint(true), Quaternion.identity, "Bear"));
    }

    /// <summary> Генерирует уровень. </summary>
    /// <returns> Возвращает все объекты, которые были сгенерированы. </returns>
    internal LevelObjectsList Generation()
    {
        AddWalls();
        AddLogs();
        AddDressingParts();
        AddResources();
        levelObjectsList.LevelObjects.Add(new LevelObject(GenRandomPoint(false), Quaternion.identity, "Player"));
        levelObjectsList.LevelObjects.Add(new LevelObject(GenRandomPoint(false), Quaternion.identity, "Finish"));
        return levelObjectsList;
    }

    // генерирует случайную точку внутри границ уровня
    // учитывает, что возвращаемая точка должна быть на расстоянии от других объектов
    private Vector3 GenRandomPoint(bool isEnemy)
    {
        float minDistance = isEnemy ? 5f : 3f;
        Vector3 point;
        do
        {
            point = new Vector3(
                Random.Range(2f, (float)(levelSize * 2.56 - 2)),
                Random.Range(2f, (float)(levelSize * 2.56 - 2)),
                0
            );
        } while (levelObjectsList.LevelObjects.Any(c => Vector2.Distance(c.pos, point) < minDistance));
        return point;
    }
}
