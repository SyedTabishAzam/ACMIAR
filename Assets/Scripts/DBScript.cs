using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data;
using MySql.Data.MySqlClient;
using UnityEngine.SceneManagement;

public class DBScript : MonoBehaviour {


    bool isConnectedToDatabase;
    MySqlConnection con;
    //For System Admin
    
        //
	public string sql;
	public InputField AddData;
    public Dropdown PilotName;
    public Dropdown CallsignDropDown;
    public Dropdown RoleDropDown;
	public Dropdown MissionPhaseDropdown;
	public Dropdown MissiontypeDropdown;

	//
	//public string callsignselectedtext;
	public InputField missionname;
	public InputField missiondate;
	public InputField missiondescription;


    // Use this for initialization
    void Start () {

        isConnectedToDatabase = false;
        ConnectToDatabase();
        InitializeFields();

	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void Awake()
    {
        DontDestroyOnLoad(this);
    }
    void InitializeFields()
    {
        PopulateFormationCallSignDropdown();
		PopulatemissionphaseDropdown ();
		PopulatemissiontypeDropdown ();
    }


	void PopulatemissionphaseDropdown()
	{
		MySqlDataReader temp = FetchQuery("select * from mission_phase");
		List<string> options = new List<string>();
		while (temp.Read())
		{
				options.Add(temp.GetString(temp.GetOrdinal("PHASE_NAME")));
		}

		temp.Close();
		MissionPhaseDropdown.AddOptions(options);
	}

	void PopulatemissiontypeDropdown()
	{
		MySqlDataReader temp = FetchQuery("select * from mission_type");
		List<string> options = new List<string>();
		while (temp.Read())
		{
			options.Add(temp.GetString(temp.GetOrdinal("TYPE_NAME")));
		}

		temp.Close();
		MissiontypeDropdown.AddOptions(options);
	}

    void PopulateFormationCallSignDropdown()
    {
        MySqlDataReader temp = FetchQuery("select * from formationcallsign");

        List<string> options = new List<string>();
        while (temp.Read())
        {
            options.Add(temp.GetString(temp.GetOrdinal("CallSign")));

        }
        temp.Close();
        CallsignDropDown.AddOptions(options);
       

    }
     void Reset()
    {
        Scene _scene = SceneManager.GetActiveScene();
       
    }
    //public void SaveCallSignToDB()
    // {
    //     //string CallSignAdd = FormationCallSign.text.ToString();
    //    Debug.Log("saving call sign to db  testing " + CallSignAdd);
    //     var cmd = new MySqlCommand(sql, con);
    //     cmd.CommandText = "INSERT INTO formation(FORMATION_NAME) VALUES('" + CallSignAdd + "')";
    //     cmd.ExecuteNonQuery();

    // }

    //SendData in mysql database #Sabs

    //Update Formation
    public void SendDatainMySql()
	{
		//var cmd = new MySqlCommand(sql, con);
        MySqlCommand cmd = new MySqlCommand(sql, con);

        cmd.CommandText = "INSERT INTO formation(FORMATION_ID,FORMATION_NAME) VALUES('10','sabbar10')";
		cmd.ExecuteNonQuery ();
	}

	//Update Mission
	public void SaveMissionDataToDB()
	{
		//CallsignDropDown.captionText = callsignselectedtext;
		string missioname = missionname.text.ToString();
		string missiodate = missiondate.text.ToString ();
		string missiodescription = missiondescription.text.ToString ();
		
		//var cmd = new MySqlCommand(sql, con);
        MySqlCommand cmd = new MySqlCommand(sql, con);

        cmd.CommandText = "INSERT INTO mission(MISSION_NAME,DESCRIPTION,MISSION_DATE) VALUES('" + missioname + "','" + missiodate + "','" + missiodescription +"')";

		cmd.ExecuteNonQuery ();
	}


    private void OnApplicationQuit()
    {
        con.Close();
    }

    void ConnectToDatabase()
    {
        try
        {

            string cs = @"server=localhost;port=3306;userid=root;password=;database=acmi";

            con = new MySqlConnection(cs);
            //con.OpenAsync();
            con.Open();
            
            Debug.Log("Connected To Database Succesfully!");
            isConnectedToDatabase = true;
        }
        catch(MySqlException e)
        {
            Debug.Log("Failed To Connect With Database!");
            Debug.Log("EXception Message : " + e.Message);
            isConnectedToDatabase = false;
            
        }
    }

    MySqlDataReader FetchQuery(string sqlquery)
    {
        string sql = sqlquery;
        //var cmd = new MySqlCommand(sql, con);
        MySqlCommand cmd = new MySqlCommand(sql, con);

        MySqlDataReader rdr = cmd.ExecuteReader();
        
        return rdr;
    }
}
