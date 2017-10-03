using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
	private Animator animation;
	private PlayerMovement movement;

	[SerializeField] private AnimationClip[] _animations;
	private AnimationClip _lastClip;

	private AnimationClip CurrentClip
	{
		get { return _lastClip; }
	}
	
	private const float landDistance = 2f;
	
	private void Start()
	{
		animation = transform.GetChild(0).GetComponent<Animator>();
		movement = GetComponent<PlayerMovement>();

		SetAnimation("player_move");
	}

	private bool LandConditions()
	{
		float dist = movement.Velocity.y + -landDistance;
		if (dist > 0) dist = 0;
		
		return (Physics2D.Raycast(transform.position, Vector2.up, 
			       dist, movement.CollisionMask)
			&& CurrentClip.name == "player_jump");
	}

	public bool RunConditions()
	{
		return CurrentClip.name == "player_jumpend";
	}

	public bool JumpConditions()
	{
		return CurrentClip.name == "player_move";
	}

	public void SetAnimation(string clipname)
	{
		if(CurrentClip != null)
			if (CurrentClip.name == clipname) return;
		_lastClip = GetClip(clipname);
		animation.Play("Base Layer." + clipname);
	}

	private bool IsPlaying(string name)
	{
		return !animation.GetCurrentAnimatorStateInfo(0).IsName(name);
	}
	
	private AnimationClip GetClip(string clipname)
	{
		return _animations.FirstOrDefault(clip => clip.name == clipname);
	}

	private void Update()
	{
		if (LandConditions()) 
			SetAnimation("player_jumpend");

		if (CurrentClip == null) return;
		
		if (CurrentClip.name == "player_jumpend" 
		    && !IsPlaying("player_jumpend") && movement.Grounded)
			SetAnimation("player_move");

		animation.speed = CurrentClip.name == "player_move" ? movement.Velocity.x : 1;
	}
}
