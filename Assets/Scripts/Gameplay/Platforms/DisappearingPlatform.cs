using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingPlatform : Platform {
	public float duration;
	public float wait;
	public float animationDuration;
	public SpriteAnimator animator;
	public SpriteRenderer rend;

	public override PlatformType type {
		get {
			return PlatformType.Disappearing;
		}
	}

	private BoxCollider2D coll;

	private bool started, disappeared;
	private float startTime;
	private bool shouldAppear;

	private ContactFilter2D contactFilter;
	private Collider2D[] overlapResults;

	private float seed;

	public void Start() {
		this.coll = this.gameObject.GetComponent<BoxCollider2D>();

		this.contactFilter = new ContactFilter2D();
		this.contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(this.gameObject.layer));
		this.contactFilter.useLayerMask = true;
		this.contactFilter.useTriggers = false;

		this.overlapResults = new Collider2D[16];

		this.seed = Random.value * 5f;
	}

	public override void Contact(Actor actor, RaycastHit2D hit, bool vertical) {
		Disappear();
	}

	public void Update() {
		if(!this.started) {
			return;
		}

		if(!this.shouldAppear) {
			float delta = Time.time - this.startTime;

			if(delta < this.duration) {
				this.animator.enabled = true;
				SetProgress(Mathf.Clamp01( (delta - this.animationDuration) / (this.duration - this.animationDuration) ) );
			}
			
			if(!this.disappeared && delta > this.duration) {
				this.coll.enabled = false;
				this.animator.enabled = false;
				this.disappeared = true;
				SetProgress(1);
			} else if(this.disappeared && delta > this.duration + this.wait) {
				this.shouldAppear = true;
			}
		}

		if(this.shouldAppear) {
			int count = Physics2D.OverlapBoxNonAlloc(this.transform.position, this.coll.size, 0, this.overlapResults);
			bool doAppear = true;
			for(int i = 0; i < count; i++) {
				if(this.overlapResults[i].gameObject.tag == "Actor") {
					doAppear = false;
				}
			}

			if(doAppear) {
				this.coll.enabled = true;
				this.rend.sprite = this.animator.animations[0].frames[0];
				this.animator.enabled = false;
				this.animator.SetFrame(0);
				this.disappeared = false;
				this.started = false;

				StartCoroutine(ReappearCor());

				this.shouldAppear = false;
			}
		}
	}

	private void Disappear() {
		if(this.started) {
			return;
		}

		this.started = true;
		this.startTime = Time.time;
	}

	private IEnumerator ReappearCor() {
		float begin = Time.time;
		float dur = this.duration - this.animationDuration;

		while(Time.time - begin < dur && !this.started) {
			SetProgress(1 - ( (Time.time - begin) / dur ) );
			yield return null;
		}

		SetProgress(0);

		yield return null;
	}

	private void SetProgress(float t) {
		this.rend.material.SetVector("_Params", new Vector4(t, this.seed, 0, 0));
	}
}
