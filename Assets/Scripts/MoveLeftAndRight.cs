using System;
using Unity.VisualScripting;
using UnityEngine;

public class MoveLeftAndRight : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;

    private bool movingRight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Hello World!");
        movingRight = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!movingRight)
        {
            transform.position += new Vector3(-1, 0, 0) * Time.deltaTime * speed; // move left   
            if(transform.position.x <= minX)
            {
                movingRight = true;
            }
        }
        else
        {
            transform.position += new Vector3(1, 0, 0) * Time.deltaTime * speed; // move right
            if(transform.position.x >= maxX)
            {
                movingRight = false;
            }    
        }
    }
}
