

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeft : MonoBehaviour
{

    public float speed;
    private float leftBound = -13;

    void Start() 
    {
    }

    //background move left
    void Update()
    {
        // if (!GameManager.instance.isGameOver)
        // {
            transform.Translate(speed * Time.deltaTime * Vector3.left, Space.World);
        // }

        if (transform.position.x < leftBound && !gameObject.CompareTag("Background"))
        {
            Destroy(gameObject);
        }

    }
}

