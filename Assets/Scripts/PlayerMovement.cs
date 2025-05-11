using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float playerSpeed = 5;
    [SerializeField]
    private float horizontalSpeed = 6;
    [SerializeField]
    private float jumpHeight = 7;
    [SerializeField]
    private float slideDuration = 1.2f;
    [SerializeField]
    private GameObject playerAnim;

    [SerializeField]
    private Camera mainCamera;

    private float[] lanes = new float[] { -14f, -8f, -2f, 4f, 10f };
    private int currentLaneIndex = 2;
    private Vector2 touchStartPosition;
    private bool isSwiping = false;
    private bool isMovingHorizontally = false;
    private bool isJumping = false;
    private bool isSliding = false;

    private Vector3 originalScale;

    private float cameraOffsetZ; // Maintain initial offset between player and camera

    void Start()
    {
        originalScale = transform.localScale;
        cameraOffsetZ = transform.position.z - mainCamera.transform.position.z;
    }

    void Update()
    {
        // Move forward
        transform.Translate(Vector3.forward * Time.deltaTime * playerSpeed, Space.World);
        mainCamera.transform.Translate(Vector3.forward * Time.deltaTime * playerSpeed, Space.World);

        // Lock Z offset (insurance against any animation-based visual drift)
        Vector3 correctedPosition = transform.position;
        correctedPosition.z = mainCamera.transform.position.z + cameraOffsetZ;
        transform.position = correctedPosition;

        // Horizontal movement
        if (isMovingHorizontally)
        {
            Vector3 targetPosition = new Vector3(lanes[currentLaneIndex], transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, horizontalSpeed * Time.deltaTime);

            if (transform.position.x == lanes[currentLaneIndex])
            {
                isMovingHorizontally = false;
            }
        }

        // ------------------ Touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPosition = touch.position;
                    isSwiping = true;
                    break;

                case TouchPhase.Moved:
                    if (isSwiping)
                    {
                        float deltaX = touch.position.x - touchStartPosition.x;
                        float deltaY = touch.position.y - touchStartPosition.y;

                        // Horizontal swipe
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
                        // Vertical swipe
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
                            playerAnim.GetComponent<Animator>().Play("Running");

                        }
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isSwiping = false;
                    break;
            }
        }
    }

    IEnumerator Jump()
    {
        isJumping = true;
        playerAnim.GetComponent<Animator>().Play("Big Jump");

        float jumpSpeed = horizontalSpeed / 2f;
        float targetY = transform.position.y + jumpHeight;

        // Move upward
        while (transform.position.y < targetY)
        {
            Vector3 newPos = transform.position;
            newPos.y = Mathf.MoveTowards(transform.position.y, targetY, jumpSpeed * Time.deltaTime);
            transform.position = newPos;
            yield return null;
        }

        // Move downward
        float groundY = 3.8f;
        while (transform.position.y > groundY)
        {
            Vector3 newPos = transform.position;
            newPos.y = Mathf.MoveTowards(transform.position.y, groundY, jumpSpeed * Time.deltaTime);
            transform.position = newPos;
            yield return null;
        }

        isJumping = false;
    }

    IEnumerator Slide()
    {
        isSliding = true;
        playerAnim.GetComponent<Animator>().Play("Running Slide");

        transform.localScale = new Vector3(originalScale.x, originalScale.y / 2, originalScale.z);
        yield return new WaitForSeconds(slideDuration);
        transform.localScale = originalScale;

        isSliding = false;
    }
}