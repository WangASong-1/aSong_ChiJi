using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class WallClimber_bak : MonoBehaviour {
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
    public Vector3 HorizontalHandOffset = new Vector3(0,0.2f,0.03f);
    /// <summary>
    /// 跳起和下落的时候使用的偏移
    /// </summary>
    public Vector3 fallHandOffset = new Vector3(0,0.2f,0);
    public LayerMask SpotLayer;
    public LayerMask CurrentSpotLayer;
    public LayerMask CheckLayersForObstacle;
    public LayerMask CheckLayersReachable;

    //攀爬目标点
    private Vector3 TargetPoint;
    private Vector3 TargetNormal;

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
	
    void Update()
    {
        //1. w按钮刚按下就开始爬墙
        //if (currentSort == Climbingsort.Walking && Input.GetAxis("Vertical") > 0)
        //StartClimbing();
        /*
        if (currentSort == Climbingsort.Climbing)
            Climb();

        UpdateStats();

        if (currentSort == Climbingsort.ClimbingTowardsPoint || currentSort == Climbingsort.ClimbingTowardPlateau)
            MoveTowardsPoint();

        if (currentSort == Climbingsort.Jumping || currentSort == Climbingsort.Falling)
            Jumping();
            */

        UpdateStats();
        switch (currentSort)
        {
            case Climbingsort.Climbing:
                Climb();
                break;
            case Climbingsort.checkingForClimbStart:
                break;
            case Climbingsort.ClimbingTowardPlateau:
            case Climbingsort.ClimbingTowardsPoint:
                MoveTowardsPoint();
                break;
            case Climbingsort.Falling:
            case Climbingsort.Jumping:
                Jumping();
                break;
            case Climbingsort.Walking:
                //Walking();
                break;
        }
    }

    public void UpdateStats()
    {
        //若没在走路状态,然后又落在地上,并且不是爬墙状态.那么变走路状态,并启动移动按钮
        if(currentSort != Climbingsort.Walking && TPC.m_IsGrounded && currentSort != Climbingsort.ClimbingTowardsPoint)
        {
            currentSort = Climbingsort.Walking;
            TPUC.enabled = true;
            rigid.isKinematic = false;
        }

        //走路状态但是不在地上,那么改变为跳跃状态
        if (currentSort == Climbingsort.Walking && !TPC.m_IsGrounded)
            currentSort = Climbingsort.Jumping;

        //走路状态并且只要在移动(包括惯性),那么开始检查面前是否有墙可爬
        if (currentSort == Climbingsort.Walking && (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0))
            CheckForClimbStart();

        if(!TPC.m_IsGrounded && (currentSort == Climbingsort.Climbing || currentSort == Climbingsort.ClimbingTowardPlateau || currentSort == Climbingsort.ClimbingTowardsPoint))
        {
            animator.SetBool("Climbing", true);
        }
        else
        {
            animator.SetBool("Climbing", false);
            animator.SetBool("ClimbUp", false);
            animator.SetBool("ClimbDown", false);
            rigid.isKinematic = false;
        }
    }

    /// <summary>
    /// 开始爬墙,起跳
    /// </summary>
    public void StartClimbing()
    {
        //射线检测 
        //Debug.Log("transform.position+transform.rotation*raycastPosition = " + transform.position + transform.rotation * raycastPosition);
        Debug.DrawRay(transform.position + transform.rotation * raycastPosition, transform.forward, Color.green);
        //Debug.LogError("transform.position+transform.rotation*raycastPosition = " + transform.position + transform.rotation * raycastPosition);
        if(Physics.Raycast(transform.position+transform.rotation*raycastPosition, transform.forward, 0.4f) 
            && Time.time - lasttime> CoolDown && currentSort == Climbingsort.Walking)
        {
            if (currentSort == Climbingsort.Walking)
                rigid.AddForce(transform.up * jumpForece);
            lasttime = Time.time;
        }
    }
     
    /// <summary>
    /// jumping 和 falling 状态判断,以及他们的攀爬点检测
    /// </summary>
    public void Jumping()
    {
        //掉落
        if(rigid.velocity.y < 0 && currentSort != Climbingsort.Falling)
        {
            currentSort = Climbingsort.Falling;
            oldRotation = transform.rotation;
        }

        if (rigid.velocity.y > 0 && currentSort != Climbingsort.Jumping)
            currentSort = Climbingsort.Jumping;

        //跳起的时候检测点往上移一点点，预测
        if (currentSort == Climbingsort.Jumping)
            CheckForSpots(HandTrans.position + fallHandOffset , -transform.up, 0.1f, CheckingSort.nomal);
        
        //下落的时候，检测的点往下移一点点.
        if(currentSort == Climbingsort.Falling)
        {
            CheckForSpots(HandTrans.position + fallHandOffset + transform.rotation * new Vector3(0.02f, -0.6f, 0), -transform.up, 0.4f, CheckingSort.nomal);
            transform.rotation = oldRotation;
        }
    }

    /// <summary>
    /// 爬动方法.进行下一次爬动的预先计算
    /// </summary>
    public void Climb()
    {
        if(Time.time - lasttime > CoolDown && currentSort == Climbingsort.Climbing)
        {
            //上下爬动检测
            if(Input.GetAxis("Vertical") > 0)
            {
                //Debug.LogError(12);
                CheckForSpots(HandTrans.position + transform.rotation * VerticalHandOffset + transform.up * ClimbRange * 0.5f, -transform.up, ClimbRange * 0.5f, CheckingSort.nomal);

                //当没有检测到上面有障碍物,就爬平台
                if (currentSort != Climbingsort.ClimbingTowardsPoint)
                    CheckForPlateau();
            }

            if (Input.GetAxis("Vertical") < 0)
            {
                CheckForSpots(HandTrans.position - transform.rotation * (VerticalHandOffset + new Vector3(0,0.3f,0)) , -transform.up, ClimbRange, CheckingSort.nomal);
                
                if(currentSort != Climbingsort.ClimbingTowardsPoint)
                {
                    rigid.isKinematic = false;
                    TPUC.enabled = true;
                    currentSort = Climbingsort.Falling;
                    oldRotation = transform.rotation;

                    animator.SetBool("ClimbDown", true);
                }
            }

            //左右爬动检测
            if (Input.GetAxis("Horizontal") != 0)
            {
                CheckForSpots(HandTrans.position + transform.rotation * HorizontalHandOffset,
                    transform.right*Input.GetAxis("Horizontal") - transform.up/3.5f, ClimbRange/2, CheckingSort.nomal);

                if(currentSort != Climbingsort.ClimbingTowardsPoint)
                    CheckForSpots(HandTrans.position + transform.rotation * HorizontalHandOffset, 
                        transform.right * Input.GetAxis("Horizontal") - transform.up / 1.5f, ClimbRange / 3, CheckingSort.nomal);

                if (currentSort != Climbingsort.ClimbingTowardsPoint)
                    CheckForSpots(HandTrans.position + transform.rotation * HorizontalHandOffset, 
                        transform.right * Input.GetAxis("Horizontal") - transform.up / 6f, ClimbRange / 1.5f, CheckingSort.nomal);

                if(currentSort != Climbingsort.ClimbingTowardsPoint)
                {
                    int hor = 0;
                    if(Input.GetAxis("Horizontal") < 0)
                        hor = -1;
                    if (Input.GetAxis("Horizontal") > 0)
                        hor = 1;

                    CheckForSpots(HandTrans.position + transform.rotation * HorizontalHandOffset + transform.right*hor*SmallestEdge/4, 
                        transform.forward - transform.up*2, ClimbRange/3f, CheckingSort.turning);
                    if (currentSort != Climbingsort.ClimbingTowardsPoint)
                        CheckForSpots(HandTrans.position + transform.rotation * HorizontalHandOffset + transform.right * 0.2f, 
                            transform.forward - transform.up * 2 + transform.right * hor/1.5f, ClimbRange / 3, CheckingSort.turning);
                }
            }
        }
    }

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

    /// <summary>
    /// 查找墙边点 改变状态
    /// </summary>
    /// <param name="h">所使用检测到的射线</param>
    /// <param name="sort">当前的状态</param>
    public void FindSpot(RaycastHit h, CheckingSort sort)
    {
        //若是该攀爬点跟世界的角度过大(过于倾斜),则不能抓
        if(Vector3.Angle(h.normal, Vector3.up)< MaxAngle)
        {
            RayInfo ray = new RayInfo();

            if (sort == CheckingSort.nomal)
                ray = GetClosetPoint(h.transform, h.point + new Vector3(0, -0.01f, 0), transform.forward / 2.5f);
            else if(sort == CheckingSort.turning)
                ray = GetClosetPoint(h.transform, h.point + new Vector3(0, -0.01f, 0), transform.forward / 2.5f - transform.right*Input.GetAxis("Horizontal"));
            else if(sort == CheckingSort.falling)
                ray = GetClosetPoint(h.transform, h.point + new Vector3(0, -0.01f, 0), -transform.forward / 2.5f);

            TargetPoint = ray.point;
            TargetNormal = ray.nomal;

            //如果该点能攀爬
            if (ray.CanGoToPoint)
            {
                //若不是攀爬状态,并且不是爬动状态
                if(currentSort != Climbingsort.Climbing && currentSort != Climbingsort.ClimbingTowardsPoint)
                {
                    //爬向最近的spot.爬动时人物控制取消,刚体取消,不在地上
                    TPUC.enabled = false;
                    rigid.isKinematic = true;
                    TPC.m_IsGrounded = false;
                }
                currentSort = Climbingsort.ClimbingTowardsPoint;
                BeginDistance = Vector3.Distance(transform.position, (TargetPoint - transform.rotation * HandTrans.localPosition));
            }
        }
    }

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
        if(Physics.Raycast(pos-dir,dir, out hit2, dir.magnitude*2, CurrentSpotLayer))
        {
            curray.point = hit2.point;
            curray.nomal = hit2.normal;
            //Debug.Log("curray.point = " + curray.point);
            //Debug.Log("hit obj = " + hit2.transform.name);
            //攀爬时手
            //Debug.DrawLine(HandTrans.position + transform.rotation * new Vector3(0, 0.05f, -0.05f),curray.point + new Vector3(0, 0.5f, 0), Color.blue);
            //Debug.Log("HandTrans.position + transform.rotation * new Vector3(0, 0.05f, -0.05f) = " + (HandTrans.position + transform.rotation * new Vector3(0, 0.05f, -0.05f)));
            //Debug.LogError("寻找最近的攀爬点");
            //if(!Physics.Raycast(HandTrans.position + transform.rotation*new Vector3(0,0.05f,-0.05f), 
            //从手部抓取位置 到 墙壁点往上0.5f检测上面是否还有可以爬的地方.(以便跳到极限距离位置)
            if (!Physics.Linecast(HandTrans.position + transform.rotation * new Vector3(0, 0.05f, -0.05f),
                curray.point+ new Vector3(0, 0.5f,0), out hit2, CheckLayersReachable))
            {
                //检测从墙前面偏移一点点的位置发射射线来检测左右位置是否都是空的能抓
                Debug.DrawLine(curray.point - Quaternion.Euler(new Vector3(0, 90, 0)) * curray.nomal * 0.35f + 0.1f * curray.nomal,
                    curray.point + Quaternion.Euler(new Vector3(0, 90, 0)) * curray.nomal * 0.35f + 0.1f * curray.nomal, Color.green);
                if(!Physics.Linecast(curray.point - Quaternion.Euler(new Vector3(0,90,0))*curray.nomal*0.35f+0.1f*curray.nomal,
                    curray.point + Quaternion.Euler(new Vector3(0,90,0))*curray.nomal*0.35f+0.1f*curray.nomal, out hit2, CheckLayersForObstacle))
                {
                    Debug.DrawLine(curray.point + Quaternion.Euler(new Vector3(0, 90, 0)) * curray.nomal * 0.35f + 0.1f * curray.nomal + Vector3.down*0.1f,
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

    /// <summary>
    /// 爬向寻找好的spot
    /// </summary>
    public void MoveTowardsPoint()
    {
        //爬动
        transform.position = Vector3.Lerp(transform.position, (TargetPoint - transform.rotation * HandTrans.localPosition), Time.deltaTime * ClimbForce);
        //面朝向点法线相反的方向(因为可能是斜坡.)
        Quaternion lookrotation = Quaternion.LookRotation(-TargetNormal);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookrotation, Time.deltaTime * ClimbForce);

        animator.SetBool("OnGround", false);

        float distance = Vector3.Distance(transform.position, (TargetPoint - transform.rotation * HandTrans.localPosition));
        //跳跃动画程度控制
        float percent = -9 * (BeginDistance - distance) / BeginDistance;
        //Debug.Log("percent = " + percent);

        animator.SetFloat("Jump", percent);
        

        //阈值.爬到了
        if (distance <= 0.01f && currentSort == Climbingsort.ClimbingTowardsPoint)
        {
            transform.position = TargetPoint - transform.rotation * HandTrans.localPosition;
            transform.rotation = lookrotation;

            lasttime = Time.time;
            currentSort = Climbingsort.Climbing;
        }

        //往上爬平台.快到的时候,就直接切换为Walking
        if(distance <= 0.25f &&  currentSort == Climbingsort.ClimbingTowardPlateau)
        {
            transform.position = TargetPoint - transform.rotation * HandTrans.localPosition;

            transform.rotation = lookrotation;

            lasttime = Time.time;
            currentSort = Climbingsort.Walking;

            rigid.isKinematic = false;
            TPUC.enabled = true;
        }
        
    }

    //检查是否可以爬墙,查找可以爬的点
    public void CheckForClimbStart()
    {
        RaycastHit hit2;
        //爬墙射线检测方向.从身上发射到前下方
        Vector3 dir = transform.forward - transform.up / 0.8f;
        //Debug.DrawRay(transform.position + transform.rotation * raycastPosition, dir, Color.yellow);
        //Debug.Log("CheckForClimbStart");
        //Debug.LogError("CheckForClimbStart");
        if(!Physics.Raycast(transform.position + transform.rotation*raycastPosition, dir, 1.6f) && !Input.GetButton("Jump"))
        {
            currentSort = Climbingsort.checkingForClimbStart;
            //从玩家身上往下发射射线寻找点.因为是从跳跃开始,在落下过程中寻找攀爬点.所以是从人身体胸部左右位置往下发射射线
            //Debug.DrawRay(transform.position + new Vector3(0, 1.1f, 0), -transform.up, Color.yellow);
            //Debug.LogError("寻找攀爬点");
            if (Physics.Raycast(transform.position + new Vector3(0, 1.1f, 0), -transform.up, out hit2, 1.6f, SpotLayer))
                FindSpot(hit2, CheckingSort.falling);
        }
    }

    /// <summary>
    /// 检测是否可站立的平台
    /// </summary>
    public void CheckForPlateau()
    {
        //Debug.Log("CheckForPlateau");
        RaycastHit hit2;

        Vector3 dir = transform.up + transform.forward / 4;
        //Debug.LogError("CheckForPlateau11111");

        //上前方没有障碍物了，说明上面是平台顶端
        if (!Physics.Raycast(HandTrans.position+transform.rotation*VerticalHandOffset,dir, out hit2, 1.5f, SpotLayer))
        {
            currentSort = Climbingsort.ClimbingTowardPlateau;
            animator.SetBool("ClimbUp", true);
            //Debug.LogError("CheckForPlateau");
            //上方
            if (Physics.Raycast(HandTrans.position + dir * 1.5f, -Vector3.up, out hit2, 1.7f, SpotLayer))
            {
                Debug.Log("这里有墙可以踩");
                //TargetPoint = HandTrans.position + dir * 1.5f;
                //TargetPoint - transform.rotation * HandTrans.localPosition
                TargetPoint = hit2.point  + transform.rotation * HandTrans.localPosition + transform.up*0.5f;
            }
            else
            {
                //这里没墙了,应该是抓到了树枝边上,准备跳吧
                TargetPoint = HandTrans.position + dir * 1.5f + transform.rotation * new Vector3(0, -0.2f, 0.25f);

            }
            TargetNormal = -transform.forward;
            //Debug.DrawRay(HandTrans.position + dir * 1.5f, -Vector3.up*1.7f, Color.yellow);
            //Debug.Log("TargetPoint = " + TargetPoint);
            //Debug.LogError("CheckForPlateau");
            animator.SetBool("Crouch", true);
            animator.SetBool("OnGround", true);
        }
    }


    [System.Serializable]
    public enum Climbingsort
    {
        /// <summary>
        /// 站立状态
        /// </summary>
        Walking,
        /// <summary>
        /// 跳起状态
        /// </summary>
        Jumping,
        /// <summary>
        /// 下落状态
        /// </summary>
        Falling,
        /// <summary>
        /// 攀附状态
        /// </summary>
        Climbing,
        /// <summary>
        /// 攀爬ing状态
        /// </summary>
        ClimbingTowardsPoint,
        /// <summary>
        /// 爬向平台
        /// </summary>
        ClimbingTowardPlateau,
        /// <summary>
        /// 墙边操作,检测是否开始爬墙
        /// </summary>
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
