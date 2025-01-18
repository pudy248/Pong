using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    public Vector2 up;
    public int player;
    public float radius;

    public Vector2 pos;

    // Computes delay to intercept and predicted position of impact.
    public (float, float) Intercept(Ball b)
    {
        float sign = up.x != 0 ? up.x : up.y;
        float bx = up.x != 0 ? b.pos.x : b.pos.y;
        float v = up.x != 0 ? b.vel.x : b.vel.y;
        float x = up.x != 0 ? pos.x : pos.y;

		float t = (x - bx) / v;

        Vector2 intercept_pos = b.PredictPosition(t).Item1;

        float y = up.x != 0 ? intercept_pos.y - pos.y : intercept_pos.x - pos.x;
        return (t, y / radius * sign);
	}

    public void Initialize()
    {
        // Kind of odd to have a whole function to do so little, but these don't need much init
        if (up.y != 0)
            pos.x = 0;
        else pos.y = 0;
    }


    void Update()
    {
        if (Manager.instance.running)
        {
            if (player == 0 && Manager.instance.input == 0)
            {
                pos.x += Input.GetAxis("Mouse X") * 0.25f;
            }
            else if (Manager.instance.input == 1)
            {
                // There is a bug: Holding 2 of the same direction key at the same time in singleplayer
                //  makes you go twice as fast. I think it's neat so it's staying in.
                if (player == 0 || player == 1)
                {
                    if (Input.GetKey(KeyCode.A))
                        pos.x -= Time.deltaTime * Manager.instance.paddle_speed;
                    if (Input.GetKey(KeyCode.D))
                        pos.x += Time.deltaTime * Manager.instance.paddle_speed;
                }
                if (player == 2)
                {
					if (Input.GetKey(KeyCode.LeftArrow))
						pos.x -= Time.deltaTime * Manager.instance.paddle_speed;
					if ( Input.GetKey(KeyCode.RightArrow))
						pos.x += Time.deltaTime * Manager.instance.paddle_speed;
				}
            }

            pos.x = Mathf.Max(pos.x, Manager.instance.play_field.xMin + radius + Manager.instance.paddle_border);
            pos.x = Mathf.Min(pos.x, Manager.instance.play_field.xMax - radius - Manager.instance.paddle_border);

			transform.localScale = new(2 * radius, transform.localScale.y, 1);
            transform.position = pos;
        }
	}
}
