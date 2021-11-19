using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatyBox : MonoBehaviour
{

    public float moveForce; // how fast to move

    public float jumpForce;
    public float fallForce;
    
    public LayerMask groundLayer;
    public Transform feet;

    private bool grounded = false;  // touching ground?
    private bool seenJump = false;  // indicates whether a jump has happened - used to avoid multiple jump touch strokes during liftoff
    private bool fallGrace = false; // grace period before allowing fall

    Rigidbody2D _rigidbody;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        for (int i = 0; i < Input.touchCount; ++i)
        {
            // get ground collision
            grounded = Physics2D.OverlapCircle(feet.position, .3f, groundLayer);

            Touch touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Moved)
            {
                if (Mathf.Abs(touch.deltaPosition.y) > Mathf.Abs(touch.deltaPosition.x))
                {
                    // jump (1) or fall (0)
                    bool sign = (touch.deltaPosition.y > 0);
                    if (sign && !seenJump && grounded)
                    {
                        // jump or fall
                        _rigidbody.AddForce(Vector2.up * jumpForce);
                        //touch.phase = TouchPhase.Canceled;
                        seenJump = true;
                        StartCoroutine(resetJump(1f));
                    }
                    if (!fallGrace && !sign && !grounded)
                    {
                        _rigidbody.AddForce(Vector2.down * jumpForce);
                        fallGrace = true;
                        StartCoroutine(resetFall(.2f));
                    }
                }
                else
                {
                    // move
                    int sign = (touch.deltaPosition.x > 0) ? 1 : -1;
                    _rigidbody.AddForce(sign * Vector2.right * moveForce);
                }
            }
        }
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
