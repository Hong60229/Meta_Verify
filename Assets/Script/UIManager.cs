using TMPro; // 引入 TextMeshPro 的命名空間
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI roundsText; // 回合數文本
    public TextMeshProUGUI accuracyText; // 準確度文本

    public GameObject reportWindow; // 訓練報告窗口
    public TextMeshProUGUI reportText; // 訓練報告內容

    public GameObject hintWindow;
    public GameObject scoreWindow;

    private GameManager gameManager; // 引用 GameManager

    private void Start()
    {
        // 獲取統計管理器
        gameManager = FindObjectOfType<GameManager>();

        if (gameManager == null)
        {
            Debug.LogError("GameStatsManager 未找到！");
        }

        // 初始化 UI
        UpdateUI();

        // 隱藏訓練報告窗口
        if (reportWindow != null)
        {
            reportWindow.SetActive(false);
        }
    }

    public void UpdateUI()
    {
        if (gameManager == null) return;

        // 更新回合數和準確度
        roundsText.text = $"回合數 : {gameManager.totalRounds + 1} / {gameManager.maxRounds}";
        accuracyText.text = $"準確度 : {gameManager.Accuracy:F2}%";
    }

    public void ShowReport(int correct, int wrong, float accuracy)
    {
        // 顯示訓練報告窗口
        if (reportWindow != null)
        {
            reportWindow.SetActive(true);
        }

        // 設定訓練報告內容
        if (reportText != null)
        {
            reportText.text = $"訓練報告\n\n" +
                              $"正確次數: {correct}\n" +
                              $"錯誤次數: {wrong}\n" +
                              $"準確度: {accuracy:F2}%";
        }

        if (hintWindow != null)
        {
            hintWindow.SetActive(false);
        }

        if (scoreWindow != null)
        {
            scoreWindow.SetActive(false);
        }

    }

    public void ToggleHintWindow()
    {
        if (hintWindow != null)
        {
            // 切換 HintWindow 的啟動和關閉狀態
            hintWindow.SetActive(!hintWindow.activeSelf);
        }
        else
        {
            Debug.LogWarning("HintWindow 未設置！");
        }
    }

    public void ToggleScoreWindow()
    {
        if (scoreWindow != null)
        {
            // 切換 HintWindow 的啟動和關閉狀態
            scoreWindow.SetActive(!scoreWindow.activeSelf);
        }
        else
        {
            Debug.LogWarning("ScoreWindow 未設置！");
        }
    }

    public void RestartProject()
    {
        // 獲取當前活動場景的名稱
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 加載當前場景，實現重啟效果
        SceneManager.LoadScene(currentSceneName);

        Debug.Log("專案已重啟！");
    }

}
