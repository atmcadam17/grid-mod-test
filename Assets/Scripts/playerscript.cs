using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.SceneManagement;

public class playerscript : MonoBehaviour
{
    public Vector2 playerPos;
    public gridmaker gridMaker;

    private bool gameOver;

    private bool playtestMode;
    public bool gameStart;
    private static int counter = 1;
    
    void Start()
    {
        gridMaker = GameObject.Find("GameManager").GetComponent<gridmaker>();
        playerPos = gridMaker.playerPos;
        gameOver = false;
    }

    void Update()
    {
        
        if (gameOver == false && !gridMaker.repopulate)
        {
            // end game
            if (counter == 0 && playtestMode == false)
            {
                gameOver = true;
                SceneManager.LoadScene("GameOver");
            }
            
            // swap movement
            if (Input.GetKeyDown(KeyCode.W))
            {
                Swap(0,-1);
                gameStart = true;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                Swap(1,0);
                gameObject.transform.localScale = new Vector3(-1, 1, 0);
                gameStart = true;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Swap(0,1);
                gameStart = true;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                Swap(-1,0);
                gameObject.transform.localScale = new Vector3(1, 1, 0);
                gameStart = true;
            }
            
            // chomp
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                gameObject.GetComponent<Animator>().SetTrigger("chompUp");
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                gameObject.transform.localScale = new Vector3(-1, 1, 0);
                gameObject.GetComponent<Animator>().SetTrigger("chompSide");
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                gameObject.GetComponent<Animator>().SetTrigger("chompDown");
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                gameObject.transform.localScale = new Vector3(1, 1, 0);
                gameObject.GetComponent<Animator>().SetTrigger("chompSide");
            }
        }

        if (Input.GetKeyDown(KeyCode.Z) && playtestMode == false)
        {
            playtestMode = true;
            Debug.Log("test mode toggled");
        } else if (Input.GetKeyDown(KeyCode.Z) && playtestMode == true)
        {
            playtestMode = false;
            Debug.Log("test mode off");
        }
    }

    void Swap(int x, int y)
    {
        Vector2 oldLocation = new Vector2(playerPos.x, playerPos.y);
        Vector2 newLocation = new Vector2(playerPos.x + x, playerPos.y + y);

        if ((int) newLocation.x < gridmaker.WIDTH &&
            (int) newLocation.x >= 0 &&
            (int) newLocation.y < gridmaker.HEIGHT &&
            (int) newLocation.y >= 0)
        {
            // put tile in place
            GameObject swapTile = gridMaker.tiles[(int) newLocation.x, (int) newLocation.y];
            // remember where tile is
            Vector3 swapPosition = swapTile.transform.localPosition;

            // move player
            // set tile to player position or something
            swapTile.transform.localPosition = transform.localPosition;
            transform.localPosition = swapPosition;

            // set spot in array
            gridMaker.tiles[(int) oldLocation.x, (int) oldLocation.y] = swapTile;
            gridMaker.tiles[(int) newLocation.x, (int) newLocation.y] = gameObject;

            // move player to new position
            playerPos = newLocation;
        }
    }
}