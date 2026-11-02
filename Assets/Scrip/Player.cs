using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public float speed = 10f;
	public Vector2 maxVelocity = new Vector2(3, 5);
	public bool standing;
	public float jetSpeed = 15f;
	public float airSpeedMultiplier = .3f;

	private PlayerController controller;
	private Animator animator;

	public AudioClip leftFootSound;
	public AudioClip rightFootSound;
	public AudioClip thudSound;
	public AudioClip rocketSound;

	void Start(){
		controller = GetComponent<PlayerController> ();
		animator = GetComponent<Animator> ();
	}

	void PlayLeftFootSound(){
		if (leftFootSound)//kalo file suaranya ga kosong
			AudioSource.PlayClipAtPoint (leftFootSound, transform.position);//play pada posisi player
	}

	void PlayRightFootSound(){
		if (rightFootSound)
			AudioSource.PlayClipAtPoint (rightFootSound, transform.position);
	}
	void OnCollisionEnter2D(Collision2D target){//bunyi ketika mendarat injak tanah
		if (!standing) { //lagi terbang
			var absVelX = Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x); //ambil posisi
			var absVelY = Mathf.Abs(GetComponent<Rigidbody2D>().velocity.y);

			if(absVelX <= .1f || absVelY <= .1f){ //gerakan x&y kecil, artinya sdg mendarat
				if(thudSound)
					AudioSource.PlayClipAtPoint(thudSound, transform.position);
			}
		}

	}

	void PlayRocketSound(){
		if (!rocketSound || GameObject.Find ("RocketSound"))//klo sudah ada gameobjectnya, ga dibunyiin lagi
			return; //lgsg return

		GameObject go = new GameObject ("RocketSound"); //buat gameobject
		AudioSource aSrc = go.AddComponent<AudioSource> (); //masukkin audiosource
		aSrc.clip = rocketSound; //masukkin clip
		aSrc.volume = 0.7f; //set volume
		aSrc.Play ();

		Destroy (go, rocketSound.length); //destroy setelah suara SELESAI dibunyikan		
	}

	// Update is called once per frame
	void Update () {
		//menetralkan nilai gerakan
		var forceX = 0f;
		var forceY = 0f;
		
		//ngecek kecepatan sekarang
		var absVelX = Mathf.Abs (GetComponent<Rigidbody2D>().velocity.x);
		var absVelY = Mathf.Abs (GetComponent<Rigidbody2D>().velocity.y);

		if (absVelY < .2f) {//cek lagi terbang atau tidak
			standing = true;
		} else {
			standing = false;
		}

		if (controller.moving.x != 0) {//cek kontroller gerak kiri kanan
			if(absVelX < maxVelocity.x){//pembatas agar tidak lebih dari kecepatan maks
				forceX = standing ? speed * controller.moving.x : (speed * controller.moving.x * airSpeedMultiplier); //kalo lg di udara akan ada perkalian dgn airspeed, yg membuatnya lbh lamban
				transform.localScale = new Vector3(forceX > 0 ? 1 : -1, 1, 1);//utk flip gambar
			}
			animator.SetInteger ("AnimState", 1); //animasi jalan
		} else {
			animator.SetInteger ("AnimState", 0); //animasi stop
		}

		if (controller.moving.y > 0) {//cek kontroller gerak keatas
			PlayRocketSound();
			if(absVelY < maxVelocity.y)//pembatas kecepatan terbang
				forceY = jetSpeed * controller.moving.y;
				animator.SetInteger ("AnimState", 2); //animasi jet terbang ada apinya
			} else if (absVelY > 0) {
			animator.SetInteger("AnimState", 3); //animasi jet tidak ada apinya
		}

		GetComponent<Rigidbody2D>().AddForce (new Vector2 (forceX, forceY));//melakukan gerakan tsb berdasarkan nilai forceX & forceY yg sudah dihitung diatas
	}
}
