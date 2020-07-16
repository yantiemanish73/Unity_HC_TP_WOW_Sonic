using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SonicBoy : MonoBehaviour
{
    [Header("走路速度"), Range(10, 100)]
    public float speedWalk = 80;
    [Header("跑步速度"), Range(100, 500)]
    public float speedRun = 150;
    [Header("跳躍高度"), Range(500, 5000)]
    public float jump = 1500;
    [Header("阻力"), Range(1, 5), Tooltip("避免移動時滑行")]
    public float drag = 2.5f;
    [Header("加速音效")]
    public AudioClip soundRun;
    [Header("金幣音效")]
    public AudioClip soundCoin;
    [Header("遊戲失敗音效")]
    public AudioClip soundGameOver;
    [Header("遊戲過關音效")]
    public AudioClip soundGamePass;

    [HideInInspector]
    public  string coin = "金幣";
    private float speed;
    private bool isGrounded;
    private Rigidbody rig;
    private Animator ani;
    private AudioSource aud;
    private ParticleSystem[] ps;
    private GameManager gm;
    private bool dead;

    private void Start()
    {
        rig = GetComponent<Rigidbody>();
        ani = GetComponent<Animator>();
        aud = GetComponent<AudioSource>();
        ps = GetComponentsInChildren<ParticleSystem>();
        gm = FindObjectOfType<GameManager>();

        rig.drag = drag;
        speed = speedWalk;

        Physics.gravity = Vector3.up * -50;
    }

    private void Update()
    {
        Jump();
        CheckDead();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + transform.up * 0.05f, -transform.up * 0.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        EatCoin(other);

        if (other.name == "過關區域" && gm.coinCurrent == gm.coinTotal) Pass();
    }

    /// <summary>
    /// 移動
    /// </summary>
    private void Move()
    {
        float h = Input.GetAxis("Horizontal");

        //if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow)) transform.localScale = new Vector3(1, 1, 1);
        //else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow)) transform.localScale = new Vector3(1, 1, -1);

        transform.localScale = new Vector3(1, 1, rig.velocity.z >= 0 ? 1 : -1);

        speed = Input.GetKey(KeyCode.LeftShift) ? speedRun : speedWalk;
        ani.SetBool("走路開關", h != 0);
        ani.SetBool("跑步開關", speed == speedRun);

        rig.AddForce(transform.forward * h * speed);

        if (rig.velocity.magnitude > 0.2f) for (int i = 0; i < ps.Length; i++) ps[i].Play();
        else for (int i = 0; i < ps.Length; i++) ps[i].Stop();

        if (h != 0 && Input.GetKeyDown(KeyCode.LeftShift)) aud.PlayOneShot(soundRun);
    }

    /// <summary>
    /// 跳躍
    /// </summary>
    private void Jump()
    {
        if (Physics.Raycast(transform.position + transform.up * 0.05f, -transform.up, 0.15f))
        {
            isGrounded = true;
            rig.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        }
        else
        {
            isGrounded = false;
            rig.constraints = RigidbodyConstraints.FreezeRotation;
        }
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rig.AddForce(transform.up * jump);
            rig.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    /// <summary>
    /// 檢查是否死亡
    /// </summary>
    private void CheckDead()
    {
        if (!dead && transform.position.y < -6)
        {
            dead = true;
            StartCoroutine(Dead());
        }
    }

    /// <summary>
    /// 死亡
    /// </summary>
    private IEnumerator Dead()
    {
        CloseAllAudio();
        aud.pitch = 1.5f;
        aud.PlayOneShot(soundGameOver);
        enabled = false;
        yield return new WaitForSeconds(6);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// 關閉所有音效
    /// </summary>
    private void CloseAllAudio()
    {
        AudioSource[] auds = FindObjectsOfType<AudioSource>();
        for (int i = 0; i < auds.Length; i++) auds[i].Stop();
    }

    /// <summary>
    /// 吃金幣
    /// </summary>
    /// <param name="other">碰撞的物件</param>
    private void EatCoin(Collider other)
    {
        if (other.tag == coin)
        {
            aud.PlayOneShot(soundCoin);
            Destroy(other.gameObject);
            gm.UpdateCoin();
        }
    }

    /// <summary>
    /// 過關
    /// </summary>
    private void Pass()
    {
        CloseAllAudio();
        aud.PlayOneShot(soundGamePass);
        StartCoroutine(gm.Pass());
    }
}
