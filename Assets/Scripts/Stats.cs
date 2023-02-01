///<summary>
///Хранит статистику успешного прохождения конкретного уровня конкретным пользователем
///</summary>
public class Stats
{
    internal string login;
    internal string levelName;
    internal float score;
    internal float totalTime;
    public Stats(string _login, string _levelname, float _score, float _totalTime)
    {
        login = _login;
        levelName = _levelname;
        score = _score;
        totalTime = _totalTime;
    }
}