using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Pieces piece;
    public GameObject player;

    private IEnumerator coroutine;

    private bool followOn = false;
    private float delay = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        coroutine = FollowPlayer(delay);
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<PlayerControl>().death)
        {
            StopAllCoroutines();
            Debug.Log("Coroutines Stopped!");
        }

        if (piece.difficulty >= 2) //Easy+
        {
            //Follow player with 3 second delay
            if (!followOn)
            {
                StartCoroutine(coroutine);
                Debug.Log("Coroutine Start!");
                followOn = true;

                //Soft drop at halfway point
                piece.TurnOnSoftDrop();
            }
        }
    }

    private IEnumerator FollowPlayer(float waitTime)
    {

        while (true)
        {

            yield return new WaitForSeconds(waitTime);

            float diff = player.transform.position.x - piece.position.x;
            int diffInt = Mathf.RoundToInt(diff);

            Debug.Log("diffInt is " + diffInt);

            if (diffInt < 0) //If negative
            {
                diffInt *= -1; //Make it positive

                //Move left equal to the difference to player
                piece.MoveLeft(diffInt);
            }
            else //If positive
            {
                //Move right equal to the difference to player
                piece.MoveRight(diffInt);
            }
        }
        
    }
}
