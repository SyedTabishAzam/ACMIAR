using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class SpriteTextFactory : NetworkBehaviour {

    public string textToDisplay = "";

    int rows;
    int cols;
    float cellWidth;
    float cellHeight;
    float spriteWidth;
    float spriteHeight;
    bool inverseTop = true;
    bool init = false;
    Sprite spriteSheet;
    Texture2D texture;
    bool doCombine = false;
    Sprite sprite;
    List<Texture2D> charList;
    float offSet = 0; //0.0046f

    [SyncVar]
    public Transform parentObject;

    [ClientRpc]
    public void RpcSpawnWithParams(string parentName, Vector3 position)
    {
        parentObject = GameObject.Find(parentName).transform;
        transform.position = position;
    }

    void Update()
    {
        if (transform.parent != parentObject)
            transform.SetParent(parentObject);
    }

    // Use this for initialization
    void Start () {
        textToDisplay = "Empty";
        Init();
      
    }

   public void ChangeText(string text)
    {
        if (!init)
            Init();
        textToDisplay = text;
        charList.Clear();
        foreach (char c in textToDisplay)
        {
            charList.Add(GetChar(c));
            doCombine = true;
        }
    }

    private void Init()
    {
        init = true;
        spriteSheet = Resources.Load<Sprite>("Sprites\\" + "font") as Sprite;
        charList = new List<Texture2D>();
        spriteHeight = spriteSheet.rect.yMax;
        spriteWidth = spriteSheet.rect.xMax;
        rows = 10;
        cols = 10;
        cellWidth = spriteWidth / cols;
        cellHeight = spriteHeight / rows;
    }

    public void DrawText()
    {
        
        if(doCombine)
            CombiineTexture(charList);
    }
	
    void CombiineTexture(List<Texture2D> charList)
    {
        texture = new Texture2D((int)cellWidth*charList.Count, (int)cellHeight);
        sprite = Sprite.Create(texture, new Rect(0, 0, cellWidth * charList.Count, cellHeight), Vector2.zero);
        

        int currentIndex = 0;
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++) //Goes through each pixel
            {
                Color pixelColour;
                currentIndex = x / (int)cellWidth;
                pixelColour = charList[currentIndex].GetPixel(x - (currentIndex*(int)cellWidth) , y);
                texture.SetPixel(x, y, pixelColour);
            }
        
        }

         GetComponent<SpriteRenderer>().sprite = sprite;
         texture.Apply();
        transform.localPosition += Vector3.left * charList.Count * offSet;
    }
    // Update is called once per frame
  
    Texture2D GetChar(char ch)
    {
        int ascii = ch;
        int firstCharAscii = 32;
        int cellNumber = ascii - firstCharAscii;
     

        int colNumber = cellNumber % rows;
        int rowNumber = cellNumber / rows;
        //row is 7
        //col = 3

        float Left = cellWidth * colNumber;
        float Top = cellHeight * (rowNumber + 1);
        //left = 61 * 7 = 427
        //top = 61 * 3=  183

        if (inverseTop)
            Top = spriteHeight - Top;
        //top is 427

       

        Texture2D texture = new Texture2D((int)cellWidth, (int)cellHeight);
        //Sprite sprite = Sprite.Create(texture, new Rect(0, 0, cellWidth, cellHeight), Vector2.zero);
      
        
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++) //Goes through each pixel
            {
                Color pixelColour;
                pixelColour = spriteSheet.texture.GetPixel(x + (int)Left, y+(int)Top);
                texture.SetPixel(x, y, pixelColour);
            }
        }

       // GetComponent<SpriteRenderer>().sprite = sprite;
      //  texture.Apply();
        return texture;
    }
}
