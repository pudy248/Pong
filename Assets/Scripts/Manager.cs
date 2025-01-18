using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// Questionable global class to do lots of things
// Generally a bad idea, but Pong is small enough for it to be fine.
// In a more procedural language like C++ these would all just be
//  plain functions and global variables, but C# is C#.
public class Manager : MonoBehaviour
{
	public static Manager instance;

	public bool running;
	public bool paused;

	public Rect play_field;

	public int[] score = new int[2];

	public int input;
	public int gamemode;
	public int difficulty;

	public float ball_speed_mult;
	public float paddle_speed;
	public float paddle_border;

	public float[] difficulty_ball_speed;
	public float[] difficulty_paddle_radius;
	public float[] difficulty_paddle_speed;
	public float[] difficulty_paddle_border;

	private void Awake()
	{
		instance = this;
	}

	// Some of this stuff would do better being stored in local variables once instead of having
	//  to do strange ECS tree traversal. Anyway, I already wrote it like this, so tech debt is
	//  actually meant to be an accurate simulation of large enterprise development and not
	//  just because there's partial differential equations homework to do.
	// GetChild should really only ever be called once per object in the tree
	private void Start()
	{
		transform.GetChild(0).GetChild(3).GetComponent<Button>().onClick.AddListener(StartGame);
		transform.GetChild(0).GetChild(4).GetComponent<Button>().onClick.AddListener(Application.Quit);
		transform.GetChild(0).GetChild(1).GetComponent<TMP_Dropdown>().onValueChanged.AddListener(
			gm => { if (gm == 1) transform.GetChild(0).GetChild(0).GetComponent<TMP_Dropdown>().value = 1; }
		);
		SwitchUI();
	}

	public void SwitchUI()
	{
		if (!paused)
		{
			foreach (Ball b in FindObjectsOfType<Ball>(true))
				b.gameObject.SetActive(true);
			transform.GetChild(1).gameObject.SetActive(true);
			transform.GetChild(2).gameObject.SetActive(gamemode > 0);
			for (int i = 0; i < 6; i++)
				transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
			transform.GetChild(0).GetChild(6).gameObject.SetActive(!running);
		}
		else
		{
			foreach (Ball b in FindObjectsOfType<Ball>())
				b.gameObject.SetActive(false);
			foreach (Paddle p in FindObjectsOfType<Paddle>())
				p.gameObject.SetActive(false);
			for (int i = 0; i < 6; i++)
				transform.GetChild(0).GetChild(i).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(6).gameObject.SetActive(false);
		}
	}

	public void StartGame()
	{
		input = transform.GetChild(0).GetChild(0).GetComponent<TMP_Dropdown>().value;
		gamemode = transform.GetChild(0).GetChild(1).GetComponent<TMP_Dropdown>().value;
		difficulty = transform.GetChild(0).GetChild(2).GetComponent<TMP_Dropdown>().value;
		transform.GetChild(0).GetChild(8).gameObject.SetActive(gamemode > 0);

		Paddle[] paddles = new Paddle[] { transform.GetChild(1).GetComponent<Paddle>(), transform.GetChild(2).GetComponent<Paddle>() };

		paddle_speed = difficulty_paddle_speed[difficulty];
		paddle_border = difficulty_paddle_border[difficulty];
		paddles[0].radius = difficulty_paddle_radius[difficulty];
		if (gamemode == 1) paddles[1].radius = difficulty_paddle_radius[difficulty];
		else paddles[1].radius = 1;
		foreach (Paddle p in paddles)
			p.Initialize();

		ResetBalls(true);

		// This is sort of nonsense in terms of readability
		if (gamemode == 0)
		{
			paddles[0].player = 0;
		}
		else if (gamemode == 1)
		{
			paddles[0].player = 1;
			paddles[1].player = 2;
		}
		else
		{
			// Player ID 3 is bot (unimplemented).
			paddles[0].player = 0;
			paddles[1].player = 3;
		}

		score[0] = 0;
		score[1] = 0;
		running = true;
		paused = false;
		SwitchUI();
	}

	public void ResetBalls(bool destroy)
	{
		Ball[] bs = FindObjectsOfType<Ball>(true);
		bs[0].Initialize(difficulty_ball_speed[difficulty]);
		for (int i = 1; i < bs.Length; i++) {
			if (destroy) Destroy(bs[i].gameObject);
			else bs[i].Initialize(difficulty_ball_speed[difficulty]);
		}
	}

	void GameUpdate()
	{
		foreach (Ball b in FindObjectsOfType<Ball>())
		{
			float dt = Time.deltaTime;
			foreach (Paddle p in FindObjectsOfType<Paddle>())
			{
				// Don't intersect from behind (well, this doesn't normally happen when the death conditions are enabled but still)
				if (Vector2.Dot(p.up, b.vel) > 0) continue;
				var (t, x) = p.Intercept(b);
				// Collision is happening this frame and actually did hit the paddle
				// Remember, Intercept just gets the relative position analytically and it could be off the screen, let alone off the paddle.
				if (t > 0 && t < dt && Mathf.Abs(x) < 1)
				{
					b.Step(t);
					dt -= t;
					// Like in the original pong, we don't reflect, but change angle based on where on the paddle
					//  the ball hit, this adds a bit more variety.
					float magn = b.vel.magnitude * ball_speed_mult;
					const float MAX_ANGLE_RAD = 0.33f; // Max angle isn't horizontal, that turns out to be problematic
					float theta = x * Mathf.PI * MAX_ANGLE_RAD; 
					b.vel = magn * new Vector2(p.up.x * Mathf.Cos(theta) + p.up.y * Mathf.Sin(theta), p.up.y * Mathf.Cos(theta) - p.up.x * Mathf.Sin(theta));
					if (gamemode == 0)
						score[0]++;
				}
			}

			// Edge collisions (still nonsense control flow)
			if (b.pos.y + b.vel.y * dt < play_field.yMin)
			{
				if (gamemode == 0)
				{
					running = false;
					SwitchUI();
				}
				else
				{
					score[0]++;
					ResetBalls(false);
				}
			}
			if (b.pos.y + b.vel.y * dt > play_field.yMax && gamemode > 0)
			{
				score[1]++;
				ResetBalls(false);
			}

			b.Step(dt);
		}
	}


	void Update()
	{
		// You can resize the game while it's running!
		// The fact that you can get infinite score by making the window narrower than the paddle can be considered an "easter egg".
		Vector3 top_left = GetComponent<Camera>().ViewportToWorldPoint(new(0, 0, GetComponent<Camera>().nearClipPlane));
		Vector3 bottom_right = GetComponent<Camera>().ViewportToWorldPoint(new(1, 1, GetComponent<Camera>().nearClipPlane));
		play_field = new(top_left, bottom_right - top_left);

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			paused = !paused;
			SwitchUI();
		}
		if (!paused && Input.GetKeyDown(KeyCode.R))
		{
			StartGame();
		}
		if (running && Input.GetKeyDown(KeyCode.B) && Input.GetKey(KeyCode.LeftShift))
		{
			GameObject g = Instantiate(FindObjectsOfType<Ball>()[0].gameObject);
			g.GetComponent<Ball>().Initialize(difficulty_ball_speed[difficulty]);
			g.name = "Ball";
		}

		if (running && !paused) GameUpdate();

		Cursor.visible = !running || paused || input != 0;
		transform.GetChild(0).GetChild(7).GetComponent<TextMeshProUGUI>().text = $"SCORE: {score[0]}";
		transform.GetChild(0).GetChild(8).GetComponent<TextMeshProUGUI>().text = $"SCORE: {score[1]}";

	}
}
