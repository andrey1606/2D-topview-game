using UnityEngine;

///<summary>
///Управление поведением противников
///</summary>
public class EnemyBehaviour : MonoBehaviour
{
    // следующие 6 полей отличаются у каждого типа противника и задаются в инспекторе
    // задает расстояние до игрока, начиная с которого противник будет его преследовать
    [SerializeField] private float angerRadius;
    [SerializeField] private float speed;
    [SerializeField] private float angrySpeed;
    // задает радиус, в пределах которого будет двигаться противник
    [SerializeField] private float radiusOfPatrol;
    [SerializeField] private float damage;
    // задержка между нанесением урона
    [SerializeField] private float damageDelay;
    // фиксирует время, в которое в последний раз был нанесен урон
    private float lastDamageTime = 0;
    private GameObject player;
    // точка, вокруг которой будет двигаться противник
    private Vector3 pointOfPatrol;
    // целевая точка противника
    private Vector3 targetPoint;
    // true, когда начинает преследовать игрока
    private bool isAngry = false;
    // true, когда касается игрока
    private bool isCollision = false;

    private void Awake()
    {
        // ищем игрока
        player = GameObject.Find("Player");
        // в качестве точки патрулирования задаем позицию противника на момент запуска уровня
        pointOfPatrol = gameObject.transform.position;
    }

    private void Update()
    {
        AngerCheck();
        Move();
    }

    // описывает логику движения противника
    private void Move()
    {
        // противник стоит на месте, если касается игрока
        if (isCollision) targetPoint = transform.position;
        if (transform.position != targetPoint && !isCollision)
        {
            if (isAngry) transform.position = Vector3.MoveTowards(transform.position, targetPoint, angrySpeed * Time.deltaTime);
            else transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);
        }
        if (transform.position == targetPoint && !isAngry) SetNewTargetPoint();
    }

    // создает новую целевую точку в пределах заданного радиуса от точки патрулирования
    private void SetNewTargetPoint()
    {
        Vector2 NewPoint = Random.insideUnitCircle * radiusOfPatrol;
        targetPoint = new Vector2(pointOfPatrol.x + NewPoint.x, pointOfPatrol.y + NewPoint.y);
    }

    // проверяет расстояние до игрока
    private void AngerCheck()
    {
        if (Vector2.Distance(transform.position, player.transform.position) < angerRadius)
        {
            isAngry = true;
            targetPoint = player.transform.position;
        }
        else isAngry = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player") isCollision = true;
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player") DealDamage();
        // если противник столкнулся с коллайдером, отличным от игрока, задаем новую целевую точку
        if (other.gameObject.tag != "Player" && isAngry == false) SetNewTargetPoint();
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player") isCollision = false;
    }

    // описывает логику нанесения урона
    private void DealDamage()
    {
        if (Time.time - lastDamageTime >= damageDelay)
        {
            lastDamageTime = Time.time;
            player.GetComponent<HealthController>().TakeDamage(damage);
        }
    }
}
