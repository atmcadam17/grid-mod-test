using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tilescript : MonoBehaviour
{
    public int type;
    public Sprite[] tileColors;

    public Vector3 startPosition;
    public Vector3 destPosition;
    public bool inSlide = false;
    
    public bool isMatch(GameObject gameObject1, GameObject gameObject2)
    {
        // ~ i like... don't understand this whole part really
        tilescript ts1 = gameObject1.GetComponent<tilescript>();
        tilescript ts2 = gameObject2.GetComponent<tilescript>();
        return ts1 != null && ts2 != null && type == ts1.type && type == ts2.type;
    }
    
    void Start()
    {
        if(gameObject.CompareTag("Player"))
        {
            type = -1;
        }
    }

    void Update()
    {
        if (inSlide)
        {
            if (gridmaker.slideLerp < 0)
            {
                transform.localPosition = destPosition;
                inSlide = false;
            }
            else
            {
                transform.localPosition = Vector3.Lerp(startPosition, destPosition, gridmaker.slideLerp);
            }
        }
    }

    public void SetSprite(int rand)
    {
        type = rand;
        GetComponent<SpriteRenderer>().sprite = tileColors[type];
    }

    public void SetupSlide(Vector2 newDesPos)
    {
        inSlide = true;
        startPosition = transform.localPosition;
        destPosition = newDesPos;
    }
}