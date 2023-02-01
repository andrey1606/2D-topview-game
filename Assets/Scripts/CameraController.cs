using UnityEngine;

///<summary>
///Управление камерой во время игры
///</summary>
public class CameraController : MonoBehaviour
{
    [SerializeField] private float speed;
    private Transform player;
    // целевая точка камеры
    private Vector3 Target;
    private void Awake()
    {
        // ищем игрока
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void Update()
    {
        // задаем в качестве целевой точки местоположение игрока
        Target = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        // и отправляем в эту точку камеру с заданной скоростью    
        transform.position = Vector3.Lerp(transform.position, Target, speed * Time.deltaTime);
    }
}
