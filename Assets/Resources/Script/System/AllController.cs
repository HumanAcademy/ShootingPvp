using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

[AddComponentMenu("Scripts/Controller/XBoxController")]
public class AllController : MonoBehaviour
{
    #region プレイヤー関係
    [HideInInspector] public PlayerData[] player;
    [HideInInspector] public Mesh[] meshes;
    [HideInInspector] public Texture[] textures;
    #endregion

    #region パス関係(String)
    public const string materialPath = "Model/Material/Material";
    public const string PLAYER_MESH_PATH = "Mesh";
    public const string PLAYER_COLOR_PATH = "Color"; 
    public static string nextSceneName;
    public const string PAD = "Pad";
    public const string COLOR_PATH = "Model/Color";
    public const string MESH_PATH = "Model/Mesh";
    public const string FOUNDATION_PATH = "Model/Foundation/Model";
    #endregion

    public enum SceneMode
    {
        Title,
        Choice,
        Game,
        Load
    }


    //通常ボタン
    public enum Str
    {
        LB,
        RB,
        Select,
        Start,
        A,
        B,
        X,
        Y
    }
    
    #region Axisボタン

    public enum AxisStr
    {
        LeftJoyUp,
        LeftJoyRight,
        LeftButtonUp,
        LeftButtonRight,
        RightJoyUp,
        RightJoyRight,
        BackTrigger,
    }

    public class Axis
    {
        public bool flag = false;
        public bool flagLog = false;
        public string name;
    }

    public Axis[][] axis;

    #endregion

    public virtual void Start()
    {
        meshes = MeshsLoad();
        textures = Resources.LoadAll<Texture>(COLOR_PATH);
        AxisButtonStart();
    }

    public virtual void Update()
    {
        AxisButtonLog();
    }

    //関数---------------------------

    #region ロード関係
    public void SceneLoad(SceneMode mode)
    {
        SceneManager.LoadScene(mode.ToString());
    }

    #endregion

    #region ボタンの定義と必要なこと

    //Axisボタンの定義
    void AxisButtonStart()
    {
        axis = new Axis[2][];
        for (int i = 0; i < axis.Length; i++)
        {
            axis[i] = new Axis[Enum.GetValues(typeof(AxisStr)).Length];
            for (int j = 0; j < axis[i].Length; j++)
            {
                axis[i][j] = new Axis();
                axis[i][j].name = PAD + (i + 1).ToString() + ((AxisStr)Enum.ToObject(typeof(AxisStr), j)).ToString();
                axis[i][j].flag = false;
            }
        }
    }

    //ボタンのログ取得
    void AxisButtonLog()
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < axis[i].Length; j++)
            {
                axis[i][j].flagLog = axis[i][j].flag;
                if (Input.GetAxisRaw(axis[i][j].name) != 0f)
                {
                    axis[i][j].flag = true;
                }
                else
                {
                    axis[i][j].flag = false;
                }
            }
        }
    }

    #endregion

    #region 通常ボタンの当たり判定

    public bool ButtonDown(Str button, int Num)
    {
        return Input.GetButtonDown(PAD + Num.ToString() + button.ToString());
    }

    public bool ButtonUp(Str button, int Num)
    {
        return Input.GetButtonUp(PAD + Num.ToString() + button.ToString());
    }

    public bool Button(Str button, int Num)
    {
        return Input.GetButton(PAD + Num.ToString() + button.ToString());
    }

    #endregion

    #region Axisボタンの判定

    public bool ButtonDown(AxisStr button, bool Plus, int Num)
    {
        if (Input.GetAxisRaw(axis[Num][(int)button].name) > 0f && !Plus ||
            Input.GetAxisRaw(axis[Num][(int)button].name) < 0f && Plus ||
            Input.GetAxisRaw(axis[Num][(int)button].name) == 0f)
        {
            return false;
        }

        if (axis[Num][(int)button].flagLog)
        {
            return false;
        }
        
        return true;
    }

    public bool ButtonUp(AxisStr button, bool Plus, int Num)
    {
        if (Input.GetAxisRaw(axis[Num][(int)button].name) < 0f && Plus ||
            Input.GetAxisRaw(axis[Num][(int)button].name) > 0f && !Plus)
            return false;

        if (axis[Num][(int)button].flag ||
            !axis[Num][(int)button].flagLog)
            return false;

        return true;
    }

    public bool Button(AxisStr button, bool Plus, int Num)
    {
        if (Input.GetAxisRaw(axis[Num][(int)button].name) < 0f && Plus ||
            Input.GetAxisRaw(axis[Num][(int)button].name) > 0f && !Plus)
            return false;

        if (!axis[Num][(int)button].flag)
            return false;

        return true;
    }

    public float AxisGet(AxisStr button, int Num)
    {
        return Input.GetAxisRaw(axis[Num][(int)button].name);
    }

    #endregion
    //-------------------------------

    #region 汎用関数(いろいろなところで使う関数)

    #region プレイヤーの見た目関係
    //プレイヤー用のMeshの取得
    Mesh[] MeshsLoad()
    {
        GameObject[] go = Resources.LoadAll<GameObject>(MESH_PATH);
        List<Mesh> meshes = new List<Mesh>();
        for (int i = 0; i < go.Length; i++)
        {
            meshes.Add(go[i].GetComponent<MeshFilter>().sharedMesh);
        }
        return meshes.ToArray();
    }

    public void MeshCreate(ref MeshFilter mf, int i)
    {
        mf.mesh = meshes[i];
    }

    public void ColorCreate(ref Renderer rend, int i)
    {
        rend.material.mainTexture = textures[i];
    }

    #endregion

    //trueを1　falseを-1として返す
    public int BoolToInt(bool flag)
    {
        return Convert.ToInt32(flag) * 2 - 1;
    }

    //選択用※Plusには1か-1しか入らない
    public int Choice(ref int i,int Plus,int Max)
    {
        i += Plus;

        if (i < 0)
            i = Max - 1;

        else if (i >= Max)
             i = 0;

        return i;
    }

    #endregion
}
