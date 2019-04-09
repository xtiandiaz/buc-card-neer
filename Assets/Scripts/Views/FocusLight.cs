using UnityEngine;

public class FocusLight : MonoBehaviour
{
    [SerializeField] private Light light;
    [SerializeField] private new BoardCamera camera;

    private void Awake()
    {
        var cameraDepth = Mathf.Abs(camera.transform.position.z);

        light.range = cameraDepth * 2.5f;
        transform.localPosition = new Vector3(0 , 0, cameraDepth * 0.75f);
    }
}
