using UnityEngine;
using System.Collections;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour, IDamage
{

    
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Transform headPos;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int animTranSpeed;
    [SerializeField] int FOV;
    [SerializeField] int roamPauseTime;
    [SerializeField] int roamDist;

    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    float shootTimer;
    float roamTimer;
    float angleToPlayer;
    float stoppingDist;


    Vector3 playerDir;
    Vector3 startingPos;

    bool playerInRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gamemanager.instance.UpdateGameGoal(1);
        startingPos = transform.position;
        stoppingDist = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        setAnimLocomotion();


        shootTimer += Time.deltaTime;

        if (agent.remainingDistance < 0.01f)
            roamTimer += Time.deltaTime;

        if (playerInRange && !canSeePlayer())
        {
            checkRoam();
        }
         else if (!playerInRange)
         {
            checkRoam();
        }

    }

    bool canSeePlayer()
    {
        playerDir = gamemanager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {

            if (hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
            {
                agent.SetDestination(gamemanager.instance.player.transform.position);

                if (shootTimer >= shootRate)
                {
                    Shoot();
                }

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    FaceTarget();
                }
                return true;
            }

        }
        agent.stoppingDistance = 0;
        return false;
    }

    void checkRoam()
    {
        if ((roamTimer > roamPauseTime && agent.remainingDistance < 0.01f) || gamemanager.instance.playerScript.HP <= 0)
        {
            roam();
        }
    }

    void roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;

        Vector3 ranPos = Random.insideUnitSphere * roamDist;
        ranPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);
    }

    private void setAnimLocomotion()
    {
        float agentSpeed = agent.velocity.normalized.magnitude;
       float animSpeedCur = anim.GetFloat("Speed");
       anim.SetFloat("Speed", Mathf.Lerp(animSpeedCur, agentSpeed, Time.deltaTime * animTranSpeed));
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            {
                playerInRange = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            {
                playerInRange = false;
                agent.stoppingDistance = 0;
            }
        }
    }
  

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(FlashRed());
        roamTimer = 0;

        agent.SetDestination(gamemanager.instance.player.transform.position);

        if (HP <= 0)
        {
            gamemanager.instance.UpdateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }

    void Shoot()
    {
        shootTimer = 0;
        Instantiate(bullet, shootPos.position, transform.rotation);
    }
}
