using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float zoomSpeed = 2f;

    // Update is called once per frame
    void Update()
    {
        // Перемещение камеры
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(horizontalInput, verticalInput, 0) * moveSpeed * Time.deltaTime);

        // Приближение камеры
        float zoomInput = Input.GetAxis("Mouse ScrollWheel");
        transform.Translate(Vector3.forward * zoomInput * zoomSpeed, Space.Self);
    }
}
