using UnityEngine;
using UnityEngine.EventSystems;

///<summary>
///Описывает взаимодействие с ресурсами
///</summary>
public class ResourceController : MonoBehaviour
{
    // хранит текущий остаток ресурса
    [SerializeField] internal byte currentStock;
    // хранит начальный запас ресурса
    [SerializeField] private byte initialStock;
    // время добычи ресурса
    [SerializeField] internal float extractionTime;
    // подсчет времени с начала добычи
    internal float currentExtractionTime = 0f;
    // идет ли добыча
    internal bool isExtraction = false;

    private void Awake() => currentStock = initialStock;

    private void Update()
    {
        if (isExtraction)
        {
            currentExtractionTime += Time.deltaTime;
            if (currentExtractionTime >= extractionTime)
            {
                StopExtraction();
                currentStock--;
                // ресурс удаляется, когда его запас заканчивается
                if (currentStock <= 0)
                    Destroy(gameObject);
            }
        }
    }

    internal void StartExtraction() => isExtraction = true;

    internal void StopExtraction()
    {
        isExtraction = false;
        currentExtractionTime = 0f;
    }
}
