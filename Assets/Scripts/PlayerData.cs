using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class PlayerData
{
    public List<List<float>> allData;
    public string TailId = "";
    public string TypeAC = "";
    public string CallSign = "";
    public string CallShort = "";
    public string Pilotname = "";
    public string Color = "";
    public bool taxiData = false;
}
