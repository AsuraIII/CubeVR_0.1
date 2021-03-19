using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Monster : MonoBehaviour
{

    public Transform rayStarPos;

    public Transform groundChecker;
    private Transform standBrick;
    private float moveSpeed = 2.0f;
    private float groundHeightCheck = 0.2f;
    private float down45LengthCheck = 0.5f;
    private float forwardLengthCheck = 1.0f;
    private LayerMask brickLayer;

    private bool isMoving = false;
    private bool isTurning = false;

    private Animator m_animator;
    // Start is called before the first frame update
    void Start()
    {
        brickLayer = LayerMask.GetMask("Brick");
        m_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //IsGrounded();
        RayCastCheckIsOnCorner();

        if (IsGrounded() && !isTurning)
        {
            Move();
        }

        // Debug.Log(isTurning);
        //Debug.Log(isMoving);
    }

    private void Move()
    {
        if (Input.GetKey(KeyCode.K))
        {
            isMoving = true;
            transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
        }
        if (Input.GetKeyUp(KeyCode.K))
        {
            isMoving = false;
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            StartCoroutine(QRotate(0, -90, 0, 1f));
            //transform.localRotation *= Quaternion.Euler(0, -90, 0);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            StartCoroutine(QRotate(0, 90, 0, 1f));
            //transform.localRotation *= Quaternion.Euler(0, 90, 0);
        }
    }
    private bool RayCastCheckIsOnCorner()
    {
        if (IsGrounded())
        {
            RaycastHit hit;
            Vector3 down45 = (rayStarPos.forward - rayStarPos.up).normalized;
            Debug.DrawRay(rayStarPos.position, down45 * down45LengthCheck, Color.green);
            if (Physics.Raycast(rayStarPos.position, down45, out hit, down45LengthCheck, brickLayer))
            {

                if (IsParallel(transform.forward, hit.transform.forward))
                {
                    RaycastHit hit2;
                    //if (Physics.Raycast(rayStarPos.position, rayStarPos.forward, out hit2, ))
                    ClimbBrickWithAnimation(hit.transform, hit.point);

                    return true;
                }
                //if (Vector3.Dot(transform.forward, hit.transform.forward) != 0)
                //    Debug.Log(hit.transform.position
            }
        }
        return false;
    }


    private bool IsParallel(Vector3 vec1, Vector3 vec2)
    {
        float dotNumf = Vector3.Dot(vec1, vec2);
        if (Mathf.Approximately(dotNumf, 1.0f))
        {
            return true;
        }
        else if (Mathf.Approximately(dotNumf, -1.0f))
        {
            return true;
        }
        else
            return false;
    }

    private bool IsGrounded()
    {
        RaycastHit hit;
        if (!isTurning)
        {
            if (Physics.Raycast(groundChecker.transform.position, -groundChecker.transform.forward, out hit, groundHeightCheck, brickLayer))
            {
                ChangeStandBrick(hit.transform);
                return true;
            }
        }
        return false;
    }

    private void ChangeStandBrick(Transform brick)
    {
        if (standBrick == null)
        {
            standBrick = brick;
            this.transform.SetParent(brick);
        }
        else if (standBrick != brick)
        {
            this.transform.SetParent(brick);
            standBrick = brick;
        }
    }

    private void ClimbBrickWithAnimation(Transform brick, Vector3 hitPoint)
    {
        if (standBrick != brick && isMoving)
        {
            //m_animator.SetTrigger("Climb");
            transform.parent = null;
            //transform.forward = transform.forward.normalized;
            //Debug.Log(transform.forward);

            //transform.localRotation *= Quaternion.Euler(-90, 0, 0);
            //transform.position = hitPoint;

            //transform.forward = transform.up;

            StartCoroutine(ClimbAndRotate(hitPoint, 2f));

            //transform.up = -brick.forward;
            //standBrick = brick;
        }
    }

    private IEnumerator ClimbAndRotate(Vector3 hitpoint, float time)
    {
        isMoving = false;
        isTurning = true;
        Vector3 starPos = transform.position;
        Vector3 finalPos = hitpoint;

        Quaternion startRotation = transform.localRotation;
        Quaternion finalRotation = (transform.localRotation *= Quaternion.Euler(-90, 0, 0));

        float elapsedTime = 0;
        //transform.localRotation *= Quaternion.Euler(-90, 0, 0);
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(startRotation, finalRotation, (elapsedTime / time));
            transform.position = Vector3.Lerp(starPos, finalPos, (elapsedTime / time));
            yield return null;
        }
        isTurning = false;
    }

    private IEnumerator QRotate(float x, float y, float z, float time)
    {
        isTurning = true;
        Quaternion startRotation = transform.localRotation;
        Quaternion finalRotation = (transform.localRotation *= Quaternion.Euler(x, y, z));
        float elapsedTime = 0;
        //transform.localRotation *= Quaternion.Euler(-90, 0, 0);
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(startRotation, finalRotation, (elapsedTime / time));
            yield return null;
        }
        isTurning = false;
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.transform.tag == "Brick")
    //    {
    //        this.transform.parent = collision.transform;
    //    }
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.transform.tag == "Brick")
    //    {
    //        this.transform.parent = null;
    //    }
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "Brick")
    //    {
    //        Debug.Log(this.gameObject.name);
    //        this.transform.parent = other.transform;
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.tag == "Brick")
    //    {
    //        this.transform.parent = null;
    //    }
    //}
}
