using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Shape : MonoBehaviour
{

    public static float speed = 1.0f;
    private float lastMovedDown = 0;
    private int spaceFromBottom = 0;
    private float keyDelay = 2f;
    private float timePassed = 0f;

    //Touch variables
    private int touchSensHor = 20;
    private int touchSensVer = 15;

    Vector2 previousPosition = Vector2.zero;
    Vector2 direction = Vector2.zero;

    bool moved = false;
    


    // Use this for initialization
    void Start()
    {
        if (!IsInGrid())
        {
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.gameOver);

            Invoke("OpenGameOverScene", 0.5f);

        }
    }


    void OpenGameOverScene()
    {
        Destroy(gameObject);
        SceneManager.LoadScene("GameOver");
    }

    public static void IncreaseSpeed(int level)
    {
        Shape.speed -= 0.005f * level;
    }

    // Update is called once per frame
    void Update()
    {
        CheckUserInput();
 
    }

    void CheckUserInput()
    {
        if(Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if(t.phase == TouchPhase.Began)
            {
                previousPosition = new Vector2(t.position.x, t.position.y);
            }
            else if(t.phase == TouchPhase.Moved)
            {
                Vector2 touchDelta = t.deltaPosition;
                direction = touchDelta.normalized;

                if(Mathf.Abs(t.position.x - previousPosition.x) >= touchSensHor && direction.x < 0 && t.deltaPosition.y > -10 && t.deltaPosition.y < 10)
                {
                    //---Move Left---
                    MoveLeft();
                    previousPosition = t.position;
                    moved = true;

                } else if(Mathf.Abs(t.position.x - previousPosition.x) >= touchSensHor && direction.x > 0 && t.deltaPosition.y > -10 && t.deltaPosition.y < 10)
                {
                    //---Move Right---
                    MoveRight();
                    previousPosition = t.position;
                    moved = true;
                }
                else if(Mathf.Abs(t.position.y - previousPosition.y) >= touchSensVer && direction.y < 0 && t.deltaPosition.x > -10 && t.deltaPosition.x < 10)
                {
                    //---Move Down---
                    MoveDown();
                    previousPosition = t.position;
                    moved = true;

                }
                else if(Mathf.Abs(t.position.y - previousPosition.y) >= touchSensVer && direction.y > 0 && t.deltaPosition.x > -10 && t.deltaPosition.x < 10)
                {
                    //---Save shape---
                    SaveShape();

                }
            } 
            else if(t.phase == TouchPhase.Ended)
            {
                if (!moved)
                {
                    Rotate();

                }
                moved = false;
            }
        }


        //move shape left
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLeft();
        }

        //Move shape Right
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveRight();
        }

        timePassed += Time.deltaTime;

        //move the shape down once
        if (Input.GetKeyDown(KeyCode.DownArrow) || Time.time - lastMovedDown >= Shape.speed)
        {
            MoveDown();

        }

        //rotate shape 90 degrees
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate();
        }

        //If Shift is pressed, save current shape
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            SaveShape();
        }
    }

    void MoveLeft()
    {
        transform.position += new Vector3(-1, 0, 0);

        if (!IsInGrid())
        {
            transform.position += new Vector3(1, 0, 0);
        }
        else
        {
            UpdateGameBoard();
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.shapeMove);
        }
    }

    void MoveRight()
    {
        transform.position += new Vector3(1, 0, 0);

        if (!IsInGrid())
        {
            transform.position += new Vector3(-1, 0, 0);
        }
        else
        {
            UpdateGameBoard();
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.shapeMove);
        }
    }

    void MoveDown()
    {
        transform.position += new Vector3(0, -1, 0);
        if (!IsInGrid())
        {
            transform.position += new Vector3(0, 1, 0);

            bool rowDeleted = GameBoard.DeleteFullRows();

            if (rowDeleted)
            {
                GameBoard.DeleteFullRows();
                IncreaseScore();


            }
            enabled = false;
            tag = "Untagged";

            FindObjectOfType<ShapeSpawner>().SpawnShape();

        }
        else
        {
            UpdateGameBoard();
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.shapeMove);

        }

        spaceFromBottom++;
        lastMovedDown = Time.time;

    }

    void Rotate()
    {
        transform.Rotate(0, 0, 90);

        if (!IsInGrid())
        {
            transform.position += new Vector3(0, 0, -90);
        }
        else
        {
            UpdateGameBoard();
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.rotateSound);
        }
    }




    public void SaveShape()
    {
        GameObject save = GameObject.FindGameObjectWithTag("CurrentActiveShape");
        FindObjectOfType<ShapeSpawner>().SaveShape(save.transform);

    }

    public bool IsInGrid()
    {
        foreach (Transform childBlock in transform)
        {
            Vector2 vect = RoundVector(childBlock.position);

            if (!IsInBorder(vect))
            {
                return false;
            }

            if (GameBoard.gameBoard[(int)vect.x-1, (int)vect.y-1] != null &&
                GameBoard.gameBoard[(int)vect.x-1, (int)vect.y-1].parent != transform)
            {
                return false;
            }
        }
        return true;
    }

    //checks if the object is in the specified border
    public static bool IsInBorder(Vector2 pos)
    {
        return ((int)pos.x >= 1 &&
            (int)pos.x <= 10 &&
            (int)pos.y >= 1);
    }

    public void UpdateGameBoard()
    {
        for (int y = 0; y < 20; ++y)
        {
            for (int x = 0; x < 10; ++x)
            {
                if (GameBoard.gameBoard[x, y] != null &&
                    GameBoard.gameBoard[x, y].parent == transform)
                {
                    GameBoard.gameBoard[x, y] = null;
                    Debug.Log("This is true");
                }
            }
        }

        foreach (Transform childBlock in transform)
        {
            Vector2 vect = RoundVector(childBlock.position);


            GameBoard.gameBoard[(int)vect.x-1, (int)vect.y-1] = childBlock;

            Debug.Log("Cube at:" + vect.x + " " + vect.y);


        }
    }

    public Vector2 RoundVector(Vector2 position)
    {
        return new Vector2(Mathf.Round(position.x), Mathf.Round(position.y));
    }

    public void IncreaseScore()
    {
        var textComp = GameObject.Find("Score").GetComponent<Text>();
        int score = int.Parse(textComp.text);

        score+=100;

        textComp.text = score.ToString();
    }
}

