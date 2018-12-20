using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[AddComponentMenu("Scripts/Game/Player")]
public class Player : MonoBehaviour
{
    [Range(-1f, 1f)] [SerializeField] float xp = 0f;
    [Range(-1f, 1f)] [SerializeField] float yp = 0f;

    #region 機体の情報(変数等)
    [HideInInspector] public int maxLife = 10000;
    [HideInInspector] public int life;
    [HideInInspector] public float energyMax = 10000f;
    [HideInInspector] public float energy = 0f;
    [HideInInspector] public float energyHeel = 500f;
    Rigidbody rBody;
    #endregion

    #region 消費エネルギー
    [HideInInspector] public float trunEnergy = 1000f;
    #endregion

    #region 機体のステータス
    [HideInInspector] public int number = 0;                //プレイヤーの番号
    [HideInInspector] public float speed = 10f;             //基本の速さ
    [HideInInspector] public float trunMoveSpeed = 15f;     //回転時のX方向の速さ
    [HideInInspector] public float invincibleTime = 0.4f;   //回転時の無敵時間
    [HideInInspector] public int trunCount = 2;             //回転の回数 
    [HideInInspector] public bool invincibleFlag = false;   //無敵かどうかの判断
    [HideInInspector] public float moveAngle = -20f;        //移動中の角度の最大
    #endregion

    #region その他
    GameController gCon;
    IEnumerator trunCor;
    Vector3 defaultAngle;
    Vector3 foward;
    Vector3 right;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        #region　初期化
        life = maxLife;
        gCon = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        rBody = AddRigidBody();
        defaultAngle = transform.eulerAngles;
        foward = transform.forward;
        right = transform.right;
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        EnergyMove();
        Move();
    }

    #region 関数

    #region　動き関係
    //動きのまとめ
    void Move()
    {
        PositionMove();
        TrunMove();
    }

    //回転
    void TrunMove()
    {
        if (gCon.ButtonDown(AllController.AxisStr.BackTrigger,true, number) &&
            trunCor == null||
            gCon.ButtonDown(AllController.AxisStr.BackTrigger, false, number) &&
            trunCor == null)
        {
            if(EnergyDown(trunEnergy))
            {
                trunCor = TrunCor(gCon.AxisGet(AllController.AxisStr.BackTrigger,number) > 0f);
                StartCoroutine(trunCor);
            }
        }
    }

    //場所移動
    void PositionMove()
    {
        float x = gCon.AxisGet(AllController.AxisStr.LeftJoyRight,number);
        float y = -gCon.AxisGet(AllController.AxisStr.LeftJoyUp, number);

        Vector2 velosity =
            new Vector2(x, y);

        velosity.x *= invincibleFlag ? trunMoveSpeed : speed;
        velosity.y *= speed;

        if (x != 0 && !invincibleFlag)
        transform.eulerAngles = NextAngle(x * moveAngle);

        rBody.velocity = velosity.x * right + velosity.y * foward;
    }
    #endregion

    #region エネルギー関係

    //まとめ
     void EnergyMove()
    {
        EnergyUp(Time.deltaTime * energyHeel);
    }

    //エネルギー回復
    void EnergyUp(float i)
    {
        energy = Mathf.Clamp(energy + i, 0f, energyMax);
    }

    //エネルギー消費
    bool EnergyDown(float i)
    {
        if(energy - i >= 0)
        {
            energy = Mathf.Clamp(energy - i, 0f, energyMax);
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #endregion
    //RigidBodyの要素の定義
    Rigidbody AddRigidBody()
    {
        Rigidbody rBody = gameObject.AddComponent<Rigidbody>();
        rBody.mass = 0f;
        rBody.angularDrag = 0f;
        rBody.useGravity = false;
        rBody.constraints = RigidbodyConstraints.FreezeRotation;
        rBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        return rBody;
    }

    #region 旋回の回転

    Vector3 NextAngle(float angle)
    {
        return defaultAngle + new Vector3(0f, 0f, angle);
    }

    #endregion

    #region コルーチン

    //回転(回避)
    IEnumerator TrunCor(bool flag)
    {
        //回転方向の決定
        int axis = gCon.BoolToInt(flag);
        int count = 0;
        Vector3 angle = defaultAngle;
        invincibleFlag = true;

        while (true)
        {
            angle = Vector3.MoveTowards(angle,
                new Vector3(angle.x, angle.y, 360f * axis), 360f / invincibleTime * trunCount * Time.deltaTime);

            transform.eulerAngles = angle;

            if (angle.z == 360f ||
                angle.z == -360f)
            {
                angle.z = 0f;
                if (count == trunCount - 1)
                {
                    transform.eulerAngles = defaultAngle;
                    break;
                }
                else
                {
                    count++;
                }
            }
            yield return null;
        }

        trunCor = null;
        invincibleFlag = false;
        yield break;
    }

    #endregion
}
