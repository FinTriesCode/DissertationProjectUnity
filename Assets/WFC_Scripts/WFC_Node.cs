using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WFC_Node", menuName = "WFC/Node")]
[System.Serializable]
public class WFC_Node : ScriptableObject
{
    public string _name;

    public GameObject _prefab;

    public WFC_Connection _topConnection;
    public WFC_Connection _bottomConnection;
    public WFC_Connection _leftConnection;
    public WFC_Connection _rightConnection;
}

[System.Serializable]
public class WFC_Connection
{
    public List<WFC_Node> _compatibleNodes = new();
}
