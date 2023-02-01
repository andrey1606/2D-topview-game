using UnityEngine;
using TMPro;

///<summary>
///Считывает параметры процедурной генерации из окна ввода
///</summary>
public class ReadParamsProcGen : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputSize;
    [SerializeField] private TMP_InputField inputDifficulty;
    [SerializeField] private TextMeshProUGUI infoLabel;
    internal int levelSize, levelDifficulty;

    /// <summary> Устанавливает параметры для процедурной генерации.</summary>
    /// <returns> True если параметры введены корректно, false если нет.</returns>    
    internal bool SetParams()
    {
        if (int.TryParse(inputSize.text, out int size) && int.TryParse(inputDifficulty.text, out int difficulty))
            if (size >= 10 && size <= 200 && difficulty >= 1 && difficulty <= 5)
            {
                levelSize = size;
                levelDifficulty = difficulty;
                return true;
            }
        infoLabel.text = "Parameters are incorrect";
        return false;
    }
}