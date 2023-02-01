using UnityEngine;

///<summary>
///Хранит основную информацию об игровом объекте
///</summary>
public class LevelObject
{
    public Vector3 pos;
    public Quaternion rotation;
    public string tag;
    public LevelObject(Vector3 _pos, Quaternion _rotation, string _tag)
    {
        pos = _pos;
        rotation = _rotation;
        tag = _tag;
    }
}