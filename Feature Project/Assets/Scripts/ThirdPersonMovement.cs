using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Sharkey, Logan
/// 5/7/2024
/// Handles the movement and the camera for the player, as well as the ability to teleport to shadows
/// </summary>
public class ThirdPersonMovement : MonoBehaviour
{
    private CharacterController controller;
    private PlayerInputs playerInputs;

    public Light directionalLight;
    public float speed = 6f;
    public float turnSmoothing = 0.1f;
    public Transform cam;
    public bool inShadow = false;
    public Material playerColorMat;
    public Material inShadowColorMat;
    public Material tempColorMat;

    private float turnSmoothVelocity;
    
    [SerializeField]
    private GameObject teleportSphere;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        playerInputs = new PlayerInputs();
        playerInputs.Enable();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        tempColorMat.color = playerColorMat.color;

        teleportSphere.GetComponent<Renderer>().material = playerColorMat;
        teleportSphere.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 myVector = playerInputs.Inputs.WASDmovement.ReadValue<Vector2>();

        Vector3 direction = new Vector3(myVector.x, 0f, myVector.y).normalized;

        if (direction.magnitude > 0.1)
        {
            //uses tan of two angles to find the direction the player should look at
            float lookAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

            //smooths the rotation of the player so as not to be snapping every time direction is changed
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, lookAngle, ref turnSmoothVelocity, turnSmoothing);

            //applies the rotation
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            //makes it so the character moves forward in direction of the camera rather than the world direction
            Vector3 moveDir = Quaternion.Euler(0f, lookAngle, 0f) * Vector3.forward;

            //moves the character in the given direction
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

        }

        //uses the opposite rotation of the directional light and raycasts to see if an object is hit
        //if hit comes back, that means player must be in shadow
        Vector3 dir = directionalLight.transform.forward;
        if (Physics.Raycast(transform.position, -dir, out RaycastHit shadowCheck, 1000f))
        {
            inShadow = true;
            tempColorMat.color = inShadowColorMat.color;
        }
        else
        {
            inShadow = false;
            tempColorMat.color = playerColorMat.color;
        }

        //Hold down RMB to bring up TP aimer
        if (Input.GetMouseButton(1))
        {
            RaycastHit tpCheck;
            Vector3 destination;
            LayerMask layerMask = LayerMask.GetMask("Ground");

            if (!teleportSphere.activeInHierarchy)
                teleportSphere.SetActive(true);

            //Find location being aimed at
            if (Physics.Raycast(cam.position, cam.forward, out tpCheck, 50f, layerMask))
            {
                destination = tpCheck.point;
                teleportSphere.transform.position = new Vector3(destination.x, 0f, destination.z);
            }
            else
            {
                teleportSphere.SetActive (false);
            }
            
        }
        //tp button is released
        if (Input.GetMouseButtonUp(1))
        {
            //tp location is valid (in shadow)
            if (Physics.Raycast(teleportSphere.transform.position, -dir, out RaycastHit shadowtp, 1000f))
            {
                StartCoroutine(tpDelay());
                transform.position = teleportSphere.transform.position;
            }
            teleportSphere.SetActive(false);
        }
    }

    //teleporting doesn't work while player is moving, so player inputs are momentarily disabled to ensure tp goes through
    IEnumerator tpDelay()
    {
        playerInputs.Disable();
        yield return new WaitForSeconds(0.5f);
        playerInputs.Enable();
    }

}
