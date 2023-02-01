using UnityEngine;
using UnityEngine.EventSystems;

///<summary>
///Описывает поведение игровых объектов в редактоер уровней
///</summary>
public class LevelEditorObjectBehaviour : MonoBehaviour, IPointerDownHandler
{
    private GameObject cam;
    private LevelEditorController levelEditorController;

    public void Awake()
    {
        cam = GameObject.Find("Main Camera");
        levelEditorController = cam.GetComponent<LevelEditorController>();
    }

    /// <summary>
    /// Реализует перемещение объекта при его перетаскивании левой кнопкой мыши.
    /// </summary>
    public void OnMouseDrag()
    {
        if (Input.GetMouseButton(0))
        {
            transform.position = cam.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
    }

    /// <summary>
    /// Реализует выделение объекта при нажатии правой кнопкой мыши.
    /// </summary>
    /// <param name="eventData">Данные события нажатия мыши.</param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetMouseButton(1))
        {
            var targetObject = levelEditorController.targetObject;
            var child = transform.GetChild(0).gameObject;

            if (targetObject == gameObject)
            {
                levelEditorController.targetObject = null;
                child.SetActive(false);
            }
            else
            {
                targetObject?.transform.GetChild(0).gameObject.SetActive(false);
                levelEditorController.targetObject = gameObject;
                child.SetActive(true);
            }
        }

    }
}
