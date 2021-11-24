using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatyBox : MonoBehaviour
{

    public int moveSpeed; // how fast to move

    public float jumpForce;
    public float fallForce;
    
    public LayerMask groundLayer;
    public Transform feet;

    public FloatingJoystick joystick;

    private bool grounded = false;  // touching ground?
    private bool seenJump = false;  // indicates whether a jump has happened - used to avoid multiple jump touch strokes during liftoff
    private bool fallGrace = false; // grace period before allowing fall

    Rigidbody2D _rigidbody;

    public GameObject spearPrefab;
    public int spearForce = 800;
    public Transform spearPos;

    public GameObject swordPrefab;
    public Transform swordPos;

    public bool weaponType = false;

    private bool cast = true;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // move
        float xSpeed = joystick.Horizontal * moveSpeed;
        _rigidbody.velocity = new Vector2(xSpeed, _rigidbody.velocity.y);

        for (int i = 0; i < Input.touchCount; ++i)
        {
            // get ground collision
            grounded = Physics2D.OverlapCircle(feet.position, .3f, groundLayer);

            Touch touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Moved && touch.position.x > Screen.width / 2)
            {
                // jump or fall
                if (Mathf.Abs(touch.deltaPosition.y) > Mathf.Abs(touch.deltaPosition.x))
                {
                    // jump (1) or fall (0)
                    bool sign = (touch.deltaPosition.y > 0);
                    if (sign && !seenJump && grounded)
                    {
                        // jump or fall
                        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpForce);
                        //_rigidbody.AddForce(Vector2.up * jumpForce);
                        //touch.phase = TouchPhase.Canceled;
                        seenJump = true;
                        StartCoroutine(resetJump(1f));
                    }
                    if (!fallGrace && !sign && !grounded)
                    {
                        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, -fallForce);
                        //_rigidbody.AddForce(Vector2.down * jumpForce);
                        fallGrace = true;
                        StartCoroutine(resetFall(.2f));
                    }
                }
                /*
                else
                {
                    // move
                    int sign = (touch.deltaPosition.x > 0) ? 1 : -1;
                    _rigidbody.AddForce(sign * Vector2.right * moveForce);
                }*/
            }
            else if (Input.touchCount == 1)
            {
                if (cast)
                {
                    if (weaponType == true)
                    {
                        GameObject newSword = Instantiate(swordPrefab, swordPos.position, Quaternion.identity);
                    }
                    else
                    {
                        GameObject newSpear = Instantiate(spearPrefab, spearPos.position, Quaternion.identity);
                        newSpear.GetComponent<Rigidbody2D>().AddForce(new Vector2(spearForce * transform.localScale.x, 0));
                    } 
                    cast = false;
                    //StartCoroutine(coolDown(2));
                }

            }
            else if (Input.touchCount == 2)
            {
                GameObject newSword = Instantiate(swordPrefab, swordPos.position, Quaternion.identity);
                newSword.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,0));
                    //newSpear.GetComponent<Rigidbody2D>().AddForce(new Vector2(swordForce * transform.localScale.x, 0));
                    //cast = false;
                    //StartCoroutine(coolDown(2));

            }
            /*
            else if (touch.phase == TouchPhase.Stationary)
            {
                if (cast)
                {
                    GameObject newSpear = Instantiate(spearPrefab, spearPos.position, Quaternion.identity);
                    newSpear.GetComponent<Rigidbody2D>().AddForce(new Vector2(spearForce * transform.localScale.x, 0));
                    cast = false;
                    //StartCoroutine(coolDown(2));
                }

            }*/
            /*
            StartCoroutine(coolDown(2));
            if (Input.GetButtonDown("Fire2"))
            {
                GameObject newSpear = Instantiate(spearPrefab, spearPos.position, Quaternion.identity);
                newSpear.GetComponent<Rigidbody2D>().AddForce(new Vector2(spearForce * transform.localScale.x, 0));
            }*/
        }
    }

    public IEnumerator coolDown(float time)
    {
        cast = false;
        yield return new WaitForSeconds(time);// * Time.deltaTime);
        cast = true;
    }

    // resets jump grace period
    public IEnumerator resetJump(float wait)
    {
        yield return new WaitForSeconds(wait);
        seenJump = false;
    }

    // resets fall grace period
    public IEnumerator resetFall(float wait)
    {
        yield return new WaitForSeconds(wait);
        fallGrace = false;
    }
}
