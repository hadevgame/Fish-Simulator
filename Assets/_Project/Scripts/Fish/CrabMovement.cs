using UnityEngine;

public class CrabMovement : MonoBehaviour
{
    public float moveSpeed = 0.5f;
    public float changeDirInterval = 5f;
    public float raycastHeight = 0.3f;
    public float groundOffset = 0.02f;
    public float padding = 0.1f;
    public BoxCollider tankBoundsCollider;

    private Vector3 moveDir;
    private float timer;
    private bool isMovingToTank = false;

    void Start()
    {
        PickRandomDirection();
    }

    void Update()
    {
        if (isMovingToTank) return;

        timer += Time.deltaTime;
        if (timer >= changeDirInterval)
        {
            PickRandomDirection();
            timer = 0f;
        }

        Vector3 pos = transform.position;
        Vector3 rayOrigin = pos + Vector3.up * raycastHeight;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, raycastHeight * 2f))
        {
            if (hit.collider.CompareTag(GameConstants.GROUNDINTANK))
            {
                pos.y = hit.point.y + groundOffset;
            }
        }

        Vector3 moveXZ = moveDir * moveSpeed * Time.deltaTime;
        Vector3 nextPos = pos + new Vector3(moveXZ.x, 0f, moveXZ.z);

        if (tankBoundsCollider != null)
        {
            Vector3 center = tankBoundsCollider.center;
            Vector3 size = tankBoundsCollider.size;

            Vector3 localMin = center - size * 0.5f;
            Vector3 localMax = center + size * 0.5f;

            Vector3 localPos = tankBoundsCollider.transform.InverseTransformPoint(nextPos);
            localPos.x = Mathf.Clamp(localPos.x, localMin.x + padding, localMax.x - padding);
            localPos.z = Mathf.Clamp(localPos.z, localMin.z + padding, localMax.z - padding);

            nextPos = tankBoundsCollider.transform.TransformPoint(localPos);
        }

        transform.position = nextPos;

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }
    }

    void PickRandomDirection()
    {
        Vector2 rand = Random.insideUnitCircle.normalized;
        moveDir = new Vector3(rand.x, 0f, rand.y);
    }
    public void SetMovingToTank(bool moving)
    {
        isMovingToTank = moving;
    }

    public void Setup() 
    {
        gameObject.GetComponent<Rigidbody>().useGravity = true;
        gameObject.GetComponent<BoxCollider>().isTrigger = false;
    }
}
