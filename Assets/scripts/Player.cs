using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody rBody;
    private Vector3 velocity;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * 10;
    }

    void FixedUpdate()
    {
        rBody.MovePosition(rBody.position + velocity * Time.fixedDeltaTime);
    }
}