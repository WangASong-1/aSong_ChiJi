using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class WallClimber : MonoBehaviour {
    public float ClimbForce;
    public float SmallestEdge = 0.25f;
    public float CoolDown = 0.15f;
    public float MaxAngle = 30;
    public float ClimbRange = 2f;
    public float jumpForece = 1f;
    public Climbingsort currentsort;


    public Transform HandTrans;
    public Animator animator;
    public float minDistance;
    public Rigidbody rigid;
    public Vector3 raycastPosition;
    public ThirdPersonUserControl TPUC;
    public ThirdPersonCharacter TPC;
    public Vector3 VerticalHandOffset;
    public Vector3 HorizontalHandOffset;
    public Vector3 fallHandOffset;
    public LayerMask SpotLayer;
    public LayerMask CurrentSpotLayer;
    public LayerMask CheckLayersForObstacle;
    public LayerMask CheckLayersReachable;


    private Vector3 TargetPoint;
    private Vector3 TargetNormal;

    private float lasttime;
    private float BeginDistance;
    private RaycastHit hit;
    private Quaternion oldRotation;

	// Use this for initialization
	void Start () {
		
	}
    // Update is called once per frame
    void Update()
    {
        if (currentsort == Climbingsort.Walking && Input.GetAxis("Vertical") > 0)
            StartClimbing();

        if (currentsort == Climbingsort.Climbing)
            Climb();

        UpdateStats();

        if (currentsort == Climbingsort.ClimbingTowardsPoint || currentsort == Climbingsort.ClimbingTowardPlateau)
            MoveTowardsPoint();

        if (currentsort == Climbingsort.Jumping || currentsort == Climbingsort.Falling)
            Jumping();
    }

    public void UpdateStats()
    {
        if(currentsort != Climbingsort.Walking && TPC.m_IsGrounded && currentsort != Climbingsort.ClimbingTowardsPoint)
        {
            currentsort = Climbingsort.Walking;
            TPUC.enabled = true;
            rigid.isKinematic = false;
        }

        if (currentsort == Climbingsort.Walking && !TPC.m_IsGrounded)
            currentsort = Climbingsort.Jumping;

        if (currentsort == Climbingsort.Walking && (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0))
            CheckForClimbStart();

    }

    public void StartClimbing()
    {
        if(Physics.Raycast(transform.position+transform.rotation*raycastPosition, transform.forward, 0.4f) && Time.time - lasttime> CoolDown && currentsort == Climbingsort.Walking)
        {
            if (currentsort == Climbingsort.Walking)
                rigid.AddForce(transform.up * jumpForece);
            lasttime = Time.time;
        }
    }
     
    public void Jumping()
    {
        if(rigid.velocity.y < 0 && currentsort != Climbingsort.Falling)
        {
            currentsort = Climbingsort.Falling;
            oldRotation = transform.rotation;
        }

        if (rigid.velocity.y > 0 && currentsort != Climbingsort.Jumping)
            currentsort = Climbingsort.Jumping;

        if (currentsort == Climbingsort.Jumping)
            CheckForSpots(HandTrans.position + fallHandOffset , -transform.up, 0.1f, CheckingSort.nomal);
        
        if(currentsort == Climbingsort.Falling)
        {
            CheckForSpots(HandTrans.position + fallHandOffset + transform.rotation * new Vector3(0.02f, -0.6f, 0), -transform.up, 0.4f, CheckingSort.nomal);
            transform.rotation = oldRotation;
        }
    }

    public void Climb()
    {
        if(Time.time - lasttime > CoolDown && currentsort == Climbingsort.Climbing)
        {
            if(Input.GetAxis("Vertical") > 0)
            {
                CheckForSpots(HandTrans.position + transform.rotation * VerticalHandOffset + transform.up * ClimbRange, -transform.up, ClimbRange, CheckingSort.nomal);

                if (currentsort != Climbingsort.ClimbingTowardsPoint)
                    CheckForPlateau();
            }

            if (Input.GetAxis("Vertical") < 0)
            {
                CheckForSpots(HandTrans.position - transform.rotation * (VerticalHandOffset + new Vector3(0,0.3f,0)) , -transform.up, ClimbRange, CheckingSort.nomal);
                
                if(currentsort != Climbingsort.ClimbingTowardsPoint)
                {
                    rigid.isKinematic = false;
                    TPUC.enabled = true;
                    currentsort = Climbingsort.Falling;
                    oldRotation = transform.rotation;
                }
            }

            if (Input.GetAxis("Horizontal") != 0)
            {
                CheckForSpots(HandTrans.position + transform.rotation * HorizontalHandOffset,transform.right*Input.GetAxis("Horizontal") - transform.up/3.5f, ClimbRange/2, CheckingSort.nomal);

                if(currentsort != Climbingsort.ClimbingTowardsPoint)
                    CheckForSpots(HandTrans.position + transform.rotation * HorizontalHandOffset, transform.right * Input.GetAxis("Horizontal") - transform.up / 1.5f, ClimbRange / 3, CheckingSort.nomal);

                if (currentsort != Climbingsort.ClimbingTowardsPoint)
                    CheckForSpots(HandTrans.position + transform.rotation * HorizontalHandOffset, transform.right * Input.GetAxis("Horizontal") - transform.up / 6f, ClimbRange / 1.5f, CheckingSort.nomal);

                if(currentsort != Climbingsort.ClimbingTowardsPoint)
                {
                    int hor = 0;
                    if(Input.GetAxis("Horizontal") < 0)
                        hor = 1;
                    if (Input.GetAxis("Horizontal") > 0)
                        hor = 1;

                    CheckForSpots(HandTrans.position + transform.rotation * HorizontalHandOffset + transform.right*hor*SmallestEdge/4, transform.forward - transform.up*2, ClimbRange/3f, CheckingSort.turning);

                    if (currentsort != Climbingsort.ClimbingTowardsPoint)
                        CheckForSpots(HandTrans.position + transform.rotation * HorizontalHandOffset + transform.right * 0.2f, transform.forward - transform.up * 2 + transform.right * hor/1.5f, ClimbRange / 3, CheckingSort.turning);
                }
            }
        }
    }

    public void CheckForSpots(Vector3 Spotlocation, Vector3 dir, float range, CheckingSort sort)
    {
        bool foundspot = false;
        if(Physics.Raycast(Spotlocation - transform.right*SmallestEdge/2, dir, out hit, range, SpotLayer))
        {
            if(Vector3.Distance(HandTrans.position , hit.point) - SmallestEdge / 1.5f > minDistance)
            {
                foundspot = true;
                FindSpot(hit, sort);
            }
        }

        if (!foundspot)
        {
            if (Physics.Raycast(Spotlocation + transform.right * SmallestEdge / 2, dir, out hit, range, SpotLayer))
            {
                if (Vector3.Distance(HandTrans.position, hit.point) - SmallestEdge / 1.5f > minDistance)
                {
                    foundspot = true;
                    FindSpot(hit, sort);
                }
            }
        }

        if (!foundspot)
        {
            if (Physics.Raycast(Spotlocation + transform.right * SmallestEdge / 2 + transform.forward*SmallestEdge, dir, out hit, range, SpotLayer))
            {
                if (Vector3.Distance(HandTrans.position, hit.point) > minDistance)
                {
                    foundspot = true;
                    FindSpot(hit, sort);
                }
            }
        }

        if (!foundspot)
        {
            if (Physics.Raycast(Spotlocation - transform.right * SmallestEdge / 2 + transform.forward * SmallestEdge, dir, out hit, range, SpotLayer))
            {
                if (Vector3.Distance(HandTrans.position, hit.point) > minDistance)
                {
                    foundspot = true;
                    FindSpot(hit, sort);
                }
            }
        }
    }

    public void FindSpot(RaycastHit h, CheckingSort sort)
    {
        if(Vector3.Angle(h.normal, Vector3.up)< MaxAngle)
        {
            RayInfo ray = new RayInfo();

            if (sort == CheckingSort.nomal)
                ray = GetClosetPoint(h.transform, h.point + new Vector3(0, -0.01f, 0), transform.forward / 2.5f);
            else if(sort == CheckingSort.turning)
                ray = GetClosetPoint(h.transform, h.point + new Vector3(0, -0.01f, 0), transform.forward / 2.5f - transform.right*Input.GetAxis("Horizontal"));
            else if(sort == CheckingSort.nomal)
                ray = GetClosetPoint(h.transform, h.point + new Vector3(0, -0.01f, 0), -transform.forward / 2.5f);

            TargetPoint = ray.point;
            TargetNormal = ray.nomal;

            if (ray.CanGoToPoint)
            {
                if(currentsort != Climbingsort.Climbing && currentsort != Climbingsort.ClimbingTowardsPoint)
                {
                    TPUC.enabled = false;
                    rigid.isKinematic = true;
                    TPC.m_IsGrounded = false;
                }
                currentsort = Climbingsort.ClimbingTowardsPoint;
                BeginDistance = Vector3.Distance(transform.position, (TargetPoint - transform.rotation * HandTrans.localPosition)); ;
            }
        }
    }

    public RayInfo GetClosetPoint(Transform trans, Vector3 pos, Vector3 dir)
    {
        RayInfo curray = new RayInfo();

        RaycastHit hit2;

        int oldLayer = trans.gameObject.layer;

        trans.gameObject.layer = 14;

        if(Physics.Raycast(pos-dir,dir, out hit2, dir.magnitude*2, CurrentSpotLayer))
        {
            curray.point = hit2.point;
            curray.nomal = hit2.normal;

            if(!Physics.Raycast(HandTrans.position + transform.rotation*new Vector3(0,0.05f,-0.05f), curray.point+ new Vector3(0, 0.5f,0), out hit2, CheckLayersReachable))
            {
                if(!Physics.Linecast(curray.point - Quaternion.Euler(new Vector3(0,90,0))*curray.nomal*0.35f+0.1f*curray.nomal,curray.point + Quaternion.Euler(new Vector3(0,90,0))*curray.nomal*0.35f+0.1f*curray.nomal, out hit2, CheckLayersForObstacle))
                {
                    if (!Physics.Linecast(curray.point - Quaternion.Euler(new Vector3(0, 90, 0)) * curray.nomal * 0.35f + 0.1f * curray.nomal, curray.point + Quaternion.Euler(new Vector3(0, 90, 0)) * curray.nomal * 0.35f + 0.1f * curray.nomal, out hit2, CheckLayersForObstacle))
                    {
                        curray.CanGoToPoint = true;
                    }
                    else
                    {
                        curray.CanGoToPoint = false;
                    }
                }
                else
                {
                    curray.CanGoToPoint = false;
                }
            }
            else
            {
                curray.CanGoToPoint = false;
            }
            trans.gameObject.layer = oldLayer;
            return curray;
        }
        else
        {
            trans.gameObject.layer = oldLayer;
            return curray;
        }
    }

    public void MoveTowardsPoint()
    {
        transform.position = Vector3.Lerp(transform.position, (TargetPoint - transform.rotation * HandTrans.localPosition), Time.deltaTime * ClimbForce);
        Quaternion lookrotation = Quaternion.LookRotation(-TargetNormal);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookrotation, Time.deltaTime * ClimbForce);

        animator.SetBool("OnGround", false);

        float distance = Vector3.Distance(transform.position, (TargetPoint - transform.rotation * HandTrans.localPosition));
        float percent = -9 * (BeginDistance - distance) / BeginDistance;

        animator.SetFloat("Jump", percent);

        if(distance <= 0.01f && currentsort == Climbingsort.ClimbingTowardsPoint)
        {
            transform.position = TargetPoint - transform.rotation * HandTrans.localPosition;
            transform.rotation = lookrotation;

            lasttime = Time.time;
            currentsort = Climbingsort.Climbing;
        }

        if(distance <= 0.25f &&  currentsort == Climbingsort.ClimbingTowardPlateau)
        {
            transform.position = TargetPoint - transform.rotation * HandTrans.localPosition;

            transform.rotation = lookrotation;

            lasttime = Time.time;
            currentsort = Climbingsort.Walking;
        }
    }

    public void CheckForClimbStart()
    {
        RaycastHit hit2;

        Vector3 dir = transform.forward - transform.up / 0.8f;

        if(Physics.Raycast(transform.position + transform.rotation*raycastPosition, dir, 1.6f) && !Input.GetButton("Jump"))
        {
            currentsort = Climbingsort.checkingForClimbStart;
            if (Physics.Raycast(transform.position + new Vector3(0, 1.1f, 0), -transform.up, out hit2, 1.6f, SpotLayer))
                FindSpot(hit2, CheckingSort.falling);
        }
    }

    public void CheckForPlateau()
    {
        RaycastHit hit2;

        Vector3 dir = transform.up + transform.forward / 2;

        if(!Physics.Raycast(HandTrans.position+transform.rotation*VerticalHandOffset,dir, out hit2, 1.5f, SpotLayer))
        {
            currentsort = Climbingsort.ClimbingTowardPlateau;

            if (Physics.Raycast(HandTrans.position + dir * 1.5f, -Vector3.up, out hit2, 1.7f, SpotLayer))
                TargetPoint = HandTrans.position + dir * 1.5f;
            else
                TargetPoint = HandTrans.position + dir * 1.5f + transform.rotation* new Vector3(0,-0.2f,0.25f);
            TargetNormal = -transform.forward;

            animator.SetBool("Crouch", true);
            animator.SetBool("OnGround", true);


        }
    }


    [System.Serializable]

    public enum Climbingsort
    {
        Walking,
        Jumping,
        Falling,
        Climbing,
        ClimbingTowardsPoint,
        ClimbingTowardPlateau,
        checkingForClimbStart
    }

    [System.Serializable]
    public class RayInfo {
        public Vector3 point;
        public Vector3 nomal;
        public bool CanGoToPoint;
    }

    [System.Serializable]
    public enum CheckingSort
    {
        nomal,
        turning,
        falling
    }
	
	
}
