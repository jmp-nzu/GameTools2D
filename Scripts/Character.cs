using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;


// この１行で「Capsule Collider 2D」コンポーネントを必須にする。
// このPlayerコンポーネントをゲームオブジェクトに付与した時に
// 「Capsule Collider 2D」がすでに同じゲームオブジェクトに付いていなければ、
// Unityが自動的に追加してくれる。
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))] // Rigidbody2Dも同様に必須に
[RequireComponent(typeof(PlayerInput))] // PlayerInputも同様に必須
[RequireComponent(typeof(Animator))] // PlayerInputも同様に必須
[RequireComponent(typeof(SpriteRenderer))] // SpriteRendererも同様に必須
public class Character : MonoBehaviour
{
    // 以下の変数（プロパティ）は「public」として宣言されている。
    // これは、「外部から使える」という意味で、Unityの場合、「public」
    // として宣言されたプロパティはインスペクタで表示される。

    public float jumpForce = 15f; // ジャンプの勢い

    public float jumpGravity = 0.5f; // ジャンプ中の重力の変化（これでプレーヤーがジャンプの高さを調整する）

    public float walkSpeed = 2f; // 歩く最高速度（ユニット/秒）
    public float walkAcceleration = 100f; // 歩く加速度

    public float runSpeed = 4f; // 走る時の最高速度

    public float runAcceleration = 100f; // 走る時の加速度
    public float airAcceleration = 100f; // 空中移動の加速度

    public float breakForce = 100f; // 止まるための力
    public float maxGroundDistance = 0.1f; // 地面に立っているかどうかの判定に使う閾値。
    public float maxGroundAngle = 45; // 登れる坂の最高角度

    // 斜面に立つと加速度を補正することができる。
    public float slopeAccelerationAdjustment = 2f;

    public bool flipSprite = true; // キャラクターの移動方向に合わせてスプライトの向きを変えるか

    public Action OnDied;
    public Action OnStart;


    // 以下のプロパティはpublicだが、インスペクタに表示されず、スクリプト専用
    [HideInInspector]
    public Vector2 forward = Vector2.right;    

    // 以下の変数（プロパティ）はゲームオブジェクトに付与されている
    // コンポーネントをアクセスするために用意する。このスクリプトで
    // 使いたいコンポーネントを全部このように参照する変数を用意する。
    CapsuleCollider2D capsuleCollider;
    Rigidbody2D rb;
    PlayerInput playerInput;
    Animator animator;
    SpriteRenderer spriteRenderer;


    // 以下の変数は内部処理用でスクリプトの外部からアクセスすることはない。

    bool isGrounded = false; // 地面に立っているか?
    bool isJumping = false; // ジャンプ中？

    bool isRunning = false; // 走り中？

    bool isBreaking = false; // 減速中？

    Vector2 lastGroundedVelocity; // 地面に最後に触れた時の速度。空中移動の処理に使う。
    Vector2 groundNormal; // 地面の法線。これで地面の傾きがわかる。
    Vector2 groundContactPoint; // 地面に触れている座標

    Rigidbody2D groundRigidBody = null; // 地面のオブジェクトに付与されているRigidbody2D、ない場合はnull

    Vector2 movementInput; // プレーヤーが最後に入力した移動
   


    public void Respawn(Vector3 position)
    {
        transform.position = position;
        animator.SetTrigger("respawn");
    }

    public void Die()
    {
        animator.SetTrigger("die");
    }

    void DieComplete()
    {
        OnDied?.Invoke();
    }

    // Startはゲームオブジェクトが生成された時に一度だけ実行される
    // Unityの標準メソッド（関数）。ここに初期化のコードを書く。
    void Start()
    {
        // 最初はこのスクリプトで使う他のコンポーネントを探す。
        // コンポーネントへの参照を記憶する変数を上に宣言したが、
        // ここでその参照を実際に取得しなければならない。
        // Unityでは他のコンポーネントの参照を取得するのは意外と
        // 遅いので極力Startメソッド以外では行わないといい。

        // 「CapsuleCollider2D」という種類のコンポーネントをゲームオブジェクトで
        // 探して、見つけたら「capsuleCollider」という変数の中に記憶する。
        // もし見つからねければ、「null」というC#の「何もない」が変数に代入される。
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        // Rigidbody2DもPlayerInputもAnimatorも同様に
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        OnStart?.Invoke();
    }

