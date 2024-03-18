using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    float SPEED = 1f;

    [SerializeField]
    int parent;

    [SerializeField]
    int val;

    BoxCollider2D bc;

    bool isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position != transform.parent.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.parent.position, SPEED);
            isMoving = true;
        } else
        {
            isMoving = false;
        }
    }

    public int getVal()
    {
        return val;
    }

    public void setParent(int index)
    {
        parent = index;
    }

    public Sprite getSprite()
    {
        return this.GetComponent<SpriteRenderer>().sprite;
    }

    bool SameGem( Gem other)
    {
        return other.getVal() == this.val;  
    }

    public bool getMoving()
    {
        return isMoving;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "BackTile")
        {
            if (collision.gameObject.GetComponent<gameTile>().getIndex() == parent)
            {
                print("found parent");
                GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            }
        }
    }
}
