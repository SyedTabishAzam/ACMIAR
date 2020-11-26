using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data;
using MySql.Data.MySqlClient;
public class DBScriptSystemAd : MonoBehaviour {
    public GameObject ContentParrent;
    public InputField _inputCallSign;
    public GameObject _CallSignRow;
    public GameObject _callSignParrent;
    bool isConnectedToDatabase;
    MySqlConnection con;
  
    public string sql;
    public InputField AddData;
    public Dropdown PilotName;

    void Start()
    {
        isConnectedToDatabase = false;
        ConnectToDatabase();
        InitializeFields();
    }

    void InitializeFields()
    {
        PopulateFormationCallSign();
        
    }
    //Callsign
    public Transform CallrowIndex;
    public List<int> allRowIndex = new List<int>();
    public GameObject Callrow;
    void PopulateFormationCallSign()
    {
        MySqlDataReader temp = FetchQuery("select * from formationcallsign");
         List<string> options = new List<string>();
        List<string> idoptions = new List<string>();
        // Callrow.transform.localPosition = new Vector3(0, 0, 0);
        int index = 0;
        while (temp.Read())
        {
            _CallSignRow.transform.Find("Id").GetComponent<Text>().text = temp.GetString(temp.GetOrdinal("Id"));

            _CallSignRow.transform.Find("Name").GetComponent<Text>().text = temp.GetString(temp.GetOrdinal("CallSign"));
             Callrow = Instantiate(_CallSignRow, _callSignParrent.transform.GetChild(0).GetChild(0));
            Callrow.transform.localPosition += Vector3.down * (index * 27);
            index++;
        }
        temp.Close();

    }
     void DestroyAllRows()
    {
        foreach (Transform child in ContentParrent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
       

    }
    public void SaveCallSignToDB()
    {
       
        string CallSignAdd = _inputCallSign.text.ToString();
        var cmd = new MySqlCommand(sql, con);

       cmd.CommandText = "INSERT INTO formationcallsign(CallSign) VALUES('"+ CallSignAdd +"')";

        cmd.ExecuteNonQuery();
        DestroyAllRows();
        PopulateFormationCallSign();

    }
    public void ResetFormationCallsign()
    {
        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = "ALTER TABLE formationcallsign DROP Id";
        cmd.ExecuteNonQuery();
        cmd.CommandText = "ALTER TABLE formationcallsign ADD Id INT NOT NULL AUTO_INCREMENT FIRST, ADD PRIMARY KEY(Id), AUTO_INCREMENT = 1";
        cmd.ExecuteNonQuery();
    }


    private void OnApplicationQuit()
    {
        con.Close();
    }

    void ConnectToDatabase()
    {
        try
        {

            string cs = @"server=localhost;port=3308;userid=root;password=root;database=acmi";

            con = new MySqlConnection(cs);
            con.Open();

            isConnectedToDatabase = true;
        }
        catch (MySqlException e)
        {
            Debug.Log(e.Message);
            isConnectedToDatabase = false;

        }
    }

    MySqlDataReader FetchQuery(string sqlquery)
    {

        string sql = sqlquery;
        var cmd = new MySqlCommand(sql, con);

        MySqlDataReader rdr = cmd.ExecuteReader();

        return rdr;
    }
}


