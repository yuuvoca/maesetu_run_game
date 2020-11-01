using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{

	[SerializeField]
	string jumpButtonName = "Jump";
	[SerializeField]
	float speed = 0;
	[SerializeField]
	float jumpPower = 30;
	[SerializeField]
	Transform[] groundCheckTransforms = null;

	bool isActive = false;
	bool jump = false;
	bool isGrounded = false;
	Animator animator;
	SpriteRenderer spriteRenderer;
	Rigidbody2D rigidBody2D;

	void Start()
	{
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		rigidBody2D = GetComponent<Rigidbody2D>();

		isActive = true;
	}

	void Update()
	{
		GetInput();
		UpdateAnimation();
	}

	void FixedUpdate()
	{
		Move();
	}

	void GetInput()
	{
		if (!isActive)
		{
			return;
		}

		jump = Input.GetButtonDown(jumpButtonName);
	}

	void Move()
	{
		if (!isActive)
		{
			return;
		}

		//接地判定
		GroundCheck();

		//ジャンプの速度計算
		if (jump && isGrounded)
		{
			rigidBody2D.velocity = Vector3.up * jumpPower;
		}

		//実際の移動処理
		rigidBody2D.velocity = new Vector2(speed, rigidBody2D.velocity.y);
	}

	void GroundCheck()
	{
		Collider2D[] groundCheckCollider = new Collider2D[groundCheckTransforms.Length];

		//接地判定オブジェクトが何かに重なっているかどうかをチェック
		for (int i = 0; i < groundCheckTransforms.Length; i++)
		{
			groundCheckCollider[i] = Physics2D.OverlapPoint(groundCheckTransforms[i].position);

			//接地判定オブジェクトのうち、1つでも何かに重なっていたら接地しているものとして終了
			if (groundCheckCollider[i] != null)
			{
				isGrounded = true;
				return;
			}
		}

		//ここまできたということは何も重なっていないということなので、接地していないと判断する
		isGrounded = false;
	}

	void UpdateAnimation()
	{
		animator.SetBool("Grounded", isGrounded);
	}

}
