using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class textScene : MonoBehaviour
{
    [Header("Text")] [TextArea(2, 10)]
    public string[] quotes;
    public int activeQuote;

    private TextMesh textMesh;

    private TextMesh levelText;
    private int level;
    
    void Start()
    {
        activeQuote = Random.Range(0, quotes.Length);
        textMesh = gameObject.GetComponent<TextMesh>();
        
        levelText = GameObject.Find("Level").GetComponent<TextMesh>();
        level = GameObject.Find("GameManager").GetComponent<GameManager>().level;
        levelText.text = "Level " + level.ToString();
    }

    void Update()
    {
        textMesh.text = quotes[activeQuote];
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("MainGame");
        }
    }
}
