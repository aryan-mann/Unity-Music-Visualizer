using UnityEngine;
using System.Collections;

public class QuickMove : MonoBehaviour {

    [Header("Camera Settings")]
    public Camera Cam;
    public Vector3 PositionalOffset;
    public Vector3 RotationalOffset;
    public float Distance;

    void Start() {
        Cam.transform.parent = transform;
    }

    void Update() {
        MoveCamera();
    }

    private void MoveCamera() {
        float xVal = Cam.transform.position.x - transform.position.x;
        float zVal = Cam.transform.position.z - transform.position.z;

        float angle = transform.rotation.eulerAngles.y;

        xVal = -Distance * Mathf.Cos(Mathf.Deg2Rad * angle);
        zVal = Distance * Mathf.Sin(Mathf.Deg2Rad * angle);

        Cam.transform.position = transform.position + PositionalOffset + new Vector3(xVal, 0, zVal);
        Cam.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + RotationalOffset);
    }
    
}