using UnityEngine;
using UnityEngine.UI;
using TMPro;

///<summary>
///Представляет уровень в ScrollView списке уровней
///</summary>
public class LevelView
{
    public TextMeshProUGUI nameLevel;
    public Button statsButton;
    public Button startButton;
    public LevelView(Transform rootView)
    {
        nameLevel = rootView.Find("LevelName").GetComponent<TextMeshProUGUI>();
        statsButton = rootView.Find("StatsButton").GetComponent<Button>();
        startButton = rootView.Find("StartButton").GetComponent<Button>();
    }
}