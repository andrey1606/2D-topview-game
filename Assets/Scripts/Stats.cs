///<summary>
///Хранит статистику успешного прохождения конкретного уровня конкретным пользователем
///</summary>
public class Stats
{
    public string login;
    public string levelName;
    public float score;
    public float totalTime;
    public Stats(string _login, string _levelname, float _score, float _totalTime)
    {
        login = _login;
        levelName = _levelname;
        score = _score;
        totalTime = _totalTime;
    }
}