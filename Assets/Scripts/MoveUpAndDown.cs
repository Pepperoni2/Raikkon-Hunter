using UnityEngine;

public class MoveUpAndDown : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    private bool movingUp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movingUp = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!movingUp)
        {
            transform.position += Vector3.down * Time.deltaTime * speed;
            if(transform.position.y <= minY)
            {
                movingUp = true;
            }
        }
        else
        {
            transform.position += Vector3.up * Time.deltaTime * speed;
            if(transform.position.y >= maxY)
            {
                maxY /= 3f;
                movingUp = false;
            }
        }
    }
}
