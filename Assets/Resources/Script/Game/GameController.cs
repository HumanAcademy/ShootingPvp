using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[AddComponentMenu("Scripts/Controller/GameController")]
public class GameController : AllController
{
    #region プレイヤー関係
    const float PLAYER_SIZE = 0.2f; 
    Vector3[] startPos = {new Vector3(0f,-10f,POSITION_Z), new Vector3(0f, 10f, POSITION_Z) };
    Vector3[] startAngle = {new Vector3(-90f,0f,0f), new Vector3(90f,180f, 0f) };
    #endregion

    #region ステージ関係
    Camera playerC;                         //プレイヤーのカメラ
    public const float POSITION_Z = 25f;    //カメラとオブジェクトZ軸の距離
    BoxCollider[] boxColliders;             //画面端と中央のコライダー
    const float BOX_THICKNESS = 5f;     //コライダーの分厚さ
    const float BOX_CENTER_THICKNESS = 2f;  //中央のコライダーの分厚さ
    const float COLLIDER_RANGE_Z = 10f;
    #endregion

    #region UI関係

    #endregion

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        //Playerのカメラを取得
        foreach (Transform child in GameObject.FindGameObjectWithTag("MainCamera").transform)
        {
            playerC = child.gameObject.GetComponent<Camera>();
            break;
        }

        #region ボックスコライダーの追加
        Vector2 vec2 = CameraWandH();
        boxColliders = CreateBoxCol(vec2.y, vec2.x);
        #endregion

        PlayerCreate();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SceneLoad(SceneMode.Choice);
        }
    }

    #region 関数

    void PlayerCreate()
    {
        for(int i = 0; i < 2;i++)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>(FOUNDATION_PATH));
            go.transform.position = startPos[i];
            go.transform.eulerAngles = startAngle[i];

            Renderer rend = go.GetComponent<Renderer>();
            rend.material = Resources.Load<Material>(materialPath + i.ToString());
            rend.material.mainTexture = textures[PlayerPrefs.GetInt((PLAYER_COLOR_PATH) + i.ToString())];

            go.GetComponent<MeshFilter>().mesh = meshes[PlayerPrefs.GetInt((PLAYER_MESH_PATH) + i.ToString())];
            go.transform.localScale = Vector3.one * PLAYER_SIZE;

            go.name = "Player" + (i + 1).ToString();
            go.tag = "Player" + (i + 1).ToString();

            go.AddComponent<Player>();
        }
    }

    #region コライダーの追加
    BoxCollider[] CreateBoxCol(float height, float wide)
    {
        List<BoxCollider> boxCol = new List<BoxCollider>();

        boxCol.Add(CreateBoxUp(height, wide));
        boxCol.Add(CreateBoxUp(-height, wide));
        boxCol.Add(CreateBoxSide(wide, height));
        boxCol.Add(CreateBoxSide(-wide, height));
        boxCol.Add(CreateBoxCenter(wide));

        return boxCol.ToArray();
    }

    //上下のコライダー追加用
    BoxCollider CreateBoxUp(float up, float wide)
    {
        BoxCollider box = gameObject.AddComponent<BoxCollider>();
        box.center = new Vector3(0f, up + (BOX_THICKNESS / 2 * (up / Mathf.Abs(up))), POSITION_Z);
        box.size = new Vector3((wide + BOX_THICKNESS) * 2, BOX_THICKNESS, COLLIDER_RANGE_Z);

        return box;
    }

    //左右のコライダー追加
    BoxCollider CreateBoxSide(float right, float height)
    {
        BoxCollider box = gameObject.AddComponent<BoxCollider>();
        box.center = new Vector3(right + (BOX_THICKNESS / 2 * (right / Mathf.Abs(right))), 0f, POSITION_Z);
        box.size = new Vector3(BOX_THICKNESS, height * 2, COLLIDER_RANGE_Z);

        return box;
    }

    //中央のコライダー追加用
    BoxCollider CreateBoxCenter(float wide)
    {
        BoxCollider box = gameObject.AddComponent<BoxCollider>();
        box.center = new Vector3(0f, 0f, POSITION_Z);
        box.size = new Vector3(wide * 2, BOX_CENTER_THICKNESS, COLLIDER_RANGE_Z);

        return box;
    }
    #endregion

    #region カメラの範囲

    //Vecter2へ変換
    Vector2 CameraWandH()
    {
        float height = CameraHeight();
        float wide = CameraWide(height);

        return new Vector2(wide, height);
    }

    //横の決定
    float CameraWide(float height)
    {
        float h = Screen.width * playerC.rect.width;
        float w = Screen.height * playerC.rect.height;
        float area = h / w;

        return area * height;
    }
    //縦の決定
    float CameraHeight()
    {
        float deg = Mathf.Deg2Rad * (90f - (playerC.fieldOfView / 2));
        float i = POSITION_Z / Mathf.Sin(deg);
        return i * Mathf.Cos(deg);
    }
    #endregion

    #endregion

}
