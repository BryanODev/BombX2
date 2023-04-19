using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombCanal : MonoBehaviour
{
    public int bombID;

    private IEnumerator OnTriggerStay2D(Collider2D collision)
    {
        Bomb bomb = collision.GetComponent<Bomb>();
        
        if (bomb) 
        {
            if (bomb.isOnGround) 
            {
                bomb.canBeSelected = false;

                if (bomb.bombID == bombID)
                {
                    Debug.Log("+1");
                    bomb.DefuseBomb();
                }
                else
                {
                    Debug.Log("Game Over!");
                    bomb.Explode();
                }
            }
        }

        yield return null;
    }
}
