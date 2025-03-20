using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class gamemanager : MonoBehaviour
{

    public static gamemanager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLoss;
    [SerializeField] TMP_Text goalCountText;

    public Image playerHPBar;
    public GameObject playerDamageScreen;
    public GameObject player;
    public PlayerController playerScript;

    public bool isPaused;

    int goalCount;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                StatePause();
                menuActive = menuPause;
                menuActive.SetActive(true);

            }
            else if (menuActive == menuPause)
            {
                StateUnpause();
            }

        }

    }

    public void StatePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void UpdateGameGoal(int amount)
    {
        goalCount += amount;
        goalCountText.text = goalCount.ToString("F0");
        if (goalCount <= 0)
        {
            // you win!!
            StatePause();
            menuActive = menuWin;
            menuActive.SetActive(true);


        }
    }

    public void YouLose()
    {
        StatePause();
        menuActive = menuLoss;
        menuActive.SetActive(true);
    }
}
