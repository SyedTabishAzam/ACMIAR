using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTerrain : MonoBehaviour {
    public float movementSpeed = 5f;
    public float rotateSpeed = 5f;
    public float scaleFactor = 0.5f;
    //0 true, 1 false, 2 another
    private int destroyInstructions = 0;
    public Transform pointT;
    public GameObject drawAreaManager;
    public GameObject instructionsText;
	public GameObject gameManager; 
    // Use this for initialization
    void Start() {


    }

    // Update is called once per frame
    void Update() {

        if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)  || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            drawAreaManager.GetComponent<ClippingPlane>().ChangeState("ISMOVING");
			TrailControl.crTrail = true;
            destroyInstructions = 2;
        }

        if (Input.GetKeyUp(KeyCode.KeypadMinus) || Input.GetKeyUp(KeyCode.KeypadPlus)||Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D) ||  Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S)  || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            drawAreaManager.GetComponent<ClippingPlane>().ChangeState("ISPLACED");
			TrailControl.crTrail = false;
            destroyInstructions = 0;
        }

    
        if (Input.GetKey(KeyCode.W))
        {
            MoveForward();
        }
        if (Input.GetKey(KeyCode.S))
        {
            MoveBack();
        }
        if (Input.GetKey(KeyCode.A))
        {
            MoveLeft();
        }
        if (Input.GetKey(KeyCode.D))
        {
            MoveRight();
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            MoveUp();
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            MoveDown();
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            RotateLeft();
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            RotateRight();
        }
        if (Input.GetKey(KeyCode.KeypadPlus) || Input.GetKey(KeyCode.Plus))
        {
            ScaleUp();
        }
        if (Input.GetKey(KeyCode.KeypadMinus) || Input.GetKey(KeyCode.Minus))
        {
            ScaleDown();
        }

        if (destroyInstructions==0)
        {
            FadeToDestroy();
        }
        if (destroyInstructions == 2)
        {
            Revive();
        }


    }

    private void FadeToDestroy()
    {
        Color temp = instructionsText.GetComponent<TextMesh>().color;
        instructionsText.GetComponent<TextMesh>().color = new Color(temp.r, temp.g, temp.b, temp.a-0.05f);
        if(temp.a<=0f)
        {
            instructionsText.GetComponent<TextMesh>().color = new Color(temp.r, temp.g, temp.b, 1f);
            instructionsText.SetActive(false);
            destroyInstructions = 1;
        }
    }

    private void Revive()
    {
        instructionsText.SetActive(true);
        Color temp = instructionsText.GetComponent<TextMesh>().color;
        instructionsText.GetComponent<TextMesh>().color = new Color(temp.r, temp.g, temp.b, 1f);
        destroyInstructions = 1;
    }


    public void ScaleUp()
    {
        transform.localScale += Vector3.one * scaleFactor;

    }

    public void ScaleDown()
    {
        transform.localScale -= Vector3.one * scaleFactor;
    }

    public void MoveForward()
    {
        transform.position += Vector3.forward * movementSpeed * (Time.deltaTime / Time.timeScale);
    }

    public void MoveBack()
    {
        transform.position += Vector3.back * movementSpeed * (Time.deltaTime / Time.timeScale);
    }

    public void MoveUp()
    {
        transform.position += Vector3.up * movementSpeed * (Time.deltaTime / Time.timeScale);
    }

    public void MoveDown()
    {
        transform.position += Vector3.down * movementSpeed * (Time.deltaTime / Time.timeScale);
    }

    public void MoveLeft()
    {
        transform.position += Vector3.left * movementSpeed * (Time.deltaTime / Time.timeScale);
    }

    public void MoveRight()
    {
        transform.position += Vector3.right * movementSpeed * (Time.deltaTime / Time.timeScale);
    }

    public void RotateLeft()
    {
        transform.RotateAround(pointT.position, Vector3.up, rotateSpeed * (Time.deltaTime / Time.timeScale));
    }

    public void RotateRight()
    {
        transform.RotateAround(pointT.position, Vector3.up, -rotateSpeed * (Time.deltaTime / Time.timeScale));
    }
}
