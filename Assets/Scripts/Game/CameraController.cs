using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    public float scrollSpeed = 2000f;
    private float cos;
    private float sin;
    private Vector3 touchStart;

    void Awake()
    {
        Input.multiTouchEnabled = false;
    }

    void Start()
    {
        float angle = Mathf.PI * transform.eulerAngles.x / 180;
        cos = Mathf.Cos(angle);
        sin = Mathf.Sin(angle);
    }
    void Update()
    {
        MoveCamera();
        PanAndZoomCamera();
    }

    void MoveCamera()
    {
        Vector3 pos = transform.position;

        if (Input.GetKey("a") /*|| Input.mousePosition.x <= panBorderThickness*/)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d") /*|| Input.mousePosition.x >= Screen.width - panBorderThickness*/)
        {
            pos.x += panSpeed * Time.deltaTime;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= sin * scroll * scrollSpeed * Time.deltaTime;
        pos.z += cos * scroll * scrollSpeed * Time.deltaTime;

        pos.x = Mathf.Clamp(pos.x, -30, 30);
        pos.y = Mathf.Clamp(pos.y, 10, 60);
        pos.z = Mathf.Clamp(pos.z, -60, -10);

        transform.position = pos;
    }

    public void PanAndZoomCamera()
    {
        Vector3 pos = transform.position;
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = GetWorldPosition(0);
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 direction = touchStart - GetWorldPosition(0);


            if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y) && Mathf.Abs(direction.y) > 5)
            {
                pos.y -= sin * direction.y * 0.01f;
                pos.z += cos * direction.y * 0.01f;

                pos.y = Mathf.Clamp(pos.y, 10, 60);
                pos.z = Mathf.Clamp(pos.z, -60, -10);
            }
            else
            {
                pos.x += Mathf.Clamp(direction.x, -10, 10);

                pos.x = Mathf.Clamp(pos.x, -30, 30);
            }
        }
        transform.position = pos;
    }


    private Vector3 GetWorldPosition(float z)
    {
        Ray mousePos = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        ground.Raycast(mousePos, out distance);
        return mousePos.GetPoint(distance);
    }
}
