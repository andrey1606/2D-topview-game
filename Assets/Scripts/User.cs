///<summary>
///Пользователь и его данные
///</summary>
public class User
{
    public string Login { get; set; }
    public string Password { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsCurrent { get; set; }
    public User(string _login, string _password, bool _isAdmin, bool _isCurrent)
    {
        Login = _login;
        Password = _password;
        IsAdmin = _isAdmin;
        IsCurrent = _isCurrent;
    }
}