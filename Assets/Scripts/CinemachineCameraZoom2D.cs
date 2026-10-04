using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using UnityEngine;

public class CinemachineCameraZoom2D : MonoBehaviour
{
    private const float NORMAL_ORTHOGRAPHIC_SIZE = 14f;

    public static CinemachineCameraZoom2D Instance { get; private set; }

    [SerializeField] private CinemachineCamera cinemachineCamera;

    private float targetOrthographicSize = NORMAL_ORTHOGRAPHIC_SIZE;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        float zoomSpeed = 2f;
        cinemachineCamera.Lens.OrthographicSize = 
            Mathf.Lerp(cinemachineCamera.Lens.OrthographicSize,targetOrthographicSize,Time.deltaTime * zoomSpeed);
    }

    public void SetOrthographicSize(float orthographicSize)
    {
        targetOrthographicSize = orthographicSize;
    }

    public void SetNormalOrthographicSize()
    {
        SetOrthographicSize(NORMAL_ORTHOGRAPHIC_SIZE);
    }
}