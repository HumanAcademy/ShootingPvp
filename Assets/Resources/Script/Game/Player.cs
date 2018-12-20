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
    Rigidbody rBody;
    #endregion

    #region 機体のステータス
    [HideInInspector] public int number = 1;                //プレイヤーの番号
    [HideInInspector] public float speed = 5f;              //基本の速さ
    [HideInInspector] public float trunMoveSpeed = 6f;      //回転時のX方向の速さ
    [HideInInspector] public float invincibleTime = 1f;     //回転時の無敵時間
    [HideInInspector] public int trunCount = 20;            //回転の回数 
    [HideInInspector] public bool invincibleFlag = false;   //無敵かどうかの判断
    [HideInInspector] public float moveAngle = 30f;         //移動中の角度の最大
    [HideInInspector] public float moveAngleSpeed = 1f;     //移動中の角度の変更速度360°を回る時間
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
        if (Input.GetKeyDown(KeyCode.Return) &&
            trunCor == null)
        {
            trunCor = TrunCor();
            StartCoroutine(trunCor);
        }
    }

    //場所移動
    void PositionMove()
    {
        // float x = gCon.AxisGet(XBox.AxisStr.LeftJoyRight,number);
        // float y = gCon.AxisGet(XBox.AxisStr.LeftJoyUp, number);
        //入れ替え予定

        float x = xp;
        float y = yp;

        float xPower = Mathf.Abs(x);
        float yPower = Mathf.Abs(y);

        Vector2 velosity =
            new Vector2(x, y).normalized;

        velosity.x *= invincibleFlag ? trunMoveSpeed : speed * xPower;
        velosity.y *= speed * yPower;

        if(!invincibleFlag)
        transform.eulerAngles = NextAngle(xPower, transform.eulerAngles);

        rBody.velocity = velosity.x * right + velosity.y * foward;
    }
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

    Vector3 NextAngle(float power,Vector3 now)
    {
        float nextZ = power * moveAngle;
        return new Vector3(now.x,now.y,Mathf.MoveTowards(now.z,nextZ,360f / moveAngleSpeed * Time.deltaTime));
    }

    #endregion

    #endregion

    #region コルーチン

    //回転(回避)
    IEnumerator TrunCor()
    {
        //回転方向の決定
        int axis = gCon.BoolToInt(xp > 0);
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
