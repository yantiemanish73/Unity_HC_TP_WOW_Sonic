using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("跟隨百分比"), Range(0.1f, 1)]
    public float percent = 0.6f;
    [Header("跟隨速度"), Range(10, 100)]
    public float speed = 30;
    [Header("上下限制")]
    public Vector2 limit = new Vector2(3.5f, 6);

    private Transform player;

    private void Start()
    {
        player = GameObject.Find("音速小子").transform;
    }

    private void LateUpdate()
    {
        Follow();
    }

    /// <summary>
    /// 跟隨玩家
    /// </summary>
    private void Follow()
    {
        Vector3 pos = player.position;
        pos.x = 15;
        pos.y = Mathf.Clamp(pos.y, limit.x, limit.y);
        transform.position = Vector3.Lerp(transform.position, pos, percent * Time.deltaTime * speed);
        transform.eulerAngles = Vector3.up * -90;
    }
}
