using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
using UnityEngine;
using UnityEngine.UI;

///<summary>
///Управление авторизацией
///</summary>
public class AuthorizationManagement : MonoBehaviour
{
    // список пользователей
    internal UsersList UList;
    // элементы формы и сама форма
    [SerializeField] private TextMeshProUGUI LoginGUI;
    [SerializeField] private TextMeshProUGUI PasswordGUI;
    [SerializeField] private TextMeshProUGUI ErrorGUI;
    [SerializeField] internal TextMeshProUGUI CurrentUserGUI;
    [SerializeField] private Toggle isRememberToggle;
    [SerializeField] private Toggle isAdminToggle;
    [SerializeField] private GameObject AuthorizationForm;

    // инициализация формы и ее элементов
    private void AuthFormInit()
    {
        AuthorizationForm.SetActive(true);
        ErrorGUI.gameObject.SetActive(false);
        LoginGUI.text = "";
        PasswordGUI.text = "";
        CurrentUserGUI.text = "";
    }

    private void Awake()
    {
        AuthFormInit();
        UList = new UsersList();
        if (!Directory.Exists("Assets/JsonFiles/"))
            Directory.CreateDirectory("Assets/JsonFiles/");
        if (File.Exists("Assets/JsonFiles/users.json"))
        {
            // считываем файл 
            using (FileStream fstream = File.OpenRead("Assets/JsonFiles/users.json"))
            {
                byte[] array = new byte[fstream.Length];
                fstream.Read(array, 0, array.Length);
                string file = System.Text.Encoding.UTF8.GetString(array);
                var settings = new JsonSerializerSettings
                { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented };
                UList.Users = JsonConvert.DeserializeObject<List<User>>(file, settings);
            }

            // перебираем список пользователей и ищем того, который нажал "запомнить" при прошлом запуске игры
            foreach (var c in UList.Users)
            {
                if (c.IsCurrent == true)
                {
                    CurrentUserGUI.text = c.Login;
                    CurrentUser.User = c.Login;
                    AuthorizationForm.SetActive(false);
                }
            }
        }
    }

    ///<summary>
    ///Добавить нового пользователя
    ///</summary>
    public void AddUser()
    {
        if (LoginGUI.text != null && PasswordGUI.text != null)
        {
            UList.Users.Add(new User(LoginGUI.text, PasswordGUI.text, isAdminToggle.isOn, false));
            UpdateFile();
            ErrorGUI.gameObject.SetActive(true);
            ErrorGUI.text = "User has been added!";
        }
    }

    // обновляет файл с пользователями
    private void UpdateFile()
    {
        using (StreamWriter file = File.CreateText("Assets/JsonFiles/users.json"))
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented };
            JsonSerializer serializer = JsonSerializer.Create(settings);
            serializer.Serialize(file, UList.Users);
        }
    }

    ///<summary>
    ///Вход в игру
    ///</summary>
    public void LogIn()
    {
        // проверяем ввод
        if (LoginGUI.text != null && PasswordGUI.text != null)
        {
            if (UList.Users.Count != 0)
            {
                // перебираем список пользователей и ищем совпадение по логину и паролю
                foreach (var c in UList.Users)
                {
                    if (c.Login == LoginGUI.text && c.Password == PasswordGUI.text)
                    {
                        c.IsCurrent = isRememberToggle.isOn;
                        CurrentUserGUI.text = c.Login;
                        UpdateFile();
                        AuthorizationForm.SetActive(false);
                        break;
                    }
                }
                // если не нашли совпадения, выводим сообщение об ошибке
                ErrorGUI.gameObject.SetActive(true);
                ErrorGUI.text = "Error: login or password is incorrect";
            }
        }
    }

    ///<summary>
    ///Выход из системы
    ///</summary>
    public void LogOut()
    {
        foreach (var c in UList.Users)
        {
            if (c.Login == CurrentUserGUI.text)
            {
                c.IsCurrent = false;
                UpdateFile();
                AuthFormInit();
                break;
            }
        }
    }
}