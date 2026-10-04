using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI StatsLableTextMesh;
    [SerializeField] private GameObject SpeedRightArrowGameObject;
    [SerializeField] private GameObject SpeedLeftArrowGameObject;
    [SerializeField] private GameObject SpeedUpArrowGameObject;
    [SerializeField] private GameObject SpeedDownArrowGameObject;
    [SerializeField] private Image FuelImage;

    private void Update()
    {
        UpdateStatsTextMesh();
    }
    
    private void UpdateStatsTextMesh()
    {
        SpeedRightArrowGameObject.SetActive(Lander.Instance.GetSpeedX() >= 0);
        SpeedLeftArrowGameObject.SetActive(Lander.Instance.GetSpeedX() <= 0);
        SpeedUpArrowGameObject.SetActive(Lander.Instance.GetSpeedY() >= 0);
        SpeedDownArrowGameObject.SetActive(Lander.Instance.GetSpeedY() <= 0);

        FuelImage.fillAmount = Lander.Instance.GetNormalizedFuel();

        StatsLableTextMesh.text =
           GameManager.Instance.GetLevelNumber() + "\n" + 
           GameManager.Instance.GetScore() + "\n" +
           Mathf.Abs(Mathf.Round(GameManager.Instance.GetTime())) + "\n" +
           Mathf.Abs(Mathf.Round(Lander.Instance.GetSpeedX())) * 10f + "\n" +
           Mathf.Round(Lander.Instance.GetSpeedY()) * 10f;
           
    }

}