    // UpdateはUnityの標準のメソッド（関数）で、ゲームの状態が更新される
    // たびに実行される。通常はフレームレートと連動する。アニメーションや
    // ゲームのロジックをここで更新する。ゲームの最も中心的な処理をここで
    // 行うことが多い。
    void Update()
    {
        // アニメーションだけアップデートする。
        // アニメーションはこのUpdateの後に処理されるので、このタイミングで
        // アニメーションのパラメーターを更新するといい。
        animator.SetFloat("walkInput", movementInput.x);
        animator.SetFloat("yVelocity", rb.velocity.y);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isBreaking", isBreaking);
    }

    // FixedUpdateはUpdateと同様にUnityの標準メソッド。Updateと違って、
    // 必ず一定間隔（1秒に60回）で実行される。ここで物理演算に影響する処理を
    // 行う。
    void FixedUpdate()
    {
        // 地面に立っているかどうかを確認する。
        bool newGroundedState = GetNewGroundedState();
        isGrounded = newGroundedState; // 情報を最新にする
        
        // 止まるかどうかの判定をとりあえずfalseにしておく。
        isBreaking = false;

        // ジャンプしているかどうかの状態を更新する。
        UpdateJumpingState();

        // 重力の影響を調整する。
        AdjustGravityScale();

        // ここからキャラクターの移動を制御するコードがになる。
        // 大きく分けて、地面に立っている時の移動と空中の移動がある。isGroundedで
        // どちらになるかをまず区別する。
        if (isGrounded) { // 地面に立っている
            if (movementInput.magnitude > 0) {
                ApplyGroundedMotion(); // ユーザーから移動の入力があった
            }

            CounterGravity();
        }
        else { // 地面に立っていない
            ApplyAirMotion();
        }
    }

    void UpdateJumpingState()
    {
        // ここでジャンプに関する処理を行う。
        // まず、キャラクターの速度（velocity）のy軸が0以下なら、下へ動いているので、
        // ジャンプが終了している判定にする。これは、重力の調整に影響する。
        if (rb.velocity.y <= 0) {
            isJumping = false;
        }
    }

    void AdjustGravityScale()
    {
        // ジャンプ中に重力を弱くする事によって、ジャンプの高さが調整できる。
        if (isJumping) {
            // Rigidbodyはゲームオブジェクトの物理演算を行う。
            // ここに重力と反対方向の力を加える事によって重力を弱くして、ジャンプの高さを
            // 調整する。
            rb.gravityScale = jumpGravity;
        }
        else {
            rb.gravityScale = 1f; // ジャンプしていないので通常の重力に戻す。
        }
    }

