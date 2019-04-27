using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostShape : MonoBehaviour
{


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
    }


    bool CheckIsValidPos()
    {
        foreach(Transform mino in transform)
        {
            Vector2 pos = FindObjectOfType<Shape>().RoundVector(mino.position);
            if (Shape.IsInBorder(pos) == false)
                return false;


        }
        return true;

    }

}
