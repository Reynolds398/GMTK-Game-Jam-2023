using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Will not work if not attached to a trigger game object with a character parent
//Handles updating the current jump count and checking if touching the floor
public class GroundChecker : MonoBehaviour
{
    [HideInInspector]
    public int curJump = 0;
    [HideInInspector]
    public bool ableToJump = false;
    [HideInInspector]
    public bool touchingGround = false;

    private int maxJump = 0;

    // Start is called before the first frame update
    void Start()
    {
       //Set max jump to the value set in parent
       maxJump = GetComponentInParent<PlayerControl>().jumpCount;
       curJump = maxJump;
    }

    // Update is called once per frame
    void Update()
    {
        //Keeps updating max jump to take into account of max jump change midgame
        maxJump = GetComponentInParent<PlayerControl>().jumpCount;

        if (curJump <= 0) //If cur jump 0, set jump to false
        {
            ableToJump = false;
        }
        else //else jump is true
        {
            ableToJump = true;
        }

        Debug.Log("curJump value: " + curJump);
        Debug.Log("ableToJump bool value: " + ableToJump);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Jumpable")
        {
            curJump = maxJump;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("OnTriggerStay triggered");

        if (collision.tag == "Jumpable")
        {
            curJump = maxJump;
            touchingGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Jumpable")
        {
            touchingGround = false;
        }
    }
}