    // 地面に立っているかどうかの状態を更新する。
    // 地面に立っているかどうかという情報は様々なところで使うが、
    // その情報を使うたびに計算してしまうと効率が悪いので、ゲームの
    // 状態が変わった時点で一度だけ計算して、その結果を記憶する。
    bool GetNewGroundedState()
    {
        // 「Bounds」は空間の範囲を表す型。中心点とサイズ（幅、高さ）
        // で後世されている。ここは、コライダのその範囲を取得して記憶する。
        Bounds colliderBounds = capsuleCollider.bounds;

        // 壁に邪魔される事が多いので、地面の衝突判定は実際のコライダより少し
        //
        colliderBounds.size -= Vector3.right * 0.02f;

        // 登れる傾斜の閾値を計算する。
        // 最初は角度を度からラジアンに変換する。
        float maxGroundAngleRadians = maxGroundAngle * (Mathf.PI / 180f);
        // ラジアンの角で、法線のy値の最低値が計算できる。
        float normalThreshold = Mathf.Cos(maxGroundAngleRadians);

        ContactFilter2D filter = new ContactFilter2D(); // 衝突判定に使うパラメータ。初期値のままでいい。
        
        List<RaycastHit2D> hits = new List<RaycastHit2D>(); // これが衝突の情報を格納する配列になる。

        // 衝突判定を行う。この関数は衝突したゲームオブジェクトの数を返す。
        // 渡す引数は：
        // 長方形の中心点、長方形の寸法、長方形の角度、衝突判定の方向、ContactFilter2D、結果を格納する配列、衝突の最高距離
        int count = Physics2D.BoxCast(colliderBounds.center, colliderBounds.size, 0, Vector2.down, filter, hits, maxGroundDistance);

        // 次の処理は、地面の衝突判定が失敗（つまり空中）の前提で始める。
        bool newGroundedState = false;
        groundNormal = Vector2.up; // 空中では地面の法線をデフォルトの真上にする
        groundRigidBody = null;
        groundContactPoint = colliderBounds.center + Vector3.down * colliderBounds.extents.y;
        foreach(RaycastHit2D hit in hits) { // 衝突情報を一つずつ確認する
            // もし、衝突したオブジェクトが自分出なければ、そして、衝突した面の角度が閾値内なら...
            if (hit.collider.gameObject != gameObject) {
                groundNormal = GetGroundNormal(); // 地面の法線を記憶する
                groundContactPoint = hit.point; // 着地した点
                groundRigidBody = hit.rigidbody; // Rigidbody2Dを記憶

                if (groundNormal.y > normalThreshold) {
                    newGroundedState = true; // 地面に立っている
                    break; // 地面として判定された衝突を見つけたので、breakでループから脱却する
                }
            }
        }

        // 新しい「地面に立つ」状態を返す。
        return newGroundedState;
    }

    // 地面の法線ベクトルを所得する。これで地面の傾斜が分かる。
    Vector2 GetGroundNormal()
    {
        Vector2 normal = Vector2.up; // デフォルトは真上（水平の地面）

        // 足元の位置を計算する。
        Vector2 footPosition = capsuleCollider.bounds.center + Vector3.down * capsuleCollider.bounds.extents.y;

        // キャラクターの幅を取得する。
        float width = capsuleCollider.bounds.extents.x;
        
        // 地面として認識される辺のの最大の角度を度からラジアンに変換する。
        float maxGroundAngleRadians = maxGroundAngle * (Mathf.PI / 180f);

        // 便利な閾値を準備する。
        float raycastDistance = Mathf.Tan(maxGroundAngleRadians) * width;
        float normalThreshold = Mathf.Cos(maxGroundAngleRadians);

        // 動いている場合、足元を移動方向にずらす
        if (movementInput.x > 0) {
            footPosition.x += width;
            footPosition.y += width;
            raycastDistance += width;
        } else if (movementInput.x < 0) { // 動いていない場合、足元は中央に
            footPosition.x -= width;
            footPosition.y += width;
            raycastDistance += width;
        }

        // 足元から真下へレイキャストを行って地面を探す
        RaycastHit2D[] hits = Physics2D.RaycastAll(footPosition, Vector2.down, raycastDistance + maxGroundDistance);
        foreach (RaycastHit2D hit in hits) {
            if (hit.collider.gameObject != gameObject) {
                // 自分以外の何かに当たった。これを地面として認識して、法線ベクトルを更新する。
                normal = hit.normal;
                break;
            }
        }

        // 法線ベクトルを返す。
        return normal;
    }

    // 地面に立っている時の移動を適用する
    void ApplyGroundedMotion()
    {
        float speed = rb.velocity.magnitude; // ベロシティ（速度）の長さ（magnitude）は速度の絶対値

        // 傾斜の場合、「前」の方向が傾く。ここは、地面の法線を使って、前進する方向を計算する。
        Vector2 right = new Vector2(groundNormal.y, -groundNormal.x);

        // 最高速度と加速度を設定する
        float maxSpeed = walkSpeed;
        float acceleration = walkAcceleration;
        if (isRunning) {
            maxSpeed = runSpeed;
            acceleration = runAcceleration;
        }

        // 加速度は地面の傾斜によって調整する
        acceleration += slopeAccelerationAdjustment * groundNormal.x / groundNormal.y;

        float groundVelocity = Vector2.Dot(rb.velocity, right); // 地面の傾斜に対する速度を計算する
        float nextGroundVelocity = groundVelocity + movementInput.x * acceleration * Time.deltaTime; // 加速後の速度を予測する
        float nextGroundSpeed = Mathf.Abs(nextGroundVelocity); // 絶対速度

        // もし速度が最高速度を超えて、且つ移動方向と同じ方向へ加速しようとするなら
        if (nextGroundSpeed > maxSpeed && Mathf.Sign(movementInput.x) == Mathf.Sign(groundVelocity)) {
            // 加速度を制限する...
            float deltaSpeed = nextGroundSpeed - maxSpeed;
            acceleration = Mathf.Max(0, acceleration - deltaSpeed / Time.fixedDeltaTime);
        }
        
        rb.AddForce(right * movementInput.x * acceleration, ForceMode2D.Force); // 加速の力を加える

        // 速度を記憶する
        lastGroundedVelocity = rb.velocity;
    }


