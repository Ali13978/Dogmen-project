/*
 * POLYGON DOG CAMERA CONTROLLER SCRIPT
 * DESCRIPTION: This script demonstrates the range of animations and Prefabs included in 
 * Polygon Dog which can be customized for the users preference. Please attach this to the
 * Dog.Prefab asset then customize the keys for the particular animations.
 * 
 * PLEASE NOTE: This script is intended for demonstration purposes and user customization or 
 * third party animation plugins will be required for further animation options
 */
using UnityEngine;
public class POLYGON_CameraController : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 20f;
    public float rotationSpeed = 20f;
    public float cameraRotationX = 15.0f;
    public float cameraPositionY = 1.8f;
    public float cameraPositionZ = 2.0f;
    float distance;
    Vector3 position;
    Vector3 newPosition;
    Quaternion rotation;
    Quaternion newRotation;
    Transform camera;
    // (Move to input script)
    float MouseX;
    void Start()
    {
        //camera start
        distance = transform.position.y - target.position.y;
        position = new Vector3(target.position.x, target.position.y + distance, target.position.z);
        rotation = Quaternion.Euler(new Vector3(45f, target.rotation.eulerAngles.y, 0f));
        // camera offset 
        camera = this.gameObject.transform.GetChild(0);
        camera.position = new Vector3(0f, cameraPositionY, cameraPositionZ);
        camera.rotation = Quaternion.Euler(cameraRotationX, 0f, 0f);
    }
    void Update()
    {
        MouseX += Input.GetAxis("Mouse X");
    }
    void LateUpdate()
    {
        if (target)
        {
            //camera follow
            newPosition = target.position;
            newPosition.y += (distance);
            newRotation = Quaternion.Euler(new Vector3(0f, target.rotation.eulerAngles.y + MouseX, 0f));
            position = Vector3.Lerp(position, newPosition, followSpeed * Time.deltaTime);
            rotation = Quaternion.Lerp(rotation, newRotation, rotationSpeed * Time.deltaTime);
            transform.position = position;
            transform.rotation = rotation;
        }
    }
}