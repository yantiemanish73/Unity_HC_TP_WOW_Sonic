using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private Text textTime;
    private Text textCoin;
    private Transform passPanel;
    
    [HideInInspector]
    public int coinCurrent;
    [HideInInspector]
    public int coinTotal;

    private SonicBoy sonicBoy;
    private string textContentCoin;
    private string textContentTime;
    private float timer;
    private bool pass;
    private bool canReplay;

    private void Start()
    {
        Initial();
    }

    private void Update()
    {
        UpdateTime();
        Replay();
    }

    /// <summary>
    /// 初始
    /// </summary>
    private void Initial()
    {
        textTime = GameObject.Find("時間文字").GetComponent<Text>();
        textCoin = GameObject.Find("金幣文字").GetComponent<Text>();
        passPanel = GameObject.Find("過關畫面").transform;

        if (PlayerPrefs.GetFloat("最快時間") == 0) PlayerPrefs.SetFloat("最快時間", 99999);
        sonicBoy = FindObjectOfType<SonicBoy>();
        coinTotal = GameObject.FindGameObjectsWithTag(sonicBoy.coin).Length;
        if (textCoin)
        {
            textContentCoin = textCoin.text;
            textCoin.text = textContentCoin + 0 + " / " + coinTotal;
        }
        if (textTime) textContentTime = textTime.text;
    }

    /// <summary>
    /// 更新金幣
    /// </summary>
    public void UpdateCoin()
    {
        if (textCoin)
        {
            coinCurrent++;
            textCoin.text = textContentCoin + coinCurrent + " / " + coinTotal;
        }
    }

    /// <summary>
    /// 更新時間
    /// </summary>
    private void UpdateTime()
    {
        if (textTime && !pass)
        {
            timer = Time.timeSinceLevelLoad;
            textTime.text = textContentTime + timer.ToString("F2");
        }
    }

    /// <summary>
    /// 重新遊戲
    /// </summary>
    private void Replay()
    {
        if (canReplay && Input.anyKeyDown)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// 過關
    /// </summary>
    public IEnumerator Pass()
    {
        if (!pass)
        {
            pass = true;
            sonicBoy.enabled = false;
            CanvasGroup group = passPanel.GetComponent<CanvasGroup>();
            group.blocksRaycasts = true;

            if (timer < PlayerPrefs.GetFloat("最快時間")) PlayerPrefs.SetFloat("最快時間", timer);

            string textContentBest = passPanel.GetChild(0).GetComponent<Text>().text;
            passPanel.GetChild(0).GetComponent<Text>().text = textContentBest + PlayerPrefs.GetFloat("最快時間").ToString("F2");

            string textContentCurrent = passPanel.GetChild(1).GetComponent<Text>().text;
            passPanel.GetChild(1).GetComponent<Text>().text = textContentCurrent + timer.ToString("F2");

            for (int i = 1; i < 3; i++) passPanel.GetChild(i).gameObject.SetActive(false);

            while (group.alpha < 1)
            {
                group.alpha += 0.1f;
                yield return new WaitForSeconds(0.05f);
            }

            for (int i = 1; i < 3; i++)
            {
                yield return new WaitForSeconds(0.5f);
                passPanel.GetChild(i).gameObject.SetActive(true);
            }

            canReplay = true;
        }
    }
}
