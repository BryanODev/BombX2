using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IBombTrigger 
{
    public void OnTriggerEnter2D(Collider2D collision);
    public IEnumerator OnTriggerStay2D(Collider2D collision);
    public void OnTriggerExit2D(Collider2D collision);
}

public class BombCanal : MonoBehaviour, IBombTrigger
{
    public int bombID;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    public IEnumerator OnTriggerStay2D(Collider2D collision)
    {
        Bomb bomb = collision.GetComponent<Bomb>();
        
        if (bomb) 
        {
            if (bomb.isOnGround && !bomb.bombDefused)
            {
                OnBombEnter(bomb);
            }
        }
         
        yield return null;
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
       
    }


    public virtual void OnBombEnter(Bomb bomb) 
    {
       
        bomb.canBeSelected = false;

        if (bomb.bombID == bombID)
        {
            bomb.DefuseBomb();
        }
        else
        {
            Debug.Log("Game Over!");
            bomb.Explode();
        }

    }


}
