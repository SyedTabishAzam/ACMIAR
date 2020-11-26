using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileData
{
    //Contains simple properties of missile

    public string missileName = "DefaultMissile";
    public double WeightAtLaunch = 125;         //Lbs
    public double WeightAtBurnout = 50;         // Lbs
    public double Thrust = 690;                 //Lbs
    public double TimeOfMotorBurn = 8.0;        //seconds
    public double MaximumAcceleration = 30;     //G
    public double Range = 2.5;                  //Miles

    public override string ToString()
    {
        return "Weight At Launch : " + WeightAtLaunch.ToString() +
          " ,Weight At Burnout : " + WeightAtBurnout.ToString() +
          " ,Thrust : " + Thrust.ToString() +
          " ,Time Of Motor Burn : " + TimeOfMotorBurn.ToString() +
          " ,Range : " + Range.ToString();
    }
}

public class AircraftMisileData
{
    //To be used by aircraft - to know the properties of missiles and its count
    public MissileData installedMissile = new MissileData();

    //this struct only has an extra total missiles (this shouldn't be part of missile data)
    public int totalMissiles = 1;
}

public class MissileManager : MonoBehaviour
{

    private string _path;
    //Container for aircraft parent transform
    public GameObject _aircraftParent;
    public GameObject missilePrefab;
    public GameObject _resultTextObject;
    public GameObject _probabilityTextPrefab;
    public Transform _missileParent;

    List<movement> planeChildArr;
    Dictionary<string, AircraftMisileData> aircraftMissileMap;
    ProbabilisticCalculation _flyoutModel;

    // Use this for initialization
    void Start()
    {

        planeChildArr = new List<movement>();
        aircraftMissileMap = new Dictionary<string, AircraftMisileData>();
        _flyoutModel = GetComponent<ProbabilisticCalculation>();
        foreach (Transform child in _aircraftParent.transform)
        {
            movement Movement = child.gameObject.GetComponent<movement>();
            planeChildArr.Add(Movement);
        }

        _path = Application.dataPath + "\\Resources\\MissileData\\MissileData.csv";
        ExtractMissileData();
    }

    private void ExtractMissileData()
    {
        string fileData = System.IO.File.ReadAllText(_path);
        string[] lines = fileData.Split("\n"[0]);
        for (int i = 0; i < lines.Length; i++)
        {
            //Skip first line as we already know what to expect
            if (i == 0)
                continue;
            string[] lineData = (lines[i].Trim()).Split(',');


            AircraftMisileData aircraftMissileData = new AircraftMisileData();

            string aircraftType = lineData[0];
            int noOfMissiles = int.Parse(lineData[1]);
            aircraftMissileData.totalMissiles = noOfMissiles;

            MissileData missileData = new MissileData();
            missileData.WeightAtLaunch = double.Parse(lineData[2]);
            missileData.WeightAtBurnout = double.Parse(lineData[3]);
            missileData.Thrust = double.Parse(lineData[4]);
            missileData.TimeOfMotorBurn = double.Parse(lineData[5]);
            missileData.MaximumAcceleration = double.Parse(lineData[6]);
            missileData.Range = double.Parse(lineData[7]);

            //Save this missile data in aircraft missile data struct
            aircraftMissileData.installedMissile = missileData;

            //map aircraft type to aircraft missile data (avoid duplication)
            if (!aircraftMissileMap.ContainsKey(aircraftType))
                aircraftMissileMap.Add(aircraftType, aircraftMissileData);
            else
                Debug.LogError("Error in missile data ! Duplication of aircraft type found");

            Debug.Log(missileData.ToString());
        }

    }

    private void FixedUpdate()
    {

    }



    public void Launch(GameObject owner, string type)
    {

        //Feed missile data in paladin
        //feed owner position in paladin
        //feed other positions in paladin
        //Launch
        //if Hit, then launch missile to target
        //if Miss, then move to next target
        //On end, report miss

        MissileData missileData = GetMissileDataFromType(type);

        if (missileData != null)
        {
            bool isHit = false;

            int targetsInRange = 0;
            List<float> probabilities = new List<float>();
            List<GameObject> hitTargets = new List<GameObject>();

            foreach (Transform targetChild in _aircraftParent.transform)
            {
                if (targetChild.gameObject == owner)
                    continue;

                float hitProbability = 0;
                _flyoutModel.CalculateIsTargetHit(owner, targetChild.gameObject, missileData, ref hitProbability);
                if (hitProbability > 0f)
                {
                    targetsInRange++;
                    probabilities.Add(hitProbability);
                    hitTargets.Add(targetChild.gameObject);
                    isHit = true;
                }
                else
                {
                    Debug.Log("False not in range");
                }
            }

            if (isHit)
            {
                float max = 0f;
                GameObject hitTarget = null;
                for (int i = 0; i < targetsInRange; i++)
                {
                    float currentAircraftProbability = probabilities[i] * (1f / (float)targetsInRange);
                    if (currentAircraftProbability > max)
                    {
                        max = currentAircraftProbability;
                        hitTarget = hitTargets[i];
                        GameObject probabilityIndicator = Instantiate(_probabilityTextPrefab, hitTarget.transform);
                        probabilityIndicator.GetComponent<TextMesh>().text = "Kill " + currentAircraftProbability + "%";
                        Destroy(probabilityIndicator, (float)missileData.TimeOfMotorBurn);
                    }
                }
                LaunchMissile(missileData, owner, hitTarget);
            }


            if (!isHit)
            {
                LaunchMissile(missileData, owner, null);
                Debug.Log("Missile Miss");
            }

        }
    }


    private void LaunchMissile(MissileData missileData, GameObject sourceAircraft, GameObject destinationAircraft)
    {
        Transform parent = sourceAircraft.transform.GetChild(0);
        GameObject missile = Instantiate(missilePrefab, parent);
        missile.transform.SetParent(_missileParent);
        MissileMovement missieMovementScript = missile.GetComponent<MissileMovement>();

        if (destinationAircraft != null)
            missieMovementScript.SetTarget(destinationAircraft.transform.GetChild(0).gameObject);

        missieMovementScript.SetVelocityPerSecond(7500f);
        missieMovementScript.SetBurnTime((float)missileData.TimeOfMotorBurn);
        missieMovementScript.SetMinimumFocus(70f);
        missieMovementScript.SetLaunchingAircraft(sourceAircraft);
    }

    public void DisplayResultOnMap(bool isHit, GameObject targetChild)
    {
        _resultTextObject.SetActive(true);
        if (isHit)
        {
            _resultTextObject.GetComponent<TextMesh>().text = "Hit Success : " + targetChild.name;
            _resultTextObject.GetComponent<TextMesh>().color = Color.blue;
        }
        else
        {
            _resultTextObject.GetComponent<TextMesh>().text = "Missed target";
            _resultTextObject.GetComponent<TextMesh>().color = Color.red;
        }
        Invoke("StopDisplay", 2.0f);
    }

    public void StopDisplay()
    {
        _resultTextObject.SetActive(false);
    }

    private MissileData GetMissileDataFromType(string type)
    {
        if (aircraftMissileMap.ContainsKey(type))
        {
            return aircraftMissileMap[type].installedMissile;
        }
        return null;
    }

    private int GetTotalMissilesForType(string type)
    {
        if (aircraftMissileMap.ContainsKey(type))
        {
            return aircraftMissileMap[type].totalMissiles;
        }
        return 0;
    }
}
