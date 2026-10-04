using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LandedUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TitleTextMesh;
    [SerializeField] private TextMeshProUGUI StatsTextMesh;
    [SerializeField] private TextMeshProUGUI NextButtonTextMesh;
    [SerializeField] private Button NextButton;

    private Action nextButtonClickAction; 
    private void Awake()
    {
        NextButton.onClick.AddListener( ()=>
        {
            nextButtonClickAction();
        });

    }
    private void Start()
    {
        Lander.Instance.OnLanded += Lander_OnLanded;
        hide();
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        if (e.landingType == Lander.LandingType.Success)
        {
            TitleTextMesh.text = "SUCCESS";
            nextButtonClickAction = GameManager.Instance.GoToNextLevel;
            NextButtonTextMesh.text = "CONTINUE";
        }
        else
        {
            TitleTextMesh.text = "<color=#ff0000>CRASH</color>";
            nextButtonClickAction = GameManager.Instance.RetryLevel;
            NextButtonTextMesh.text = "RETRY";

        }

        StatsTextMesh.text =
            Mathf.Round(e.landingSpeed * 2f) + "\n" +
            Mathf.Round(e.dotvector * 100f) + "\n" +
            "x" + e.scoreMultiplier + "\n" +
            e.score;

        show();
        Debug.Log("LandedUI received OnLanded event!");
    }

    private void show()
    {
        gameObject.SetActive(true);
    }

    private void hide()
    {
        gameObject.SetActive(false);
    } 
}
