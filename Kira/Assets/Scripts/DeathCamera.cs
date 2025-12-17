using UnityEngine;

public class DeathCamera : MonoBehaviour
{
    public Camera deadCamera;
    public float fovOutSpeed = 5f;

    // Update is called once per frame
    void Update()
    {
        deadCamera.fieldOfView -= Time.unscaledDeltaTime * fovOutSpeed;
    }
}
