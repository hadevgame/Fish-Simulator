using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSwim : MonoBehaviour
{
    public float speed = 0.005f;

    private Vector3 direction;
    private float changeDirectionTime = 4f;
    private float timer = 0f;
    private bool isMovingToTank = false;
    [SerializeField] private bool isRotate ;

    [SerializeField] private Transform tankTransform; // Gắn bể chứa cá vào đây
    private BoxCollider tankCollider;
    [SerializeField] private float padding = 0.005f;
    void Start()
    {
        SetRandomDirection();
        if (tankTransform != null)
        {
            tankCollider = tankTransform.GetComponent<BoxCollider>();
            if (tankCollider == null)
            {
                Debug.LogWarning("Tank does not have a BoxCollider!");
            }
        }
    }

    void Update()
    {
        if (isMovingToTank) return;

        timer += Time.deltaTime;
        if (timer >= changeDirectionTime)
        {
            SetRandomDirection();
            timer = 0f;
        }

        Vector3 newPosition = transform.localPosition + direction * speed * Time.deltaTime;
        Vector3 clampedPosition = ClampPosition(newPosition);

        transform.localPosition = Vector3.Lerp(transform.localPosition, clampedPosition, 0.05f);

        if (direction != Vector3.zero && isRotate)
        {
            Quaternion targetRotation = Quaternion.LookRotation(tankTransform.TransformDirection(direction.normalized), Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 1f);
        }

        if ((newPosition - clampedPosition).sqrMagnitude > 0.02f)
        {
            direction = -direction; 
            timer = 0f;             
            return; 
        }
    }
    void SetRandomDirection()
    {
        direction = Random.insideUnitSphere.normalized;
    }
    Vector3 ClampPosition(Vector3 position)
    {
        if (tankCollider != null)
        {
            Vector3 center = tankCollider.center;
            Vector3 size = tankCollider.size;

            Vector3 localMin = center - size * 0.5f;
            Vector3 localMax = center + size * 0.5f;

            position.x = Mathf.Clamp(position.x, localMin.x + padding, localMax.x - padding);
            position.y = Mathf.Clamp(position.y, localMin.y + padding, localMax.y - padding);
            position.z = Mathf.Clamp(position.z, localMin.z + padding, localMax.z - padding);
        }

        return position;
    }
    public void SetMovingToTank(bool moving)
    {
        isMovingToTank = moving;
    }

    public void SetUp(Transform transform, float fishspeed, float tankpadding) 
    {
        tankTransform = transform;
        tankCollider = tankTransform.GetComponent<BoxCollider>();
        speed = fishspeed;
        padding = tankpadding;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameConstants.GROUNDINTANK)) 
        {
            direction = -direction;
            timer = 0f; 
        }
    }
}
    