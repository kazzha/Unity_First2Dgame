using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public PlayerMove player;
    public GameObject[] stages;

    public Image[] UIhealth;
    public TextMeshProUGUI UIPoint;
    public TextMeshProUGUI UIStage;
    public GameObject RestartBtn;
    public int health;

    void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();   
    }
    public void NextStage()
    {
        //Change Stage
        if (stageIndex < stages.Length-1)
        {
            stages[stageIndex].SetActive(false);
            stageIndex++;
            stages[stageIndex].SetActive(true);
            PlayerReposition();
            totalPoint += 700;

            UIStage.text = "STAGE " + (stageIndex+1);
        }
        else
        {
            //Game Clear
            //Player Control Lock
            Time.timeScale = 0;
            //Result UI
            Debug.Log("게임 클리어!");
            //Restart Button UI
        }

        //Calculate Point
        totalPoint += stagePoint;
        stagePoint= 0;
    }

    public void HealthDown()
    {
        if (health > 1) { 
            health--;
            UIhealth[health].color = new Color(1, 1, 1, 0.4f);
         }
        else 
        {
            health--;
            //All Health UI Off
            UIhealth[0].color = new Color(1, 1, 1, 0.4f);

            // Player Die Effect
            player.OnDie();

            //Retry Button UI
            RestartBtn.SetActive(true);
            Text btnText= RestartBtn.GetComponentInChildren<Text>();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //Player Reposition
            if (health > 1)
            {
                PlayerReposition();
             }

            //Health Down
            HealthDown();
           
        }
    }
    void PlayerReposition()
    {
        player.transform.position = new Vector3(-7, 3, -1);
        player.VelocityZero();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
        stagePoint = 0;
    }
}
