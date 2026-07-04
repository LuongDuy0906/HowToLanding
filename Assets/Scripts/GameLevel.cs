using UnityEngine;
using UnityEngine.Rendering;

public class GameLevel : MonoBehaviour
{
    [SerializeField] private int levelNumber;
    [SerializeField] private Transform landerStartPositionTransform;
    [SerializeField] private Transform cameraStartTargetTransform;
    [SerializeField] private float zoomedOutOrthographicSize;

    public int GetLevelNumber()
    {
        return levelNumber;
    }

    public Vector3 GetLanderStartPosition()
    {
        return landerStartPositionTransform.position;
    }

    public Transform GetCameraTransformTarget()
    {
        return cameraStartTargetTransform;
    }

    public float GetZoomedOutOrthigraphicSize()
    {
        return zoomedOutOrthographicSize;
    }
}
