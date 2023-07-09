using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathChecker : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Jumpable"))
        {
            //death
            GetComponentInParent<PlayerControl>().death = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Jumpable"))
        {
            //death
            GetComponentInParent<PlayerControl>().death = true;
        }
    }
}
