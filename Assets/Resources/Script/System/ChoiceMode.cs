using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceMode : AllController
{

    #region 定数(固定のもの)
    Vector3 playerAngle = new Vector3(-90f, 0f, 0f);
    Vector3 player1Pos = new Vector3(-5f, 0f, 10f);
    Vector3 player2Pos = new Vector3(5f, 0f, 10f);
    #endregion

    public override void Start()
    {
        base.Start();
        player = GetPlayerData();
    }

    public override void Update()
    {
        base.Update();
        //1P
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MeshCreate(ref player[0], true);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MeshCreate(ref player[0], false);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ColorCreate(ref player[0], true);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ColorCreate(ref player[0], false);
        }

        //2P
        if (Input.GetKeyDown(KeyCode.D))
        {
            MeshCreate(ref player[1], true);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            MeshCreate(ref player[1], false);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            ColorCreate(ref player[1], true);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            ColorCreate(ref player[1], false);
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            for(int i = 0;i < 2;i++)
            {
                PlayerPrefs.SetInt(PLAYER_MESH_PATH + i.ToString(),player[i].meshNum);
                PlayerPrefs.SetInt(PLAYER_COLOR_PATH + i.ToString(),player[i].materialNum);
            }
            SceneLoad(SceneMode.Game);
        }
    }
    #region 関数

    //プレイヤーのモデルを取得
    PlayerData[] GetPlayerData()
    {
        List<PlayerData> data = new List<PlayerData>();

        for (int i = 1; i < 3; i++)
        {
            PlayerData pd = new PlayerData();
            GameObject go = Instantiate(Resources.Load<GameObject>(FOUNDATION_PATH));
            pd.GameObjectCreate(ref go,i);
            MeshCreate(ref pd.meshFil,0);
            ColorCreate(ref pd.rend,0);
            pd.go.transform.eulerAngles = playerAngle;
            data.Add(pd);
        }

        data[0].go.transform.position = player1Pos;
        data[1].go.transform.position = player2Pos;

        return data.ToArray();
    }

    #region 見た目(機体)変更

    //Mesh関係---------------------------------------------------

    public void MeshCreate(ref PlayerData pd, bool Plus)
    {
        Choice(ref pd.meshNum, BoolToInt(Plus), meshes.Length);

        pd.meshFil.mesh = meshes[pd.meshNum];
    }

    //色関係------------------------------------------------------
    public void ColorCreate(ref PlayerData pd, bool Plus)
    {
        Choice(ref pd.materialNum, BoolToInt(Plus),textures.Length);
        pd.rend.material.mainTexture = textures[pd.materialNum];
    }

    #endregion
    #endregion
}

public struct PlayerData
{
    public Renderer rend;
    public MeshFilter meshFil;
    public GameObject go;
    public int meshNum;
    public int materialNum;
    public void GameObjectCreate(ref GameObject gameObject,int i)
    {
        go = gameObject;
        rend = go.GetComponent<Renderer>();
        rend.material = Resources.Load<Material>(AllController.materialPath + i.ToString());
        meshFil = go.GetComponent<MeshFilter>();
        go.transform.localScale = Vector3.one * 0.5f;
        meshNum = 0;
        materialNum = 0;
    }
}

