using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int totalRounds = 0; // 總回合數
    public int correctCount = 0; // 正確次數
    public int wrongCount = 0; // 錯誤次數

    public int maxRounds = 10; // 最大回合數
    private UIManager uiManager; // 引用 UI 管理器
    private bool isGameEnded = false; // 標誌遊戲是否結束

    private void Start()
    {
        // 獲取 UI 管理器
        uiManager = FindObjectOfType<UIManager>();

        if (uiManager == null)
        {
            Debug.LogError("GameUIManager 未找到！");
        }
    }

    public float Accuracy
    {
        get
        {
            if (totalRounds == 0) return 0f;
            return (float)correctCount / totalRounds * 100f;
        }
    }

    public void RecordResult(bool isCorrect)
    {
        if (isGameEnded) return; // 如果遊戲已結束，直接返回

        totalRounds++;

        if (isCorrect)
        {
            correctCount++;
        }
        else
        {
            wrongCount++;
        }

        // 更新 UI
        if (uiManager != null)
        {
            uiManager.UpdateUI();
        }

        // 如果達到最大回合數，結束遊戲
        if (totalRounds >= maxRounds)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        isGameEnded = true; // 標記遊戲結束

        // 顯示訓練報告
        Debug.Log("遊戲結束！");
        if (uiManager != null)
        {
            uiManager.ShowReport(correctCount, wrongCount, Accuracy);
        }
    }

    public bool IsGameEnded()
    {
        return isGameEnded;
    }

}
