using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
	public Vector2 pos = new();
	public Vector2 vel;

	// Simple analytic reflection, some branches could be eliminated but it's pong, we're not exactly compute-limited
	static (float, float) bounce(float x, float x0, float xmax, float v, float dt)
	{
		float relativePos = (x - x0 + v * dt) / xmax;
		float numReflections = Mathf.Floor(relativePos);
		float reflectionParity = Mathf.Abs(numReflections) % 2;
		float nextRelativePos = reflectionParity == 1 ? 1 - (relativePos - numReflections) : relativePos - numReflections;
		float nextPos = nextRelativePos * xmax + x0;
		float nextVel = v * (reflectionParity * -2 + 1);
		return (nextPos, nextVel);
	}

	// Used for both AI and actual updates.
	// Update: AI is out of scope but it would be useful for Pong 2.
	public (Vector2, Vector2) PredictPosition(float dt)
	{
		var (nextX, nextVX) = bounce(pos.x, Manager.instance.play_field.xMin, Manager.instance.play_field.width, vel.x, dt);
		var (nextY, nextVY) = bounce(pos.y, Manager.instance.play_field.yMin, Manager.instance.play_field.height, vel.y, dt);

		return (new(nextX, nextY), new(nextVX, nextVY));
	}

	public void Initialize(float speed)
	{
		pos = new();
		float theta = Random.value * 2 * Mathf.PI;
		vel = speed * new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
	}

	public void Step(float dt)
	{
		(pos, vel) = PredictPosition(dt);
		transform.position = pos;
	}
}
