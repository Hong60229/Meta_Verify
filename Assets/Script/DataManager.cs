using MixedReality.Toolkit.SpatialManipulation;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using static XCharts.Runtime.RadarCoord;

public class DataManager : MonoBehaviour
{
    // 用於生成的新物件的預製體
    public GameObject dataPrefab;
    public string materialsFolderPath = "Data"; // 資料夾名稱，存放材質
    private List<Material> materials = new List<Material>(); // 材質列表

    public DirectionalIndicator indicator; // 指向 DirectionalIndicator

    // 代表真/假判斷的材質
    public Material trueMaterial;
    public Material falseMaterial;

    // 當前物件的渲染器
    private Renderer dataRenderer;
    // 引用 GameStatsManager
    private GameManager gameManager;
    // 用於記錄初始生成位置的靜態變數

    private static Vector3 defaultPosition;
    // 標誌是否已記錄初始位置
    private static bool isDefaultPositionSet = false;
    // 靜態列表，用於存儲所有的 data 物件
    private static List<GameObject> allDataObjects = new List<GameObject>();

    // 初始化物件
    private void Start()
    {
        // 獲取當前物件的渲染器
        dataRenderer = GetComponent<Renderer>();

        // 獲取 GameStatsManager
        gameManager = FindObjectOfType<GameManager>();

        // 如果尚未記錄初始生成位置，記錄當前物件的位置
        if (!isDefaultPositionSet)
        {
            defaultPosition = transform.position;
            isDefaultPositionSet = true;
        }
        // 從資料夾載入材質
        LoadMaterials();

        // 為第一個物件分配材質
        if (materials.Count > 0 && dataRenderer != null)
        {
            AssignRandomMaterial(dataRenderer);
        }
        else
        {
            Debug.LogWarning("材質列表為空或未能找到渲染器，無法為第一個物件分配材質！");
        }

        // 將當前物件加入列表
        allDataObjects.Add(gameObject);
    }

    private void LoadMaterials()
    {
        // 從 Resources 資料夾載入所有材質
        Material[] loadedMaterials = Resources.LoadAll<Material>(materialsFolderPath);

        // 如果有載入成功，加入材質列表
        if (loadedMaterials.Length > 0)
        {
            materials.AddRange(loadedMaterials);
            Debug.Log($"成功載入 {materials.Count} 個材質");
        }
        else
        {
            Debug.LogError($"未能從資料夾 {materialsFolderPath} 載入材質！");
        }
    }

    // 當物件進入觸發區域時執行
    private void OnTriggerEnter(Collider other)
    {
        // 判斷觸發區域的標籤並進行材質檢查
        if (other.CompareTag("TruePlane"))
        {
            EvaluateMaterial("True"); // 檢查是否是 True 材質
        }
        else if (other.CompareTag("FalsePlane"))
        {
            EvaluateMaterial("False"); // 檢查是否是 False 材質
        }
    }

    // 檢查當前物件的材質是否與目標材質一致
    private void EvaluateMaterial(string expectedPrefix)
    {
        if (gameManager == null || gameManager.IsGameEnded()) return; // 如果遊戲結束，停止處理

        // 獲取當前物件的材質名稱
        string materialName = dataRenderer.sharedMaterial.name;

        // 判斷材質名稱是否以指定前綴開頭
        bool isCorrect = materialName.StartsWith(expectedPrefix);

        // 記錄判斷結果
        gameManager.RecordResult(isCorrect);

        Debug.Log(isCorrect
            ? $"判斷正確！材質名：{materialName} 與前綴 {expectedPrefix} 匹配"
            : $"判斷錯誤！材質名：{materialName} 與前綴 {expectedPrefix} 不匹配");

        // 繼續處理物件
        DataPass();
    }


    // 處理物件生成和當前物件的銷毀
    private void DataPass()
    {
        if (gameManager != null && gameManager.IsGameEnded()) return;

        // 生成新物件，使用記錄的初始位置
        GameObject newData = Instantiate(dataPrefab, defaultPosition, Quaternion.Euler(0, 180, 0));

        // 隨機為新生成的物件分配材質
        AssignRandomMaterial(newData.GetComponent<Renderer>());

        // 找到 DirectionalIndicator 並更新其目標
        DirectionalIndicator indicator = FindObjectOfType<DirectionalIndicator>();

        if (indicator != null)
        {
            indicator.SetDirectionalTarget(newData.transform);
        }

        // 禁用當前物件，避免其繼續執行其他邏輯
        gameObject.SetActive(false);

        // 銷毀當前物件
        Destroy(gameObject);
    }

    // 隨機分配材質給指定的渲染器
    private void AssignRandomMaterial(Renderer renderer)
    {
        if (materials.Count == 0)
        {
            Debug.LogError("材質列表為空，無法分配材質！");
            return;
        }

        // 隨機選擇一個材質
        int randomIndex = Random.Range(0, materials.Count);
        renderer.material = materials[randomIndex];

        Debug.Log($"分配材質：{materials[randomIndex].name}");
    }

    // Reset 方法，將所有 data 物件移動回 defaultPosition
    public static void Relocate()
    {
        foreach (GameObject data in allDataObjects)
        {
            if (data != null)
            {
                data.transform.position = defaultPosition;
            }
        }

        Debug.Log("所有物件已重置到初始位置");
    }
}
