using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private float horizontalSpeed = 6f;
    [SerializeField] private float jumpHeight = 7f;
    [SerializeField] private float slideDuration = 2f;

    [Header("Scene References")]
    [SerializeField] private GameObject playerAnim;
    [SerializeField] private Transform playerMesh; // ✅ This is ONLY the mesh, not the whole object
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject destructionEffectPrefab;

    private float[] lanes = new float[] { -14f, -8f, -2f, 4f, 10f };
    private int currentLaneIndex = 2;

    private Vector2 touchStartPosition;
    private float holdStartTime;
    private bool isSwiping = false;
    private bool isMovingHorizontally = false;
    private bool isJumping = false;
    private bool isSliding = false;
    private bool isDead = false;

    private Vector3 originalMeshScale;
    private Vector3 originalMeshPosition;
    private float cameraOffsetZ;

    private const float holdThreshold = 0.5f;
    private GameObject heldObject;

    void Start()
    {
        originalMeshScale = playerMesh.localScale;
        originalMeshPosition = playerMesh.localPosition;
        cameraOffsetZ = transform.position.z - mainCamera.transform.position.z;
    }

    void Update()
    {
        if (isDead) return;

        // Move forward
        transform.Translate(Vector3.forward * Time.deltaTime * playerSpeed, Space.World);
        mainCamera.transform.Translate(Vector3.forward * Time.deltaTime * playerSpeed, Space.World);

        // Lock Z offset
        Vector3 correctedPosition = transform.position;
        correctedPosition.z = mainCamera.transform.position.z + cameraOffsetZ;
        transform.position = correctedPosition;

        // Horizontal Movement
        if (isMovingHorizontally)
        {
            Vector3 targetPosition = new Vector3(lanes[currentLaneIndex], transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, horizontalSpeed * Time.deltaTime);

            if (transform.position.x == lanes[currentLaneIndex])
            {
                isMovingHorizontally = false;
            }
        }

        // Touch Input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPosition = touch.position;
                    isSwiping = true;
                    holdStartTime = Time.time;
                    heldObject = GetTouchedObject(touch.position);
                    break;

                case TouchPhase.Moved:
                    if (isSwiping)
                    {
                        float deltaX = touch.position.x - touchStartPosition.x;
                        float deltaY = touch.position.y - touchStartPosition.y;

                        if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY) && Mathf.Abs(deltaX) > 50f && !isMovingHorizontally)
                        {
                            if (deltaX > 0 && currentLaneIndex < lanes.Length - 1)
                            {
                                currentLaneIndex++;
                                isMovingHorizontally = true;
                            }
                            else if (deltaX < 0 && currentLaneIndex > 0)
                            {
                                currentLaneIndex--;
                                isMovingHorizontally = true;
                            }
                        }
                        else if (Mathf.Abs(deltaY) > Mathf.Abs(deltaX) && Mathf.Abs(deltaY) > 50f)
                        {
                            if (deltaY > 0 && !isJumping)
                            {
                                StartCoroutine(Jump());
                            }
                            else if (deltaY < 0 && !isSliding)
                            {
                                StartCoroutine(Slide());
                            }
                        }
                    }
                    break;

                case TouchPhase.Stationary:
                    if (heldObject != null && Time.time - holdStartTime >= holdThreshold)
                    {
                        ApplyHoldAction(heldObject);
                        heldObject = null;
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isSwiping = false;
                    heldObject = null;
                    break;
            }
        }
    }

    private GameObject GetTouchedObject(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    private void ApplyHoldAction(GameObject obj)
    {
        if (obj.CompareTag("metallic") || obj.CompareTag("heavyMetallic"))
        {
            PushAndDestroy(obj, Vector3.forward * 100f);
        }
    }

    private void PushAndDestroy(GameObject obj, Vector3 force)
    {
        Collider col = obj.GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.AddForce(force, ForceMode.Impulse);
        }

        StartCoroutine(DestroyAfterDelay(obj, 2f));
    }

    private IEnumerator DestroyAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
        {
            Instantiate(destructionEffectPrefab, obj.transform.position, Quaternion.identity);
            Destroy(obj);
        }
    }

    private IEnumerator Jump()
    {
        isJumping = true;
        playerAnim.GetComponent<Animator>().Play("Jump Forward");

        float jumpSpeed = horizontalSpeed / 2f;
        float targetY = transform.position.y + jumpHeight;

        while (transform.position.y < targetY)
        {
            transform.position = new Vector3(transform.position.x, Mathf.MoveTowards(transform.position.y, targetY, jumpSpeed * Time.deltaTime), transform.position.z);
            yield return null;
        }

        float groundY = 3.8f;
        while (transform.position.y > groundY)
        {
            transform.position = new Vector3(transform.position.x, Mathf.MoveTowards(transform.position.y, groundY, jumpSpeed * Time.deltaTime), transform.position.z);
            yield return null;
        }

        isJumping = false;
        if (!isSliding && !isDead)
            playerAnim.GetComponent<Animator>().Play("Running");
    }

    private IEnumerator Slide()
    {
        isSliding = true;
        playerAnim.GetComponent<Animator>().Play("Running Slide");

        // ✅ Shrink only the mesh visually, not the entire object
        playerMesh.localScale = new Vector3(originalMeshScale.x, originalMeshScale.y * 0.5f, originalMeshScale.z);
        playerMesh.localPosition = new Vector3(originalMeshPosition.x, originalMeshPosition.y - 0.5f, originalMeshPosition.z);

        yield return new WaitForSeconds(slideDuration);

        // Restore mesh
        playerMesh.localScale = originalMeshScale;
        playerMesh.localPosition = originalMeshPosition;

        isSliding = false;
        if (!isJumping && !isDead)
            playerAnim.GetComponent<Animator>().Play("Running");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;

        // ✅ Check only for objects tagged "Wall"
        if (collision.collider.CompareTag("nonmetallic"))
        {
            isDead = true;
            playerAnim.GetComponent<Animator>().Play("Death Falling Back");
            Debug.Log("Player died by hitting wall!");
        }
    }
}