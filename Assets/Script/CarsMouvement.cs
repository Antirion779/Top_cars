using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarsMouvement : MonoBehaviour
{
    [Header("Component")] 
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private GameObject front;

    [Header("constant")] 
    [SerializeField] private float speed = 2.0f;

    [SerializeField] private float maxSpeed = 20.0f;
    [SerializeField] private float speedRotation = 2.0f;
    private Vector2 dir;

    // Update is called once per frame
    void FixedUpdate()
    {
        dir = front.transform.position - transform.position;
        //Debug.Log(rb.velocity.magnitude);

        if (rb.velocity.magnitude < maxSpeed)
        {
            if (Input.GetKey(KeyCode.Keypad8))//Up
            {
                rb.AddForce(dir * speed);
            }
            if (Input.GetKey(KeyCode.Keypad2))//Down
            {
                rb.AddForce(Vector2.up * -speed);
            }
        }
        

        if (Input.GetKey(KeyCode.Keypad4))//Left
        {
            transform.Rotate(new Vector3(0,0,1) * speedRotation * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Keypad6))//Right
        {
            transform.Rotate(new Vector3(0, 0, 1) * -speedRotation * Time.deltaTime);
        }

    }
}
