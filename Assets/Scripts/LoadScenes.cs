using UnityEngine;
using UnityEngine.SceneManagement;

///<summary>
///Предоставляет набор методов для загрузки сцен
///</summary>
public class LoadScenes : MonoBehaviour
{
    public static void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    public static void OpenMenu()
    {
        StatsManagement.SaveStats();
        SceneManager.LoadScene("Menu");
    }

    public static void StartLevel()
    {
        SceneManager.LoadScene("Level");
    }

    public static void LaunchLevelEditor()
    {
        foreach (var c in GameObject.Find("Main Camera").GetComponent<AuthorizationManagement>().UList.Users)
        {
            if (c.IsCurrent == true && c.IsAdmin == true)
                SceneManager.LoadScene("LevelEditor");
        }
    }
}
