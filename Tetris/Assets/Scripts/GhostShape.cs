using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostShape : MonoBehaviour
{
    public Vector2 ghostShapePos; 

    // Start is called before the first frame update
    void Start()
    {
        tag = "CurrentGhostShape";

        foreach(Transform mino in transform)
        {
            mino.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .2f);

        }
    }

    // Update is called once per frame
    void Update()
    {
        FollowActiveShape();
    }

    public Vector2 GetPosition()
    {
        return this.ghostShapePos;
    }

    void FollowActiveShape()
    {
        Transform currentActiveShape = GameObject.FindGameObjectWithTag("CurrentActiveShape").transform;

        transform.position = currentActiveShape.position;
        transform.rotation = currentActiveShape.rotation;
        MoveDown();
    }


    void MoveDown()
    {
        while (CheckIsValidPos())
        {
            transform.position += new Vector3(0, -1, 0);
        }

        if (!CheckIsValidPos())
        {
            transform.position += new Vector3(0, 1, 0);
        }

        ghostShapePos = transform.position;
    }


    public bool CheckIsValidPos()
    {
        foreach(Transform mino in transform)
        {
            Vector2 pos = FindObjectOfType<Shape>().RoundVector(mino.position);
            if (Shape.IsInBorder(pos) == false)
                return false;
            if (FindObjectOfType<ShapeSpawner>().CheckBottom(pos) != null && FindObjectOfType<ShapeSpawner>().CheckBottom(pos).parent.tag == "CurrentActiveShape")
                return true;
            if (FindObjectOfType<ShapeSpawner>().CheckBottom(pos) != null && FindObjectOfType<ShapeSpawner>().CheckBottom(pos).parent != transform)
                return false;
        }
        return true;

    }

}
