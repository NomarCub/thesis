using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float speed = 20f;
    private float mouseXInput;
    private float mouseYInput;
    private float horizontalInput;
    private float verticalInput;
    private float horizontalRotation;
    private float verticalRotation;
    private float yPosition;

    private void Awake()
    {
        yPosition = transform.position.y;
    }

    private void Update()
    {
        GetInput();
        Rotate();
        Translate();
    }

    private void GetInput()
    {
        mouseXInput = Input.GetAxis("Mouse X");
        mouseYInput = Input.GetAxis("Mouse Y");

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    private void Rotate()
    {
        horizontalRotation += speed * mouseXInput * Time.deltaTime;
        verticalRotation -= speed * mouseYInput * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        transform.eulerAngles = new Vector3(verticalRotation, horizontalRotation, 0f);
    }

    private void Translate()
    {
        transform.Translate(horizontalInput * speed * Time.deltaTime, 0f, verticalInput * speed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, yPosition, transform.position.z);
    }
}