    // 通常の物理演算では重力の影響を受けて斜面を滑ることがある。
    // このメソッドは滑らない様にキャラクターに力を加える。
    void CounterGravity() 
    {
        // ジャンプしていなければ...
        if (!isJumping) {
            float slipCounter = -Physics2D.gravity.y / 16f; 
            Vector2 antiSlipForce = Vector2.zero;
            Vector2 tangent;
            if (groundNormal.x > 0) {
                tangent = new Vector2(-groundNormal.y, groundNormal.x);
            }
            else {
                tangent = new Vector2(groundNormal.y, -groundNormal.x);
            }
            float slip = Mathf.Abs(tangent.y / tangent.x);
            float theta = Mathf.Acos(groundNormal.y);
            antiSlipForce = tangent * slipCounter * (theta / Mathf.PI);
            rb.AddForce(tangent * slipCounter * (theta / Mathf.PI) / Time.fixedDeltaTime, ForceMode2D.Force);
        }
    }

    // 空中移動を適用する
    void ApplyAirMotion()
    {
        // 空中で移動する場合、速度の制限方法が違う。
        // 最高速度は歩く速度または地面に最後に触れていた時の最高速度の最も早い値。
        float speedLimit = Mathf.Max(Mathf.Abs(lastGroundedVelocity.x), walkSpeed);
        float xSpeed = Mathf.Abs(rb.velocity.x); // 地面の移動と同じ
        bool isTurn = rb.velocity.x < 0 != movementInput.x < 0; // 地面の移動と同じ
        if (isTurn || xSpeed < speedLimit) {
            rb.AddForce(Vector2.Scale(movementInput, Vector2.right) * airAcceleration, ForceMode2D.Force);
        }
    }


    // ここからは入力に対する反応

    // この独自メソッドは入力アクションの「Jump」が実行された時に呼び出される。
    // ジャンプの開始命令になる。
    void OnJump(InputValue input)
    {
        if (!input.isPressed)
        {
            isJumping = false;
        }
        else if (isGrounded) {
            // キャラクターが地面に立っていないとジャンプできない。
            // もし、ダブルジャンプなど、ジャンプができる条件を変えたいなら、
            // ここのif文の条件を変える。

            // Rigidbodyはゲームオブジェクトの物理演算を行う。
            // ここで上方向（Vector2.up）に力を加えて、キャラクターをジャンプさせる。
            // 「ForceMode2D.Impulse」は単発的に力を加える事を意味する。
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumping = true; // ジャンプをしている事を記憶する。

            // アニメーションを切り替える
            animator.SetTrigger("jump");
        }
    }

    // この独自メソッドは入力アクションの「Move」が実行された時に呼び出される。
    // 移動の制御に使う。
    void OnMove(InputValue input)
    {
        // プレーヤーの入力をVector2形式として表す
        Vector2 moveDirection = input.Get<Vector2>();
        movementInput = moveDirection; // 入力を記憶する

        if (Mathf.Abs(movementInput.x) > 0) {
            if (movementInput.x > 0) {
                forward = Vector2.right;
            } else {
                forward = Vector2.left;
            }
        }

        if (flipSprite) {
            spriteRenderer.flipX = forward.x < 0;
        }
    }

    void OnRun(InputValue input)
    {
        isRunning = input.isPressed;
    }

} // クラス定義はここまで
