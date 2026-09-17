using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float smoothTime = 0.3f;
    public float verticalDeadZone = 2f;

    // 맵의 실제 경계
    public float mapMinX = -10f;
    public float mapMaxX = 10f;
    public float mapMinY = -10f;
    public float mapMaxY = 10f;

    private Vector3 velocity = Vector3.zero;
    private Camera cam;
    private void Awake()
    {
        cam = GetComponent<Camera>();
    }


    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        // 카메라 화면의 절반 높이
        float cameraHalfHeight = cam.orthographicSize;

        // 카메라 화면의 절반 너비
        float cameraHalfWidth = cameraHalfHeight * cam.aspect;

        // 카메라 중심이 이동할 수 있는 실제 범위
        float minX = mapMinX + cameraHalfWidth;
        float maxX = mapMaxX - cameraHalfWidth;

        float minY = mapMinY + cameraHalfHeight;
        float maxY = mapMaxY - cameraHalfHeight;

        float targetY = transform.position.y;

        float verticalDistance = target.position.y - transform.position.y;

        if (verticalDistance > verticalDeadZone)
        {
            targetY = target.position.y - verticalDeadZone;
        }
        else if (verticalDistance < -verticalDeadZone)
        {
            targetY = target.position.y + verticalDeadZone;
        }

        // Player 위치를 카메라 목표 위치로 사용
        Vector3 targetPosition = new Vector3(
            target.position.x,
            targetY,
            transform.position.z
        );

        // 목표 위치가 맵 경계를 넘지 못하게 제한
        targetPosition.x = Mathf.Clamp(
            targetPosition.x,
            minX,
            maxX
        );

        targetPosition.y = Mathf.Clamp(
            targetPosition.y,
            minY,
            maxY
        );

        // 목표 위치까지 부드럽게 이동
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }
}