using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBombCanal : MonoBehaviour, IBombTrigger
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
      
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
       
    }

    public IEnumerator OnTriggerStay2D(Collider2D collision)
    {
        Bomb bomb = collision.GetComponent<Bomb>();

        if (bomb)
        {
            if (bomb.isOnGround && !bomb.bombDefused)
            {
                Debug.Log("Transition to level");
            }
        }

        yield return null;
    }
}
