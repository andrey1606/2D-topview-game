using UnityEngine;

///<summary>
///Управляет движением игрока
///</summary>
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D body;

    private void Awake() => body = GetComponent<Rigidbody2D>();

    // Обновление скорости игрока в зависимости от введенных данных о движении по осям
    private void Update()
    {
        var horizontal = Input.GetAxis("Horizontal") * speed;
        var vertical = Input.GetAxis("Vertical") * speed;
        body.velocity = new Vector2(horizontal, vertical);
    }
}
