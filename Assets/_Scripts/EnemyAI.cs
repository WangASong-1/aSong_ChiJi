using UnityEngine;
using System.Collections;

public enum AttackStatus{
    boxing, swing, walk
}

public class EnemyAI : MonoBehaviour {
    public AttackStatus mAtkStatus = AttackStatus.walk;


    public float speedDampTime = 0.3f;
    public float anglarSpeedDampTime = 0.3f;

    private Animator mAnim;
    private UnityEngine.AI.NavMeshAgent mNav;
    private Transform player;

    private float speed = 0f;
    private float angel = 0f;
    [SerializeField]
    private float boxRange = 1f;
    [SerializeField]
    private float swingRange = 1f;
    [SerializeField]
    private GameObject weapon;

    private void Awake()
    {
        mAnim = GetComponent<Animator>();
        mNav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnAnimatorMove()
    {
        transform.position += mAnim.deltaPosition * Time.deltaTime;
        transform.rotation = mAnim.rootRotation;
    }

    void Start () {
	
	}
	
	void Update () {
        mNav.SetDestination(player.position);
        Attacking();

        Moving();
    }

    void Moving()
    {
        if(mAtkStatus != AttackStatus.walk)
        {
            mNav.velocity = Vector3.zero;
        }
            
        speed = Vector3.Project(mNav.velocity, transform.forward).magnitude;
        float _angel = 0f;
        //计算角度
        _angel = Vector3.Angle(transform.forward, mNav.desiredVelocity);
        Vector3 dir = mNav.destination;
        dir.y = transform.position.y;
        if (mNav.desiredVelocity == Vector3.zero)
            dir = dir - transform.position;
        else
            dir = mNav.desiredVelocity;

        _angel = Vector3.Angle(transform.forward, dir);
        if (_angel < 3f)
            _angel = 0f;
        if (_angel >= 90)
        {
            //大于90°，因为没有正负,其实就是180°，所以要转身,让速度为0
            speed = 0f;
            mNav.velocity = Vector3.zero;
        }
        //差积根据性质右手定则，计算在其左右
        _angel *= Mathf.Sign(Vector3.Cross(transform.forward, dir).y) * Mathf.Deg2Rad;
        angel = _angel;

        //在 anglarSpeedDampTime 秒内切换到指定角度去, Time.deltaTime迭代值
        mAnim.SetFloat("Angular", angel, anglarSpeedDampTime, Time.deltaTime);
        //突然就切换到这个值的角度去了

        mAnim.SetFloat("Speed", speed, speedDampTime, Time.deltaTime);
    }

    void Attacking()
    {
        if (mNav.remainingDistance <= boxRange)
        {
            SwitchATKStatus(AttackStatus.boxing);
            return;
        }
        if (mNav.remainingDistance <= swingRange)
        {
            SwitchATKStatus(AttackStatus.swing);
            return;
        }
        SwitchATKStatus(AttackStatus.walk);
    }
    
    void SwitchATKStatus(AttackStatus _status)
    {
        if (mAtkStatus == _status)
            return;
        mAtkStatus = _status;
        mAnim.SetBool("InBoxRange", false);
        mAnim.SetBool("InSwingRange", false);
        switch (mAtkStatus)
        {
            case AttackStatus.boxing:
                weapon.SetActive(false);
                mAnim.SetBool("InBoxRange", true);
                break;
            case AttackStatus.swing:
                weapon.SetActive(true);
                mAnim.SetBool("InSwingRange", true);
                break;
            default:
                break;
        }
    }
}
