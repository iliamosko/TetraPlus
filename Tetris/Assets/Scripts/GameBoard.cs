using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour {

    public static Transform[,] gameBoard = new Transform[10, 20];

    private static int numRowsDel = 0;

    static int level = 1;
    static int lineClearsNeaded = 5;
    static int levelLineClearsNeaded = 5;

    

    public static int GetRowsDel()
    {
        return numRowsDel;
    }
    public static void SetToZero()
    {
        numRowsDel = 0;
    }


    public static void CheckNextLevel()
    {
        if(lineClearsNeaded == 0)
        {
            level++;
            levelLineClearsNeaded += 5;
            

            Shape.IncreaseSpeed(level);

            var textL = GameObject.Find("Level").GetComponent<Text>();
            textL.text = level.ToString();

            var textC = GameObject.Find("LinesToClear").GetComponent<Text>();
            textC.text = levelLineClearsNeaded.ToString();
            lineClearsNeaded = levelLineClearsNeaded;
            
        }

    }
    public static void LineCleared()
    {
        lineClearsNeaded--;
        var textC = GameObject.Find("LinesToClear").GetComponent<Text>();
        textC.text = lineClearsNeaded.ToString();
    }


    public static bool DeleteFullRows()
    {
        for(int r = 0; r < 20; r++)
        {
            if (IsRowFull(r))
            {
                DeleteRow(r);
                numRowsDel++;

                SoundManager.Instance.PlayOneShot(SoundManager.Instance.rowDelete);
                GameBoard.LineCleared();
                GameBoard.CheckNextLevel();

                
                Debug.Log(numRowsDel);
                return true;

            } 
        }
        return false;
    }

    public static bool IsRowFull(int row)
    {
        for(int c = 0; c < 10; ++c)
        {
            try 
            {
                //Debug.Log("rows: " + row + "Collumns" + c);
                if (gameBoard[c, row] == null)
                {
                    return false;
                }
            } 
            catch (System.IndexOutOfRangeException)
            {
                Debug.Log("rows: " + row + "Collumns" + c);
            }
           
        }
        return true;
    }

    public static void DeleteRow(int row)
    {
        for(int c = 0; c < 10; ++c)
        {
            try
            {
               // Debug.Log(" in Delete Row: rows: " + row + "Collumns" + c);
                Destroy(gameBoard[c, row].gameObject);
                gameBoard[c, row] = null;
            }
            catch (System.IndexOutOfRangeException)
            {
                Debug.Log("rows: " + row + "Collumns" + c);
            }

        }
        row++;

        Debug.Log("Deleted row" + row);

        for(int i = row; i < 20; ++i)
        {
            for(int c = 0; c < 10; ++c)
            {
                try
                {
                    if (gameBoard[c, i] != null)
                    {
                        gameBoard[c, i - 1] = gameBoard[c, i];

                        gameBoard[c, i] = null;

                        gameBoard[c, i - 1].position += new Vector3(0, -1, 0);
                    }
                }
                catch (System.IndexOutOfRangeException)
                {
                    Debug.Log("rows: " + row + "Collumns" + c);
                }
            }
        }
        Debug.Log("Shifted rows down");
    }
}
