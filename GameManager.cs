using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public PlayerMove player;
    public int health;

    public void NextStage()
    {
        stageIndex++;
        totalPoint += stagePoint;
        stagePoint= 0;
    }

    public void HealthDown()
    {
        if (health > 1)
            health--;
        else
        {
            health--;
            // Player Die Effect
            player.OnDie();

            // Result UI
            Debug.Log("ав╬З╫ю╢о╢ы!");
            //Retry Button UI
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //Player Reposition
            if (health > 1)
            {
                collision.attachedRigidbody.velocity = Vector2.zero;
                collision.transform.position = new Vector3(-2, 3, -1);
             }

            //Health Down
            HealthDown();
           
        }
    }
}
