using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�ý��ۿ��� ������ �����ϱ� ���� DLL
using System.IO;

using System.Runtime.Serialization.Formatters.Binary;

public class DataManager : MonoBehaviour
{
    private static DataManager _instance = null;
    public static DataManager instance { get { return _instance; } }

    public int playerHP = 3;
    public string currentScene = "Level1";

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Save()
    {
        SaveData saveData = new SaveData();
        saveData.sceneName = currentScene;
        saveData.playerHP = playerHP;

        //���ϻ���
        FileStream fileStream = File.Create(Application.persistentDataPath + "/save.dat");

        Debug.Log("���� ���� ����");

        //����ȭ
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(fileStream, saveData);

        //������ �ݴ´�
        fileStream.Close();
    }

    public void Load()
    {
        //������ �ִ��� Ȯ���Ѵ�
        if(File.Exists(Application.persistentDataPath + "/save.dat") == true)
        {
            FileStream fileStream = File.Open(Application.persistentDataPath + "/save.dat", FileMode.Open);

            if(fileStream != null && fileStream.Length > 0)
            {
                //������ȭ
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                SaveData saveData = (SaveData)binaryFormatter.Deserialize(fileStream);
                playerHP = saveData.playerHP;
                UIManager.instance.PlayerHP();
                currentScene = saveData.sceneName;

                fileStream.Close();
            }
        }
    }
}