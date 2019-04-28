using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeSpawner : MonoBehaviour
{

    public GameObject[] shapes;
    public GameObject[] nextShapes;
    public GameObject[] savedShapes;
    int last;
    bool saved = false;

    GameObject nextObject = null;

    int shapeIndex = 0;
    int nextShapeIndex = 0;


    private Vector2 savedShapePosition = new Vector2(-3, 17);
    private GameObject savedShape;
    private GameObject nextShape;
    private GameObject ghostShape;

    public int maxSwaps = 1;
    private int currentSwaps = 0;



    public void SpawnShape()
    {
       
        int shapeIndex = nextShapeIndex;

        Instantiate(shapes[shapeIndex],
            transform.position,
            Quaternion.identity);
        shapes[shapeIndex].tag = "CurrentActiveShape";

        nextShape = shapes[shapeIndex];

        nextShapeIndex = UnityEngine.Random.Range(0, 7);

        Vector3 nextShapePos = new Vector3(13, 16, 0);

        if (nextObject != null)
            Destroy(nextObject);

        if(nextShapeIndex == shapeIndex)
        {
            //---Makes sure no shape is spawned 2+ times in a row---
            //Debug.Log("Next shape is equal to current shape");
            while(nextShapeIndex == shapeIndex)
            {
                nextShapeIndex = UnityEngine.Random.Range(0, 7);
            }
        }

        nextObject = Instantiate(nextShapes[nextShapeIndex],
            nextShapePos,
            Quaternion.identity);
        nextShapes[nextShapeIndex].tag = "CurrentActiveShape";
        currentSwaps = 0;
        SpawnGhost();


    }

    public void SpawnSpecificShape(GameObject temp)
    {
        Instantiate(temp,
           transform.position,
           Quaternion.identity);
    }




    bool CheckIsValidPosition(GameObject shape)
    {
        foreach(Transform mino in shape.transform)
        {
            Vector2 pos = FindObjectOfType<Shape>().RoundVector(mino.position);
            
            if (!Shape.IsInBorder(pos))
                return false;
        }
        return true;
    }

    public void SaveShape(Transform s)
    {
        currentSwaps++;
        if(currentSwaps > maxSwaps)
        {
            return;
        }

        if(savedShape != null)
        {
            //---There is a saved shape---
            GameObject tempSaveShape = GameObject.FindGameObjectWithTag("CurrentSavedShape");

            tempSaveShape.transform.localPosition = new Vector2(10 / 2 ,20);

            savedShape = (GameObject)Instantiate(s.gameObject);
            savedShape.GetComponent<Shape>().enabled = false;
            savedShape.transform.localPosition = savedShapePosition;
            savedShape.tag = "CurrentSavedShape";

            

            nextShape = (GameObject)Instantiate(tempSaveShape);
            nextShape.GetComponent<Shape>().enabled = true;
            nextShape.transform.localPosition = new Vector2(10 / 2, 20);
            nextShape.tag = "CurrentActiveShape";


            DestroyImmediate(s.gameObject);
            DestroyImmediate(tempSaveShape);

            SpawnGhost();


        }
        else
        {

            //---no shape saved---
            savedShape = (GameObject)Instantiate(GameObject.FindGameObjectWithTag("CurrentActiveShape"));
            if (savedShape == null)
                Debug.Log("Didnt get");
            savedShape.GetComponent<Shape>().enabled = false;
            savedShape.transform.localPosition = savedShapePosition;
            savedShape.tag = "CurrentSavedShape";

            DestroyImmediate(GameObject.FindGameObjectWithTag("CurrentActiveShape"));


            SpawnShape();



        }
    }

    public void SpawnGhost()
    {

        if(GameObject.FindGameObjectWithTag("CurrentGhostShape")!= null)
            Destroy(GameObject.FindGameObjectWithTag("CurrentGhostShape"));

        ghostShape = (GameObject)Instantiate(nextShape, new Vector3(5f,10f,0), Quaternion.identity);

        Destroy(ghostShape.GetComponent<Shape>());
        ghostShape.AddComponent<GhostShape>();
    }

    public Transform CheckBottom(Vector2 pos)
    {
        if (pos.y > 20)
        {
            return null;
        }
        else
        {
            return GameBoard.gameBoard[(int)pos.x - 1, (int)pos.y - 1];
        }

    }
    // Use this for initialization
    void Start()
    {
        nextShapeIndex = UnityEngine.Random.Range(0, 7);

        SpawnShape();
    }


}
