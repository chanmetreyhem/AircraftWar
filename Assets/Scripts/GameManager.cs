
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Text boomAmountText,titleText,levelText,againButtonText;
    [SerializeField] private GameObject tank;

    public Image revealImage;
    public Slider healthSlider;
    public GameObject gameOverPanel;
    public bool isEndGame = false;
    public Vector3 clonePos;
    public float xRange = 9;
    public List<Tank> tanks = new List<Tank>();
    public int level = 1;
    public const string LEVEL_KEY = "level";

   
    void Start()
    {
        level = PlayerPrefs.GetInt(LEVEL_KEY,1) > 15 ? 15 : PlayerPrefs.GetInt(LEVEL_KEY, 1);
        levelText.text = "Level: " + level;

        for(int i = 0; i < level; i++)
        {
            clonePos.x = Random.Range(-xRange, xRange);
            tanks.Add(Instantiate(tank,clonePos,Quaternion.Euler(new Vector3(0,Random.Range(-90,90),0))).gameObject.GetComponent<Tank>());
        }
    }

    void Update()
    {
        if (isEndGame) return;
        if(tanks.Count <= 0)
        {
            isEndGame = true;
            titleText.text = "You Win";
            againButtonText.text = "Next";
            level += 1;
            PlayerPrefs.SetInt(LEVEL_KEY, level);
            PopupGameOver();
        }
    }

    public void UpdateBoomAmountUI(float amount)
    {
        boomAmountText.text = amount.ToString("00");
    }

    public IEnumerator  RevealBoomAmount(float time)
    {
        revealImage.gameObject.SetActive(true);
        while (time > 0)
        {
            time -= Time.deltaTime;
            revealImage.fillAmount = Mathf.Clamp(time, 0, 1);
            yield return null;
        }
        revealImage.gameObject.SetActive(false);
    }


    public void UpdateHealthSlider(float value)
    {
        healthSlider.value = value;
    }


    public void PopupGameOver()
    {
        isEndGame = true;
        gameOverPanel.SetActive(true);
    }

    public void Home()
    {
        SceneManager.LoadScene(0);
    }

    public void Again()
    {
        SceneManager.LoadScene(1);
    }
    
}
