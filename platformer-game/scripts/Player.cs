using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public CompositeCollider2D col;
    public PhysicsMaterial2D playerMaterial;
    SpriteRenderer sr;
    Animator anim;
    ParticleSystem emitterLanding;
    ParticleSystem emitterWalking;
    ParticleSystem emitterDash;
    ParticleSystem emitterDashRefresh;
    ParticleSystem emitterJump;
    BoxCollider2D playerCollider;
    GameObject playerSprite;
    Animator transitionAnim;
    Manager manage;
    GameObject slash;
    Animator slashAnim;
    SpriteRenderer slashSR;

    public AudioSource dashSound;
    public AudioSource landingSound;
    public AudioSource waklingSoundBase;
    public AudioClip walkingSound1;
    public AudioClip walkingSound2;
    public AudioClip walkingSound3;
    public AudioClip walkingSound4;
    public AudioSource jumpSound;
    public AudioClip jump;
    public AudioClip doubjeJump;
    public AudioSource attackSound;
    public AudioSource wallClingSound;

    public Vector2 respawnPoint;
    public float speed;
    public float airAccel;
    public float maxFallSpeed;
    public bool pointRight = true;
    // Input
    Vector2 dirInput;
    public Dictionary<string, KeyCode> keyBinds = new Dictionary<string, KeyCode>() {
        {"Left", KeyCode.A},
        {"Right", KeyCode.D},
        {"Up", KeyCode.W},
        {"Down", KeyCode.S},
        {"Jump", KeyCode.Space},
        {"Dash", KeyCode.DownArrow},
        {"Attack", KeyCode.LeftArrow}
    };

    // Input buffer
    public float inputBufferTime;
    float jumpBufferTimer;
    float dashBufferTimer;
    float attackBufferTimer;
    // Jump
    public float jumpVel;
    public float coyoteTime;
    float coyoteTimer;
    // Wall jump
    public float wallJumpTolerance;
    public float wallSlideSpeed;
    public float wallJumpXVel;
    public float wallJumpStunTime;
    public float dashJumpMultiplier;
    public float wallJumpCoyoteTime;
    float wallJumpTimer;
    bool highTolerance;
    float wallJumpCoyoteTimer;
    char lastWall;
    // Double jump
    public float doubleJumpMultiplier;
    bool doubleJumpAvailable;
    // Dash
    public float dashSpeed;
    public float dashTime;
    public float dashCooldown;
    public float dashEndVelMultiplier;
    public bool dashing = false;
    float dashTimer;
    bool dashAvailable;
    float gravityTemp;
    float dashCooldownTimer;
    // Attack
    public float attackCooldown;
    public float slashDist;
    public float slashXScale;
    float attackCooldownTimer;
    // Energy Field
    public float energyFieldStunTime;
    public float energyFieldSpeed;
    float energyFieldStunTimer;
    // Animations
    public float squashTime;
    float squashTimer;
    Vector3 ScaleDefault;
    Vector3 PosDefault;
    string nextTrigger = "";
    float lastYVel;
    bool jumpedThisFrame;
    // Sounds
    float walkSoundTime = .3f;
    float walkSoundTimer;
    bool lastFrameOnWall;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();

        playerSprite = GameObject.Find("player sprite");
        sr = playerSprite.GetComponent<SpriteRenderer>();
        anim = playerSprite.GetComponent<Animator>();

        emitterLanding = GameObject.Find("landing").GetComponent<ParticleSystem>();
        emitterWalking = GameObject.Find("walking").GetComponent<ParticleSystem>();
        emitterDash = GameObject.Find("dashing").GetComponent<ParticleSystem>();
        emitterDashRefresh = GameObject.Find("dash refresh").GetComponent<ParticleSystem>();
        emitterJump = GameObject.Find("jump").GetComponent<ParticleSystem>();

        transitionAnim = GameObject.Find("Transitions").GetComponent<Animator>();
        manage = GameObject.Find("Manager").GetComponent<Manager>();

        slash = GameObject.Find("slash");
        slashAnim = slash.GetComponent<Animator>();
        slashSR = slash.GetComponent<SpriteRenderer>();

        ScaleDefault = playerSprite.transform.localScale;
        PosDefault = playerSprite.transform.localPosition;
        respawnPoint = transform.position;
    }
    
    void Update()
    {
        UpdateDirectionalInput();

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Death"))
        {
            InputBuffer();
            Animations();
            WalkingSounds();
        }
        Respawn();
    }

    private void FixedUpdate()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Death"))
        {
            LandingParticles();
            SidewaysMovement();
            Dash();
            Wallslide();
            Jump();
            Attack();
            Bouncy();
        }

        if (rb.velocity.y < -maxFallSpeed)
            rb.velocity = new Vector2(rb.velocity.x, -maxFallSpeed);

        // Die if fall to low
        if (transform.position.y < -5 && !anim.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            Death();

        lastYVel = rb.velocity.y;
    }
    private void LateUpdate()
    { 
        // LateUpdate to change sprite local scale and pos
        StretchOnFall();
        GroundSmash();
    }

    void UpdateDirectionalInput()
    {
        // Key press
        if (Input.GetKeyDown(keyBinds["Left"]))
            dirInput.x = -1;
        else if (Input.GetKeyDown(keyBinds["Right"]))
            dirInput.x = 1;
        if (Input.GetKeyDown(keyBinds["Up"]))
            dirInput.y = 1;
        else if (Input.GetKeyDown(keyBinds["Down"]))
            dirInput.y = -1;

        // Key release
        if ((Input.GetKeyUp(keyBinds["Left"]) && dirInput.x == -1) || (Input.GetKeyUp(keyBinds["Right"]) && dirInput.x == 1))
        {
            if (Input.GetKey(keyBinds["Left"]))
                dirInput.x = -1;
            else if (Input.GetKey(keyBinds["Right"]))
                dirInput.x = 1;
            else
                dirInput.x = 0;
        }
        if ((Input.GetKeyUp(keyBinds["Up"]) && dirInput.y == 1) || (Input.GetKeyUp(keyBinds["Down"]) && dirInput.y == -1))
        {
            if (Input.GetKey(keyBinds["Up"]))
                dirInput.y = 1;
            else if (Input.GetKey(keyBinds["Down"]))
                dirInput.y = -1;
            else
                dirInput.y = 0;
        }
    }

    void InputBuffer()
    {
        if (Input.GetKeyDown(keyBinds["Jump"]))
        {
            jumpBufferTimer = inputBufferTime;
        }
        if (Input.GetKeyDown(keyBinds["Dash"]))
        {
            dashBufferTimer = inputBufferTime;
        }
        if (Input.GetKeyDown(keyBinds["Attack"]))
        {
            attackBufferTimer = inputBufferTime;
        }

        if (jumpBufferTimer > 0) jumpBufferTimer -= Time.deltaTime;
        if (dashBufferTimer > 0) dashBufferTimer -= Time.deltaTime;
        if (attackBufferTimer > 0) attackBufferTimer -= Time.deltaTime;
    }

    void SidewaysMovement()
    {
        // Particles and sounds for walking
        if (walkSoundTimer >= 0) walkSoundTimer -= Time.deltaTime;

        if (Mathf.Abs(rb.velocity.x) > .1f && Mathf.Abs(rb.velocity.y) < .1f && Grounded() && !dashing)
        {
            if (!emitterWalking.isPlaying)
                emitterWalking.Play();

            if (walkSoundTimer < 0)
            {
                int rnd = Random.Range(0, 3);
                switch (rnd)
                {
                    case 0:
                        //waklingSoundBase.PlayOneShot(walkingSound1);
                        break;
                    case 1:
                        //waklingSoundBase.PlayOneShot(walkingSound2);
                        break;
                    case 2:
                        //waklingSoundBase.PlayOneShot(walkingSound3);
                        break;
                    case 3:
                        //waklingSoundBase.PlayOneShot(walkingSound4);
                        break;
                }
                walkSoundTimer = walkSoundTime;
            }
        }
        else if ((Mathf.Abs(rb.velocity.x) < .1f || !Grounded()) && emitterWalking.isPlaying)
            emitterWalking.Stop();

        if (!dashing && wallJumpTimer <= 0 && energyFieldStunTimer <= 0)
        {
            // Pressing in the direction of horizontel momentum doesnt slow down the player to the normal movement speed
            if (dirInput.x * rb.velocity.x <= 0 || Mathf.Abs(rb.velocity.x) < speed)
            {
                // Instant movement on ground
                if (Grounded())
                    rb.velocity = new Vector2(dirInput.x * speed, rb.velocity.y);
                // Slowing down if no horizontal input
                else if (dirInput.x == 0)
                {
                    if (rb.velocity.x > 0)
                        rb.velocity -= new Vector2(airAccel * Time.deltaTime, 0);
                    else if (rb.velocity.x < 0)
                        rb.velocity += new Vector2(airAccel * Time.deltaTime, 0);

                    if (Mathf.Abs(rb.velocity.x) < 1)
                        rb.velocity = new Vector2(0, rb.velocity.y);
                }
                // Accelerate
                else
                {
                    rb.velocity += new Vector2(dirInput.x * airAccel * Time.deltaTime, 0);
                    if (rb.velocity.x > speed) rb.velocity = new Vector2(speed, rb.velocity.y);
                    else if (rb.velocity.x < -speed) rb.velocity = new Vector2(-speed, rb.velocity.y);
                }
            }
        }
    }

    void Wallslide()
    {
        if (OnWall() != 'n' && rb.velocity.y < -wallSlideSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            if (!lastFrameOnWall && !wallClingSound.isPlaying)
            {
                wallClingSound.pitch = Random.Range(.95f, 1.05f);
                wallClingSound.Play();
            }
            lastFrameOnWall = true;
        }
        else
            lastFrameOnWall = false;
    }

    void Jump()
    {
        // Jump, Double jump, Wall jump
        if (Grounded())
        {
            coyoteTimer = coyoteTime;
            doubleJumpAvailable = true;
            dashAvailable = true;
        }
        else if (coyoteTimer > 0)
            coyoteTimer -= Time.deltaTime;

        if (OnWall() != 'n')
        {
            wallJumpCoyoteTimer = wallJumpCoyoteTime;
            lastWall = OnWall();
        }
        else if (wallJumpCoyoteTimer > 0)
            wallJumpCoyoteTimer -= Time.deltaTime;

        if (jumpBufferTimer > 0 && !dashing)
        {
            // Normal jump
            if (coyoteTimer > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpVel);
                coyoteTimer = -1;
                squashTimer = -1; // Cancels ground squash animation
                ResetSprite();
                jumpBufferTimer = -1;
                nextTrigger = "TrJump";
                emitterJump.gameObject.transform.localPosition = new Vector3(0, -.7f, 0);
                emitterJump.gameObject.transform.eulerAngles = new Vector3(-90, 90, 0);
                emitterJump.Play();
                jumpedThisFrame = true;
                jumpSound.PlayOneShot(jump);
            }
            // Wall jump
            else if (wallJumpCoyoteTimer > 0)
            {
                // If wall jump timer didnt run out before starting another
                if (wallJumpTimer > 0) emitterDash.Stop();

                if (lastWall == 'l')
                {
                    rb.velocity = new Vector2(wallJumpXVel, jumpVel);
                    emitterJump.gameObject.transform.localPosition = new Vector3(-.4f, -.1f, 0);
                    emitterJump.gameObject.transform.eulerAngles = new Vector3(0, 90, 0);
                    emitterJump.Play();
                }
                else
                {
                    rb.velocity = new Vector2(-wallJumpXVel, jumpVel);
                    emitterJump.gameObject.transform.localPosition = new Vector3(.4f, -.1f, 0);
                    emitterJump.gameObject.transform.eulerAngles = new Vector3(180, 90, 0);
                    emitterJump.Play();
                }

                // Dash -> Jumps more powerful
                if (highTolerance)
                {
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * dashJumpMultiplier);
                    emitterDash.Play();
                }

                wallJumpTimer = wallJumpStunTime;
                squashTimer = -1; // Cancels ground squash animation
                ResetSprite();
                jumpBufferTimer = -1;
                nextTrigger = "TrWallJump";
                jumpSound.PlayOneShot(jump);
            }
            // Double jump
            else if (doubleJumpAvailable)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpVel * doubleJumpMultiplier);
                doubleJumpAvailable = false;
                squashTimer = -1; // Cancels ground squash animation
                jumpBufferTimer = -1;
                // Calcels wall jump and accelerates instaltly
                wallJumpTimer = -1;
                if (dirInput.x == 1)
                    rb.velocity = new Vector2(Mathf.Max(rb.velocity.x, speed), rb.velocity.y);
                else if (dirInput.x == -1)
                    rb.velocity = new Vector2(Mathf.Min(rb.velocity.x, -speed), rb.velocity.y);
                emitterDash.Stop();
                nextTrigger = "TrDoubleJump";   
                jumpSound.pitch = Random.Range(.95f, 1.05f);
                jumpSound.PlayOneShot(doubjeJump);
            }
        }

        if (wallJumpTimer > 0)
            wallJumpTimer -= Time.deltaTime;
        else if (wallJumpTimer <= 0 && wallJumpTimer != -1)
        {
            wallJumpTimer = -1;
            emitterDash.Stop();
        }

        highTolerance = false;
    }

    public bool Grounded()
    {
        Vector3 pointLeft = new Vector3(transform.position.x - .21f, transform.position.y - .67f, transform.position.z);
        Vector3 pointRight = new Vector3(transform.position.x + .21f, transform.position.y - .67f, transform.position.z);

        // Used to check if players hitbox is inside ground or wall
        Vector3 pointLeftCheck = new Vector3(transform.position.x - .215f, transform.position.y - .65f, transform.position.z);
        Vector3 pointRightCheck = new Vector3(transform.position.x + .215f, transform.position.y - .65f, transform.position.z);

        if (Physics2D.OverlapPoint(pointLeftCheck) == col || Physics2D.OverlapPoint(pointRightCheck) == col)
            return false;

        if (Physics2D.OverlapPoint(pointLeft) == col || Physics2D.OverlapPoint(pointRight) == col)
            return true;

        return false;
    }

    // Returns n for not on wall, l for left wall and r for right wall
    char OnWall()
    {
        if (highTolerance) wallJumpTolerance *= 2;

        Vector3 pointLeftTop = new Vector3(transform.position.x - wallJumpTolerance, transform.position.y + .35f, transform.position.z);
        Vector3 pointLeftBottom = new Vector3(transform.position.x - wallJumpTolerance, transform.position.y - .25f, transform.position.z);
        Vector3 pointRightTop = new Vector3(transform.position.x + wallJumpTolerance, transform.position.y + .35f, transform.position.z);
        Vector3 pointRightBottom = new Vector3(transform.position.x + wallJumpTolerance, transform.position.y - .25f, transform.position.z);

        // Used to check if players hitbox is inside ground
        Vector3 pointLeftCheck = new Vector3(transform.position.x - .21f, transform.position.y - .45f, transform.position.z);
        Vector3 pointRightCheck = new Vector3(transform.position.x + .21f, transform.position.y - .45f, transform.position.z);

        if (highTolerance) wallJumpTolerance /= 2;

        if (Physics2D.OverlapPoint(pointLeftCheck) == col || Physics2D.OverlapPoint(pointRightCheck) == col)
            return 'n';

        // If player is on corner double jump is prioritised over wall jump
        if ((Physics2D.OverlapPoint(pointLeftBottom) == col) && (Physics2D.OverlapPoint(pointLeftTop) == col || !doubleJumpAvailable))
            return 'l';
        else if ((Physics2D.OverlapPoint(pointRightBottom) == col) && (Physics2D.OverlapPoint(pointRightTop) == col || !doubleJumpAvailable))
            return 'r';

        return 'n';
    }

    void Dash()
    {
        if (dashBufferTimer > 0 && dashAvailable && !dashing && dashCooldownTimer <= 0)
        {
            // Start dash
            dashBufferTimer = -1;
            dashing = true;
            if (coyoteTimer <= 0)
                dashAvailable = false;
            dashTimer = dashTime;
            squashTimer = -1; // Cancels ground squash animation
            ResetSprite();
            playerCollider.size = new Vector2(playerCollider.size.x, playerCollider.size.y * .2f);
            // Dash cancels walljump momentum and energy field stun
            wallJumpTimer = -1;
            energyFieldStunTimer = -1;
            if (dirInput.x == 1) pointRight = true;
            else if (dirInput.x == -1) pointRight = false;

            gravityTemp = rb.gravityScale;
            rb.gravityScale = 0;
            if (dirInput.x == 0 && dirInput.y == 1)
                rb.velocity = Vector2.up * dashSpeed * .75f; // upwards dash is less powerful
            else if (dirInput.x == 0 && dirInput.y == -1)
                rb.velocity = Vector2.down * dashSpeed;
            else if (pointRight)
                rb.velocity = Vector2.right * dashSpeed;
            else
                rb.velocity = Vector2.left * dashSpeed;
            nextTrigger = "TrDash";
            emitterDash.Play();
            dashSound.pitch = Random.Range(.95f, 1.05f);
            dashSound.Play();
        }

        if (dashTimer < 0 && dashing)
        {
            // End dash
            rb.velocity *= dashEndVelMultiplier;
            rb.gravityScale = gravityTemp;
            dashing = false;
            dashCooldownTimer = dashCooldown;
            playerCollider.size = new Vector2(playerCollider.size.x, playerCollider.size.y * 5);

            // Used to check if players hitbox is inside ground after dashing downwards and collider expanding again
            Vector3 pointLeftCheck = new Vector3(transform.position.x - .215f, transform.position.y - .65f, transform.position.z);
            Vector3 pointRightCheck = new Vector3(transform.position.x + .215f, transform.position.y - .65f, transform.position.z);
            if (Physics2D.OverlapPoint(pointLeftCheck) == col || Physics2D.OverlapPoint(pointRightCheck) == col)
                transform.position = new Vector3(transform.position.x, transform.position.y + .2f, transform.position.z);

            // Updates direction in case of buffered inputs after the dash
            if (dirInput.x == 1) pointRight = true;
            else if (dirInput.x == -1) pointRight = false;

            emitterDash.Stop();
            emitterDashRefresh.Play();
            // Upwards dash -> walljump is easier
            highTolerance = true;
        }
        else if (dashTimer > 0)
            dashTimer -= Time.deltaTime;

        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;
    }

    void Attack()
    {
        if (attackCooldownTimer > 0)
            attackCooldownTimer -= Time.deltaTime;

        if (attackBufferTimer > 0 && attackCooldownTimer <= 0 && !dashing && (OnWall() == 'n' || rb.velocity.y > wallSlideSpeed))
        {
            Vector2 hitboxPos = Vector2.zero;
            // If player is grounded no down attack is possible
            if (dirInput.y == 0 || (Grounded() && dirInput.y == -1))
            {
                if (dirInput.x == 1 || (dirInput.x == 0 && pointRight))
                {
                    slash.transform.localPosition = new Vector3(slashDist, -.1f, slash.transform.localPosition.z);
                    slash.transform.localScale = new Vector3(slashXScale, 1.1f, 1);
                    slashSR.flipX = false;
                    slashSR.flipY = false;
                    slashAnim.SetTrigger("TrSideSlash");
                    nextTrigger = "TrSideAttack";
                    hitboxPos = new Vector2(1.25f, -.3f);
                }
                else if (dirInput.x == -1 || (dirInput.x == 0 && !pointRight))
                {
                    slash.transform.localPosition = new Vector3(-slashDist, -.1f, slash.transform.localPosition.z);
                    slash.transform.localScale = new Vector3(slashXScale, 1.1f, 1);
                    slashSR.flipX = true;
                    slashSR.flipY = false;
                    slashAnim.SetTrigger("TrSideSlash");
                    nextTrigger = "TrSideAttack";
                    hitboxPos = new Vector2(-1.25f, -.3f);
                }
            }
            else if (dirInput.y == 1)
            {
                slash.transform.localPosition = new Vector3(0, .5f, slash.transform.localPosition.z);
                slash.transform.localScale = new Vector3(2.3f, 1, 1);
                if (pointRight) slashSR.flipX = true;
                else slashSR.flipX = false;
                slashSR.flipY = true;
                slashAnim.SetTrigger("TrDownSlash");
                nextTrigger = "TrUpAttack";
                hitboxPos = new Vector2(0, .8f);
            }
            else if (dirInput.y == -1)
            {
                slash.transform.localPosition = new Vector3(0, -.7f, slash.transform.localPosition.z);
                slash.transform.localScale = new Vector3(2, 1, 1);
                if (pointRight) slashSR.flipX = true;
                else slashSR.flipX = false;
                slashSR.flipY = false;
                slashAnim.SetTrigger("TrDownSlash");
                nextTrigger = "TrDownAttack";
                hitboxPos = new Vector2(0, -1.2f);
            }
            attackCooldownTimer = attackCooldown;
            attackBufferTimer = -1;
            attackSound.pitch = Random.Range(.95f, 1.05f);
            attackSound.Play();

            Collider2D[] collidersInCircle = Physics2D.OverlapCircleAll(new Vector2(transform.position.x + hitboxPos.x, transform.position.y + hitboxPos.y), .6f);
            foreach (Collider2D c in collidersInCircle)
            {
                // Energy field bounces player away
                if (c.gameObject.layer == 7)
                {
                    doubleJumpAvailable = true;
                    GameObject partObj = c.gameObject.transform.Find("Particle System").gameObject;
                    
                    if (hitboxPos == new Vector2(1.25f, -.3f))
                    {
                        rb.velocity = new Vector2(-energyFieldSpeed, Mathf.Max(rb.velocity.y, energyFieldSpeed * .25f));
                        energyFieldStunTimer = energyFieldStunTime;
                        partObj.transform.eulerAngles = new Vector3(0, 90, 0);
                        // To change friction the collider need to be disabled and enabled to apply the change
                        playerMaterial.friction = 2f;
                        playerCollider.enabled = false;
                        playerCollider.enabled = true;
                    }
                    else if (hitboxPos == new Vector2(-1.25f, -.3f))
                    {
                        rb.velocity = new Vector2(energyFieldSpeed, Mathf.Max(rb.velocity.y, energyFieldSpeed * .25f));
                        energyFieldStunTimer = energyFieldStunTime;
                        partObj.transform.eulerAngles = new Vector3(180, 90, 0);
                        // To change friction the collider need to be disabled and enabled to apply the change
                        playerMaterial.friction = 2f;
                        playerCollider.enabled = false;
                        playerCollider.enabled = true;
                    }
                    else if (hitboxPos == new Vector2(0, -1.2f))
                    {
                        rb.velocity = new Vector2(rb.velocity.x, energyFieldSpeed);
                        partObj.transform.eulerAngles = new Vector3(90, 90, 0);
                    }
                    else if (hitboxPos == new Vector2(0, .8f))
                    {
                        rb.velocity = new Vector2(rb.velocity.x, -energyFieldSpeed);
                        partObj.transform.eulerAngles = new Vector3(-90, 90, 0);
                    }
                    
                    partObj.GetComponent<ParticleSystem>().Play();
                    c.gameObject.GetComponent<AudioSource>().Play();
                }

                // Crystal restores dash and double jump
                if (c.gameObject.layer == 8)
                {
                    c.gameObject.GetComponent<Crystal>().HitCrystal();
                    dashAvailable = true;
                    doubleJumpAvailable = true;
                    dashCooldownTimer = 0;
                    if (hitboxPos == new Vector2(0, -1.2f))
                        rb.velocity = new Vector2(rb.velocity.x, 8);
                }
            }
        }  
    }

    void Bouncy()
    {
        if (energyFieldStunTimer > 0)
        {
            energyFieldStunTimer -= Time.deltaTime;
        }
        else
        {
            // To change friction the collider need to be disabled and enabled to apply the change
            playerMaterial.friction = 0;
            playerCollider.enabled = false;
            playerCollider.enabled = true;
        }
        
    }

    void Animations()
    {
        // Flips player
        if (OnWall() == 'r' && rb.velocity.y < -wallSlideSpeed) pointRight = false;
        else if (OnWall() == 'l' && rb.velocity.y < -wallSlideSpeed) pointRight = true;
        else
        {
            if (rb.velocity.x < -1) pointRight = false;
            else if (rb.velocity.x > 1) pointRight = true;
        }

        if (pointRight)
        {
            sr.flipX = false;
            playerSprite.transform.localPosition = new Vector3(-0.03f, playerSprite.transform.localPosition.y, playerSprite.transform.localPosition.z);
        }
        else
        {
            sr.flipX = true;
            playerSprite.transform.localPosition = new Vector3(0.03f, playerSprite.transform.localPosition.y, playerSprite.transform.localPosition.z);
        } 

        
        // Used to get info about animations
        AnimatorStateInfo animInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (nextTrigger != "TrDeath" && (animInfo.normalizedTime > 1 || animInfo.IsName("Idle") || animInfo.IsName("Running") || animInfo.IsName("Falling") || animInfo.IsName("Wallslide")))
        {
            if (!Grounded() || Mathf.Abs(rb.velocity.y) > .1f)
            {
                if (OnWall() == 'n' && rb.velocity.y < -.1f && !animInfo.IsName("Falling"))
                    anim.SetTrigger("TrFalling");
                else if (OnWall() != 'n' && !animInfo.IsName("Wallslide") && rb.velocity.y < -wallSlideSpeed)
                    anim.SetTrigger("TrWallslide");
            }
            else
            {
                if (Mathf.Abs(rb.velocity.x) > .1f && !animInfo.IsName("Running"))
                    anim.SetTrigger("TrRunning");
                else if (Mathf.Abs(rb.velocity.x) < .1f && !animInfo.IsName("Idle"))
                    anim.SetTrigger("TrIdle");
            }
        }

        if (nextTrigger != "")
            anim.SetTrigger(nextTrigger);

        nextTrigger = "";
    }

    void WalkingSounds()
    {

    }

    // Next 2 functions impact the player sprite scale and position

    // Faster -> more stretched
    void StretchOnFall()
    {
        if (rb.velocity.y < -5 && !dashing)
        {
            playerSprite.transform.localScale = new Vector3(ScaleDefault.x - (-(rb.velocity.y + 5) * 0.015f), ScaleDefault.y + (-(rb.velocity.y + 5) * 0.03f), ScaleDefault.z);
        }
    }

    // Squash on ground hit
    void GroundSmash()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            squashTimer = 0;

        // Doesnt interrupt dash and jump animation
        if (Mathf.Abs(rb.velocity.y) < .1f && lastYVel < -5 && !dashing && dashBufferTimer <= 0  && jumpBufferTimer <= 0)
        {
            squashTimer = squashTime;
        }
        if (squashTimer > 0)
        {
            squashTimer -= Time.deltaTime;
            float relativeTime = squashTimer / squashTime;
            playerSprite.transform.localPosition = new Vector3(playerSprite.transform.localPosition.x, PosDefault.y -.24f * Polynom(relativeTime), PosDefault.z);
            playerSprite.transform.localScale = new Vector3(ScaleDefault.x + .4f * Polynom(relativeTime), ScaleDefault.y -.5f * Polynom(relativeTime), ScaleDefault.z);
        }
    }

    // Used for smooth squash
    float Polynom(float x)
    {
        return -2*x*x*x + 3*x*x;
    }

    void LandingParticles()
    {
        if ((Mathf.Abs(rb.velocity.y) < .1f || (jumpedThisFrame && rb.velocity.y > -.1f)) && lastYVel < -7 && !dashing  && dashBufferTimer <= 0)
        {
            emitterLanding.Emit((int)(lastYVel * -.8f));
            landingSound.volume = Mathf.Min((-lastYVel - 5) * .05f, 1);
            landingSound.pitch = Random.Range(.95f, 1.05f);
            landingSound.Play();
            walkSoundTimer = walkSoundTime;
        }
        jumpedThisFrame = false;
    }

    void ResetSprite()
    {
        playerSprite.transform.localScale = ScaleDefault;
        playerSprite.transform.localPosition = PosDefault;
    }

    public void Death()
    {
        nextTrigger = "TrDeath";
        transitionAnim.SetTrigger("TrDeathtransition");
        ResetSprite();
        emitterDash.Stop();
        emitterWalking.Stop();
        rb.bodyType = RigidbodyType2D.Static;
    }

    void Respawn()
    {
        AnimatorStateInfo animInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (animInfo.IsName("Death") && animInfo.normalizedTime > 1)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            transform.position = new Vector3(respawnPoint.x, respawnPoint.y, transform.position.z);
            rb.velocity = Vector2.up * jumpVel;
            anim.Play("Jump");
            manage.Reload(GameObject.FindGameObjectsWithTag("Moving"));
        }
    }

    //Debug
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(transform.position.x - wallJumpTolerance, transform.position.y + .35f, transform.position.z), .01f);
        Gizmos.DrawSphere(new Vector3(transform.position.x + wallJumpTolerance, transform.position.y + .35f, transform.position.z), .01f);
        Gizmos.DrawSphere(new Vector3(transform.position.x - wallJumpTolerance, transform.position.y - .25f, transform.position.z), .01f);
        Gizmos.DrawSphere(new Vector3(transform.position.x + wallJumpTolerance, transform.position.y - .25f, transform.position.z), .01f);
        Gizmos.DrawSphere(new Vector3(transform.position.x - .21f, transform.position.y - .45f, transform.position.z), .01f);
        Gizmos.DrawSphere(new Vector3(transform.position.x + .21f, transform.position.y - .45f, transform.position.z), .01f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(new Vector3(transform.position.x - .21f, transform.position.y - .67f, transform.position.z), .01f);
        Gizmos.DrawSphere(new Vector3(transform.position.x + .21f, transform.position.y - .67f, transform.position.z), .01f);
        Gizmos.DrawSphere(new Vector3(transform.position.x - .215f, transform.position.y - .65f, transform.position.z), .01f);
        Gizmos.DrawSphere(new Vector3(transform.position.x + .215f, transform.position.y - .65f, transform.position.z), .01f);

        Gizmos.color = Color.green;
        //Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - 1.2f, transform.position.z), .6f);
        //Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y + .8f, transform.position.z), .6f);
        //Gizmos.DrawSphere(new Vector3(transform.position.x + 1.25f, transform.position.y - .3f, transform.position.z), .6f);
        //izmos.DrawSphere(new Vector3(transform.position.x - 1.25f, transform.position.y - .3f, transform.position.z), .6f);

    }
}
