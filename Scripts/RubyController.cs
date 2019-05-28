using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{
	public int maxHealth = 5;
	public float timeInvincible = 2.0f;
	public float speed = 3.0f;
	public int health { get { return currentHealth; }}
	public GameObject projectilePrefab;
	
	int currentHealth;
	bool isInvincible;
	float invincibleTimer;
	
	Rigidbody2D rigidbody2d;
	
	AudioSource audioSource;
	
	public AudioClip cog;
	
	Animator animator;
	Vector2 lookDirection = new Vector2(1,0);
    // Start is called before the first frame update
    void Start()
    {
		rigidbody2d = GetComponent<Rigidbody2D>();
		currentHealth = maxHealth;
		animator = GetComponent<Animator>();
		
		audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
		Vector2 move = new Vector2(horizontal, vertical);
        
		if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
		{
			lookDirection.Set(move.x, move.y);
			lookDirection.Normalize();
		}
				
		animator.SetFloat("Look X", lookDirection.x);
		animator.SetFloat("Look Y", lookDirection.y);
		animator.SetFloat("Speed", move.magnitude);
        Vector2 position = rigidbody2d.position;
        position = position + move * speed * Time.deltaTime;
        rigidbody2d.MovePosition(position);
		
		if (isInvincible)
		{
			invincibleTimer -= Time.deltaTime;
			if (invincibleTimer < 0)
				isInvincible = false;
		}
		if(Input.GetKeyDown(KeyCode.C))
		{
			Launch();
		}
		if (Input.GetKeyDown(KeyCode.X))
		{
			RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
			if (hit.collider != null)
			{
				NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
				if (character != null)
				{
					character.DisplayDialog();
				}
			}
		}
    }
	public void PlaySound(AudioClip clip)
	{
		audioSource.PlayOneShot(clip);
	}
	public void ChangeHealth(int amount)
	{
		if (amount < 0)
		{
			if (isInvincible)
				return;
			
			isInvincible = true;
			invincibleTimer = timeInvincible;
		}
		currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
		UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
	}
	void Launch()
	{
		GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
		Projectile projectile = projectileObject.GetComponent<Projectile>();
		projectile.Launch(lookDirection, 300);
		PlaySound(cog);
		
		animator.SetTrigger("Launch");
	}
}