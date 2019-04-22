using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SocialPlatforms.GameCenter;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class gridmaker : MonoBehaviour
{
    public const int WIDTH = 5;
    public const int HEIGHT = 7;

    private float xOffset = WIDTH / 2f - .5f;
    private float yOffset = HEIGHT / 2f - .5f;

    public GameObject[,] tiles;
    public GameObject tilePrefab;
    
    public GameObject playerPrefab;
    private GameObject player;
    public Vector2Int playerPos;
    private playerscript playerScript;

    public bool repopulate;

    GameObject gridHolder;

    public static float slideLerp = -1;
    public float lerpSpeed = .8f;

    private TextMesh scoreText;
    public int score;
    public int goalScore = 50;

    private TextMesh timerText;
    public float timer;

    public AudioClip matchSound;
    
    public GameObject particles;

    void Start()
    {
        int level = GameObject.Find("GameManager").GetComponent<GameManager>().level;
        scoreText = GameObject.Find("Score").GetComponent<TextMesh>();
        tiles = new GameObject[WIDTH,HEIGHT];
        gridHolder = new GameObject();
        gridHolder.transform.position = new Vector3(-1f, -.5f, 0);
        timerText = GameObject.Find("Timer").GetComponent<TextMesh>();
        timer = 60 - ((level - 1) * 5);
        score = 0;
        goalScore = 50 + ((level - 1) * 5);

        createGrid();
        
        // spawn player
        GameObject centerTile = tiles[WIDTH / 2, HEIGHT / 2];
        player = Instantiate(playerPrefab);
        
        player.transform.parent = gridHolder.transform;
        player.transform.localPosition = centerTile.transform.localPosition;
        
        Destroy(centerTile);
        
        playerPos = new Vector2Int(WIDTH / 2, HEIGHT / 2);
        
        tiles[WIDTH / 2, HEIGHT / 2] = player;
        playerScript = player.GetComponent<playerscript>();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        timerText.text = "TIME LEFT: " + Mathf.RoundToInt(timer).ToString();
        scoreText.text = "SCORE: " + score.ToString() + " / " + goalScore.ToString();
        
        
        // reset game for testing
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("MainGame");
        }
        
        if (slideLerp < 0 && !Repopulate() && checkMatch())
        {
            removeMatches();
        } else if (slideLerp >= 0)
        {
            slideLerp += Time.deltaTime / lerpSpeed;

            if (slideLerp >= 1)
            {
                slideLerp = -1;
            }
        }

        if (score >= goalScore)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().level++;
            SceneManager.LoadScene("textScene");
        }
    }

    public tilescript checkMatch()
    {
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                tilescript tileScript = tiles[x, y].GetComponent<tilescript>();

                if (tileScript != null)
                {
                    // check horizontal match
                    if (x < WIDTH - 2 && tileScript.isMatch(tiles[x + 1, y], tiles[x+2, y]))
                    {
                        return tileScript;
                    }

                    // check vertical match
                    if (y < HEIGHT - 2 && tileScript.isMatch(tiles[x, y + 1], tiles[x, y + 2]))
                    {
                        return tileScript;
                    }
                }
            }
        }

        return null;
    }

    public void createGrid()
    {
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                // make the boy
                GameObject newTile = Instantiate(tilePrefab);
                
                // get the spot for the boy
                newTile.transform.parent = gridHolder.transform;
                newTile.transform.localPosition = new Vector2(WIDTH - x - xOffset, HEIGHT - y - yOffset);

                // place the boy in the spot
                tiles[x, y] = newTile;
                
                // sprite stuff
                tilescript tileScript = newTile.GetComponent<tilescript>();
                tileScript.SetSprite(Random.Range(0, tileScript.tileColors.Length));
            }
        }
    }

    public void removeMatches()
    {
        
        // delete tiles
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                tilescript tileScript = tiles[x, y].GetComponent<tilescript>();
                
                if (tileScript != null)
                {
                    // check horizontal match
                    if (x < WIDTH - 2 && tileScript.isMatch(tiles[x + 1, y], tiles[x+2, y]))
                    {
                        // emit particles + increase score
                        Instantiate(particles, tiles[x, y].transform.position, Quaternion.identity);
                        Instantiate(particles, tiles[x+1, y].transform.position, Quaternion.identity);
                        Instantiate(particles, tiles[x+2, y].transform.position, Quaternion.identity);
                        
                        Destroy(tiles[x,y]);
                        Destroy(tiles[x + 1,y]);
                        Destroy(tiles[x + 2,y]);

                        var source = GameObject.Find("GameManager").GetComponent<AudioSource>();
                        source.PlayOneShot(matchSound, .4f);

                        if (playerScript.gameStart)
                        {
                            score += 3;
                        }
                    }

                    // check vertical match
                    if (y < HEIGHT - 2 && tileScript.isMatch(tiles[x, y + 1], tiles[x, y + 2]))
                    {
                        // emit particles + increase score
                        Instantiate(particles, tiles[x, y].transform.position, Quaternion.identity);
                        Instantiate(particles, tiles[x, y + 1].transform.position, Quaternion.identity);
                        Instantiate(particles, tiles[x, y + 2].transform.position, Quaternion.identity);
                        
                        Destroy(tiles[x,y]);
                        Destroy(tiles[x,y + 1]);
                        Destroy(tiles[x,y + 2]);

                        var source = GameObject.Find("GameManager").GetComponent<AudioSource>();
                        source.PlayOneShot(matchSound, .4f);

                        if (playerScript.gameStart)
                        {
                            score += 3;
                        }
                    }
                }
            }
        }
    }

    public bool Repopulate()
    {
        bool repop = false;
        
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                if (tiles[x, y] == null)
                {
                    repop = true;

                    // if empty space is in top row
                    if (y == 0)
                    {
                        tiles[x, y] = Instantiate(tilePrefab);
                        tilescript tileScript = tiles[x, y].GetComponent<tilescript>();
                        
                        tileScript.SetSprite(Random.Range(0, tileScript.tileColors.Length));
                        
                        tiles[x, y].transform.parent = gridHolder.transform;
                        tiles[x,y].transform.localPosition = new Vector2(WIDTH - x - xOffset, HEIGHT - y - yOffset);
                    }
                    else
                    {
                        slideLerp = 0;
                        tiles[x, y] = tiles[x, y - 1];
                        tilescript tileScript = tiles[x, y].GetComponent<tilescript>();
                        if (tileScript != null)
                        {
                            tileScript.SetupSlide(new Vector2(WIDTH - x - xOffset, HEIGHT - y - yOffset));
                        }

                        playerscript playerScript = tiles[x, y].GetComponent<playerscript>();
                        if (playerScript != null)
                        {
                            playerScript.playerPos.Set(x,y);
                        }

                        tiles[x, y - 1] = null;
                    }
                }
            }
        }

        repopulate = repop;
        
        return repop;
    }
}