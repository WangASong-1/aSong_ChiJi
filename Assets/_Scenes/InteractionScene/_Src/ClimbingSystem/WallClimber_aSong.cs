using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class WallClimber_aSong : MonoBehaviour {
    /// <summary>
    /// 爬动的速度
    /// </summary>
    public float ClimbForce = 6f;
    public float SmallestEdge = 0.24f;
    public float CoolDown = 0.1f;
    public float MaxAngle = 30;
    public float ClimbRange = 2.1f;
    public float jumpForece = 400f;
    public Climbingsort currentSort;

    /// <summary>
    /// 手爬墙能放置的位置(中心点)
    /// </summary>
    public Transform HandTrans;
    public Animator animator;
    public float minDistance = 0.16f;
    public Rigidbody rigid;
    //人总是站着的.
    public Vector3 raycastPosition = new Vector3(0, 1, 0);
    /// <summary>
    /// 人物移动等操作方法.在爬动过程中(currentSort == Climbing)禁用
    /// </summary>
    public ThirdPersonUserControl TPUC;
    public ThirdPersonCharacter TPC;
    /// <summary>
    /// 上下爬动的时候的偏移
    /// </summary>
    public Vector3 VerticalHandOffset = new Vector3(0, 0, 0.03f);
    /// <summary>
    /// 左右爬动的时候的偏移
    /// </summary>
    public Vector3 HorizontalHandOffset = new Vector3(0, 0.2f, 0.03f);
    /// <summary>
    /// 跳起和下落的时候使用的偏移
    /// </summary>
    public Vector3 fallHandOffset = new Vector3(0, 0.2f, 0);
    public LayerMask SpotLayer;
    public LayerMask CurrentSpotLayer;
    public LayerMask CheckLayersForObstacle;
    public LayerMask CheckLayersReachable;

    private RayInfo nextClimbPoint = new RayInfo();

    /// <summary>
    /// 移动前记录时间
    /// </summary>
    private float lasttime;
    /// <summary>
    /// 移动前,当前位置到目标点位置的总距离
    /// </summary>
    private float BeginDistance;
    private RaycastHit hit;
    private Quaternion oldRotation;

    private void Update()
    {
        if (TPC.m_IsGrounded)
        {
            return;
        }
        if (currentSort == Climbingsort.None)
            Jumping();
        else
            Climb();

        if (nextClimbPoint.CanGoToPoint)
        {
            Debug.Log("nextClimbPoint.point = " + nextClimbPoint.point);
            //Debug.Log("CanGoToPoint");
            if (currentSort == Climbingsort.None)
            {
                TPUC.enabled = false;
                currentSort = Climbingsort.Climbing;
            }
                

            MoveTowardsPoint();
        }
    }

    /// <summary>
    /// jumping 和 falling 状态判断,以及他们的攀爬点检测
    /// </summary>
    public void Jumping()
    {
        //跳起的时候检测点往上移一点点，预测
        if (rigid.velocity.y > 0)
            CheckForSpots(HandTrans.position + fallHandOffset, -transform.up, 0.1f, CheckingSort.nomal);

        //下落的时候，检测的点往下移一点点.
        if (rigid.velocity.y < 0)
        {
            CheckForSpots(HandTrans.position + fallHandOffset + transform.rotation * new Vector3(0, -0.6f, 0), -transform.up, 0.4f, CheckingSort.nomal);
            transform.rotation = oldRotation;
        }
    }

    /// <summary>
    /// 爬向寻找好的spot
    /// </summary>
    public void MoveTowardsPoint()
    {
        float distance = Vector3.Distance(transform.position, (nextClimbPoint.point - transform.rotation * HandTrans.localPosition));

        //if (distance < 0.5f || currentSort == Climbingsort.ClimbingTowardPlateau)
        //{
            //Debug.Log("MoveTowardsPoint::进入阈值了,直接吸附到指定点");
            //爬动
            transform.position = Vector3.Lerp(transform.position, (nextClimbPoint.point - transform.rotation * HandTrans.localPosition), Time.deltaTime * ClimbForce);
            rigid.isKinematic = true;
        //}

        //面朝向点法线相反的方向(因为可能是斜坡.)
        Quaternion lookrotation = Quaternion.LookRotation(-nextClimbPoint.nomal);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookrotation, Time.deltaTime * ClimbForce);

        animator.SetBool("OnGround", false);



        //跳跃动画程度控制
        float percent = -9 * (BeginDistance - distance) / BeginDistance;
        //Debug.Log("percent = " + percent);

        animator.SetFloat("Jump", percent);

        //阈值.爬到了
        if (distance <= 0.1f && currentSort == Climbingsort.ClimbingTowardsPoint)
        {
            Debug.Log("MoveTowardsPoint::结束阈值,爬向点");
            transform.position = nextClimbPoint.point - transform.rotation * HandTrans.localPosition;
            transform.rotation = lookrotation;

            lasttime = Time.time;
            currentSort = Climbingsort.Climbing;
        }

        //往上爬平台.快到的时候,就直接切换为Walking
        if (distance <= 0.25f && currentSort == Climbingsort.ClimbingTowardPlateau)
        {
            Debug.Log("MoveTowardsPoint::结束阈值,爬向平台");
            transform.position = nextClimbPoint.point - transform.rotation * HandTrans.localPosition;

            transform.rotation = lookrotation;

            lasttime = Time.time;
            currentSort = Climbingsort.None;

            rigid.isKinematic = false;
            rigid.useGravity = true;
            TPUC.enabled = true;
        }
    }

    /// <summary>
    /// 爬动方法.进行下一次爬动的预先计算
    /// </summary>
    public void Climb()
    {
        if (Time.time - lasttime > CoolDown && currentSort == Climbingsort.Climbing)
        {
            //上下爬动检测
            if (Input.GetAxis("Vertical") > 0)
            {
                //Debug.LogError(12);
                CheckForSpots(HandTrans.position + transform.rotation * VerticalHandOffset + transform.up * ClimbRange * 0.5f, -transform.up, ClimbRange * 0.5f, CheckingSort.nomal);

                return;
            }

            if (Input.GetAxis("Vertical") < 0)
            {
                CheckForSpots(HandTrans.position - transform.rotation * (VerticalHandOffset + new Vector3(0, 0.3f, 0)), -transform.up, ClimbRange, CheckingSort.nomal);
                
                return;
            }

            //左右爬动检测
            if (Input.GetAxis("Horizontal") != 0)
            {
                CheckForSpots(HandTrans.position + transform.rotation * HorizontalHandOffset,
                    transform.right * Input.GetAxis("Horizontal") - transform.up / 3.5f, ClimbRange / 2, CheckingSort.nomal);

                if (currentSort != Climbingsort.ClimbingTowardsPoint)
                    CheckForSpots(HandTrans.position + transform.rotation * HorizontalHandOffset,
                        transform.right * Input.GetAxis("Horizontal") - transform.up / 1.5f, ClimbRange / 3, CheckingSort.nomal);

                if (currentSort != Climbingsort.ClimbingTowardsPoint)
                    CheckForSpots(HandTrans.position + transform.rotation * HorizontalHandOffset,
                        transform.right * Input.GetAxis("Horizontal") - transform.up / 6f, ClimbRange / 1.5f, CheckingSort.nomal);

                if (currentSort != Climbingsort.ClimbingTowardsPoint)
                {
                    int hor = 0;
                    if (Input.GetAxis("Horizontal") < 0)
                        hor = -1;
                    if (Input.GetAxis("Horizontal") > 0)
                        hor = 1;

                    CheckForSpots(HandTrans.position + transform.rotation * HorizontalHandOffset + transform.right * hor * SmallestEdge / 4,
                        transform.forward - transform.up * 2, ClimbRange / 3f, CheckingSort.turning);
                    if (currentSort != Climbingsort.ClimbingTowardsPoint)
                        CheckForSpots(HandTrans.position + transform.rotation * HorizontalHandOffset + transform.right * 0.2f,
                            transform.forward - transform.up * 2 + transform.right * hor / 1.5f, ClimbRange / 3, CheckingSort.turning);
                }
            }
        }
    }

    #region CheckForSpots 面前有墙,通过FindSpot检测上升下落的时候的攀爬点(4个点,检测了一个矩形区域) 
    /// <summary>
    /// 检测上升下落的时候的攀爬点(4个点,检测了一个矩形区域) 
    /// </summary>
    /// <param name="Spotlocation">手抓取位置</param>
    /// <param name="dir">判断的方向(-transform.up)</param>
    /// <param name="range">检测范围</param>
    /// <param name="sort">检测时的状态</param>
    public void CheckForSpots(Vector3 Spotlocation, Vector3 dir, float range, CheckingSort sort)
    {
        bool foundspot = false;
        if (Physics.Raycast(Spotlocation , dir, out hit, range, SpotLayer))
        {
            if (Vector3.Distance(HandTrans.position, hit.point) - SmallestEdge / 1.5f > minDistance)
            {
                foundspot = true;
                FindSpot(hit, sort);
            }
        }

        if (!foundspot)
        {
            if (Physics.Raycast(Spotlocation + transform.forward * SmallestEdge, dir, out hit, range, SpotLayer))
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
            if (Physics.Raycast(Spotlocation - transform.right * SmallestEdge / 2, dir, out hit, range, SpotLayer))
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
            if (Physics.Raycast(Spotlocation + transform.right * SmallestEdge / 2 + transform.forward * SmallestEdge, dir, out hit, range, SpotLayer))
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
    #endregion


    #region FindSpot 通过 GetClosetPoint 查找墙边点
    /// <summary>
    /// 查找墙边点 改变状态
    /// </summary>
    /// <param name="h">所使用检测到的射线</param>
    /// <param name="sort">当前的状态</param>
    public void FindSpot(RaycastHit h, CheckingSort sort)
    {
        //若是该攀爬点跟世界的角度过大(过于倾斜),则不能抓
        if (Vector3.Angle(h.normal, Vector3.up) < MaxAngle)
        {
            if (sort == CheckingSort.nomal)
                nextClimbPoint = GetClosetPoint(h.transform, h.point + new Vector3(0, -0.01f, 0), transform.forward / 2.5f);
            else if (sort == CheckingSort.turning)
                nextClimbPoint = GetClosetPoint(h.transform, h.point + new Vector3(0, -0.01f, 0), transform.forward / 2.5f - transform.right * Input.GetAxis("Horizontal"));
            else if (sort == CheckingSort.falling)
                nextClimbPoint = GetClosetPoint(h.transform, h.point + new Vector3(0, -0.01f, 0), -transform.forward / 2.5f);
            Debug.Log("nextClimbPoint111111111 = " + nextClimbPoint.point);
        }
    }
    #endregion


    #region GetClosetPoint 获取最近的一个攀爬点
    /// <summary>
    /// 获取最近的一个攀爬点
    /// </summary>
    /// <param name="trans">被攀爬的物体</param>
    /// <param name="pos">攀爬点的坐标</param>
    /// <param name="dir">攀爬的方向</param>
    /// <returns></returns>
    public RayInfo GetClosetPoint(Transform trans, Vector3 pos, Vector3 dir)
    {
        RayInfo curray = new RayInfo();

        RaycastHit hit2;

        int oldLayer = trans.gameObject.layer;

        //改变被攀爬的gameobject层,改为变可以被spot检测的层
        trans.gameObject.layer = 14;

        Debug.DrawRay(pos - dir, dir, Color.red);
        //从人前面一段距离往人发射射线过来，找墙壁,方便获取法线应该是.
        if (Physics.Raycast(pos - dir, dir, out hit2, dir.magnitude * 2, CurrentSpotLayer))
        {
            curray.point = hit2.point;
            curray.nomal = hit2.normal;
            //if(!Physics.Raycast(HandTrans.position + transform.rotation*new Vector3(0,0.05f,-0.05f), 
            //从手部抓取位置 到 墙壁点往上0.5f检测上面是否还有可以爬的地方.(以便跳到极限距离位置)
            if (!Physics.Linecast(HandTrans.position + transform.rotation * new Vector3(0, 0.05f, -0.05f),
                curray.point + new Vector3(0, 0.5f, 0), out hit2, CheckLayersReachable))
            {
                //检测从墙前面偏移一点点的位置发射射线来检测左右位置是否都是空的能抓
                Debug.DrawLine(curray.point - Quaternion.Euler(new Vector3(0, 90, 0)) * curray.nomal * 0.35f + 0.1f * curray.nomal,
                    curray.point + Quaternion.Euler(new Vector3(0, 90, 0)) * curray.nomal * 0.35f + 0.1f * curray.nomal, Color.green);
                if (!Physics.Linecast(curray.point - Quaternion.Euler(new Vector3(0, 90, 0)) * curray.nomal * 0.35f + 0.1f * curray.nomal,
                    curray.point + Quaternion.Euler(new Vector3(0, 90, 0)) * curray.nomal * 0.35f + 0.1f * curray.nomal, out hit2, CheckLayersForObstacle))
                {
                    Debug.DrawLine(curray.point + Quaternion.Euler(new Vector3(0, 90, 0)) * curray.nomal * 0.35f + 0.1f * curray.nomal + Vector3.down * 0.1f,
                        curray.point - Quaternion.Euler(new Vector3(0, 90, 0)) * curray.nomal * 0.35f + 0.1f * curray.nomal + Vector3.down * 0.1f, Color.yellow);
                    if (!Physics.Linecast(curray.point + Quaternion.Euler(new Vector3(0, 90, 0)) * curray.nomal * 0.35f + 0.1f * curray.nomal,
                        curray.point - Quaternion.Euler(new Vector3(0, 90, 0)) * curray.nomal * 0.35f + 0.1f * curray.nomal, out hit2, CheckLayersForObstacle))
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
    #endregion

    public enum Climbingsort
    {
        Climbing,
        ClimbingTowardsPoint,
        ClimbingTowardPlateau,
        None
    }

    public class RayInfo
    {
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
