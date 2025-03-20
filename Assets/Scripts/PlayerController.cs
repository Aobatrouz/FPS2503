using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour, IDamage, IPickup
{

    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] CharacterController controller;

    [Range(1, 10)][SerializeField] public int HP;
    [Range(2, 5) ] [SerializeField] int speed;
    [Range(2, 4)] [SerializeField] int sprintMod;
    [Range(5, 20)] [SerializeField] int jumpSpeed;
    [Range(1, 3)] [SerializeField] int jumpMax;
    [Range(15, 45)][SerializeField] int gravity;

    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;

    int jumpCount;
    int HPOrig;
    int gunListPos;

    float shootTimer;

    Vector3 moveDir;
    Vector3 playerVelocity;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        updatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        if(!gamemanager.instance.isPaused)
            Movement();

        sprint();
    }
    void Movement()
    {
        shootTimer += Time.deltaTime;


        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVelocity = Vector3.zero;
        }
        // moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        // transform.position += moveDir * speed * Time.deltaTime;

        moveDir = (Input.GetAxis("Horizontal") * transform.right) +
                  (Input.GetAxis("Vertical") * transform.forward);

        controller.Move(moveDir * speed * Time.deltaTime);


        jump();

        controller.Move(playerVelocity * Time.deltaTime);
        playerVelocity.y -= gravity * Time.deltaTime;

        if (Input.GetButton("Fire1") && gunList.Count > 0 && gunList[gunListPos].ammoCur > 0 && shootTimer >= shootRate)
            shoot();


        selectGun();
        reloadGun();

    }


    void jump()
    {
        if(Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVelocity.y = jumpSpeed;
        }
    }
    void sprint()
    {
        if(Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
        }
    }

    void shoot()
    {
        shootTimer = 0;

        gunList[gunListPos].ammoCur--;

        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);
            Instantiate(gunList[gunListPos].hitEffect, hit.point, Quaternion.identity);

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if(dmg != null)
            {
                dmg.TakeDamage(shootDamage);
            }

        }
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();
        StartCoroutine(flashDamageScreen());

        if (HP <= 0)
        {
            gamemanager.instance.YouLose();
        }
    }

    IEnumerator flashDamageScreen()
    {
        gamemanager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gamemanager.instance.playerDamageScreen.SetActive(false);

    }

    public void updatePlayerUI()
    {
        gamemanager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }

    public void getGunStats(gunStats gun)
    {
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;
        changeGun();

    }

    void selectGun()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1)
        {
            gunListPos++;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
        {
            gunListPos--;
            changeGun();
        }
    }

    void changeGun()
    {
        shootDamage = gunList[gunListPos].shootDamage;
        shootDist = gunList[gunListPos].shootDist;
        shootRate = gunList[gunListPos].shootRate;
        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].model.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void reloadGun()
    {
        if(Input.GetButtonDown("Reload"))
        {
            gunList[gunListPos].ammoCur = gunList[gunListPos].ammoMax;
        }
    }
}
