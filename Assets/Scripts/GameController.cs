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
    public bool vertical = false;
    private float playerPosX;
    private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        coroutine = FollowPlayer(delay);
        playerPosX = player.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("vertical is " + vertical);

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

        if (piece.difficulty >= 3) //Normal+
        {
            float curPlayerPosX = player.transform.position.x;

            //If player doesn't really move, rotate block to be vertical
            if (playerPosX - 1 <= curPlayerPosX && curPlayerPosX <= playerPosX + 1)
            {
                timer += Time.deltaTime;

                if (timer >= 2.0f && !vertical)
                {
                    timer -= 2.0f;

                    //Choose a random direction to rotate to
                    int random = Random.Range(0, 2);

                    if (random == 0)
                    {
                        piece.RotateLeft();
                    }
                    else
                    {
                        piece.RotateRight();
                    }

                    vertical = true;
                }
            }
            else
            {
                timer = 0;

                if (vertical)
                {
                    //Choose a random direction to rotate to
                    int random = Random.Range(0, 2);

                    if (random == 0)
                    {
                        piece.RotateLeft();
                    }
                    else
                    {
                        piece.RotateRight();
                    }

                    vertical = false;
                }

                playerPosX = curPlayerPosX;
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
