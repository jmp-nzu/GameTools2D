/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

/* 以下は「using」キーワードから始まる行はこのファイルで追加で使用する機能を
 * 読み込みます。「System」から始まるのはC#の標準機能です。（厳密にいうと、
 * その標準ライブラリの.NETまたはMonoの機能です。）「UnityEngine」から始まるのは
 * 文字通りにUnityの機能です。*/
using System.Collections; // 基本的なデータ構造のサポート機能
using System.Collections.Generic; // ListやDictionaryなど、データ構造
using UnityEngine; // Unityの基本的な機能
using UnityEngine.SceneManagement; // Unityのシーン管理機能は個別に追加する必要があります。

/* このクラスは2つの関連する役割があります。複雑なゲームの場合、分けて作った方が
 * いいですが、ここではまとめて実装しました。
 * 一つ目の役割はシーンの切り替えです。シーンを切り替える時に様々な処理が必要になるので、
 * ここで行います。
 * 2つ目の枠割は、シーンを切り替えてもリロードしても残したい情報の管理です。ここで
 * 実装しているのは：
 * 1. チェックポイントのスポーンの位置の記憶
 * 2. リロードした時にプレーヤーを正しいスポーン位置に移動
 * 3. チェックポイントなどでユーザーの所持アイテムの記憶
 * 4. リスポーンした時に、所持アイテムの復活
 * 5. スポーンした時にチェックポイントまで拾ったアイテムをフィールｓドから削除する */
public class SceneControl : MonoBehaviour
{
    /**
     * シーンが変わっても所持アイテムデータを消すか？という設定を用意します。
     * これは、新しいシーンをロードすることと現在のシーンをリロードすることを別扱いになります。
     * この設定は新しいシーンに移行した時のみ有効です。
     */
    public bool clearItemsOnLoad = false;

    /**
     * このDictionaryでプレーヤーの所持アイテムの情報を記憶します。記憶するタイミングをチェックポイント
     * などで設定できるようにしたいので、このデータをリアルタイムのプレーヤーの所持アイテム情報を分ける
     * 必要があります。
     * 
     * フィールドに配置されているアイテムにそれぞれの個別IDが付いています。（これをUID（ユニークID）といいます。）
     * このIDの型はstringで通常のリスト（List<string>）で記憶してもよかったですが、各アイテム毎に所持している個数も
     * 記憶しなければなりません。これで、Inventoryと同じ形で、Dictionaryを利用してアイテムの種類と個数を記憶します。
     * 
     * で、本当はこのDictionary一つでもよかったですが、さらに別Dictionaryの中に入っています。
     * 
     * なんで？
     * 
     * 実はこのスクリプトはマルチプレーヤー対応です。これでいろいろややこしくなります。
     * 
     * 各プレーヤーにもUIDがあります。プレーヤーの場合、現状の実装ではuidがUnityのヒエラルキービューに表示されている
     * オブジェクト名です。プレーヤーが複数いてもオブジェクト名が同じなら所持アイテムデータがごちゃ混ぜになって
     * しまいます。これはこのスクリプトの弱いところの一つです。JSON風にまとめるとこんな感じになります：
     * 
     * savedItems = {
     *        "Player1" : {"uid1": 1, "uid5": 3, "uid10": 2},
     *        "Player2" : {"uid2": 10, "uid3": 1}
     * }
     */
    Dictionary<string, Dictionary<string, int>> savedItems = new Dictionary<string, Dictionary<string, int>>();

    /**
     * 各プレーヤーが所持しているアイテムだけでなく、フィールドから消えたアイテムも記憶する必要があります。
     * なぜなら、プレーヤーの所持アイテムの消化があるかも知れないからです。消えたアイテムは所持した
     * プレーヤーの情報を記憶しなくてもいいです。ここで前期のアイテムの固有ID（uid）の出番です。
     * 
     * 拾われた、フィールドから消えたアイテムのリストを管理したいですが、同じuidのアイテムが複数回記憶される
     * のを避けるために、ListではなくHashSetを使います。これは、リストと同様に同型のオブジェクトを記憶しますが、
     * 重複を許しません。
     */
    HashSet<string> deletedItems = new HashSet<string>();

    /**
     * こちらはスポーン位置を記憶するデータ構造です。所持アイテム同様にプレーヤー毎に保存されるので、Dictionaryを
     * 利用しています。ただ、記憶するのは1つの位置だけなので、値の型は単純にVector3です。
     * ここで作っているのは2Dゲームですが、Unityは2Dも3D空間で描画しています。（つまり疑似2Dです。）Vector2でも
     * よかったですが、ゲームオブジェクトの位置は最終的にVector3になるので、ここままにします。
     */
    Dictionary<string, Vector3> spawnPositions = new Dictionary<string, Vector3>();

    /**
     * これから少し高度な話になります。でも、このスクリプトが正しく動作するために最もすっきりした作り方ですので、
     * この技を覚えると便利です。
     * オブジェクト指向プログラミングをする際、いくつもの「デザインパターン」を使います。これは、作業で
     * よく出てくる問題の解決策です。つまり、プログラミング言語や環境の機能というよりもプログラマーが問題を解決
     * するための戦略です。
     * 
     * Unityでは、シーンをリロードした際、すべてのオブジェクトが一度削除されてしまいます。普段、それが大きな
     * 問題ではないですが、このスクリプトで記憶したデータがすべて消えて、本来の目的が果たせない訳です。
     * 解決方法は「 DontDestroyOnLoad(gameObject)」を呼び出すことです。これを実行すれば、このスクリプトが付与
     * されているゲームオブジェクトはシーンが切り替わっても削除されません。
     * 
     * しかし、これで別の問題が発生してしまいます。シーンをリロードする度に、新しいSceneControlが生成されるじゃないか！
     * とそのうちに気づきます。
     * 
     * 本当は何があってもSceneControlが複数あったら困るので、強制的に複数作らせない仕組みをこの下で実装しました。
     * 「同種類（同クラス）のオブジェクトを複数作らせない仕組み」はSingleton Patternといいます。このデザインパターンは
     * ネットで調べてみると「使うな」、「そもそもデザインパターンじゃねぇ」と否定的なプログラマの存在を知ることになります。
     * 彼らの言い分は分かりますが、まあ、ある種の原理主義ではないでしょうか。プログラミングの世界ではそういうのが多いです。
     * 要するにここではSingletonパターンが最もシンプルに問題を解決してくれます。
     * 
     * まず、SceneControlは強制的に複数存在しないので、唯一無二のSceneControlを一つの変数やプロパティに記憶することが
     * できます。これのことが「singleton」です。型が「SceneControl」で、プロパティ名を「singleton」にしましたが、
     * 「instance」という名前もよく見ます。ここで重要なのは「static」キーワードです。これは、プロパティがクラスその物に
     * 付いていることを意味します。使い方は：
     * 
     * SceneControl.singleton
     * 
     * これはプログラムのどこからでも使えます。Unityで同様にstaticとして宣言され、singleton扱いされるのは「メインカメラ」です。
     * カメラが複数あってもUnityではメインカメラが必ず1つだけです。つまり、singletonです。こう使います。
     * 
     * Camera.main
     * 
     * 最初はsingletonをnullにします。つまり、SceneControlのオブジェクトが最初からないということです。
     */
    public static SceneControl singleton = null;

    /**
     * Singletonの実装はこちらに続きます。AwakeはStartと同様に、コンポーネントが生成された時に一度だけ呼び出されます。
     * Awakeが他のすべてのStartよりも早く呼び出されるのは唯一の違いです。ここで、他のSceneControlがすでに存在しているか
     * を確認します。もしあったら、後から作成されたオブジェクトを削除して、singleton以外残さないようにします。
     */
    void Awake()
    {
        if (singleton == null) // singletonがまだない！
        {
            // ここまで来たらこのオブジェクトが初めて生成されるSceneControlになるので、singletonとして記憶します。
            singleton = this; // thisは「これ」、つまり「このオブジェクト」です。
            DontDestroyOnLoad(gameObject); // シーンがリロードされても削除しないようにします。
        }
        else // singletonがすでにある！
        {
            // 悪いけど、お前はいらない。削除するぞ。
            Destroy(gameObject);
        }
    }　// Singletonの実装はここまでです。

    /**
     * このStartはsingletonパターンによって、ゲームで一度だけ実行されることになります。
     */
    void Start()
    {
        // シーンの初期化を行います。
        InitializeScene();
    }


    /**
     * シーンの初期化はここで実装します。
     */
    void InitializeScene()
    {
        /** 
         * 最初はシーンにあるすべての「Item」を探します。公式ドキュメントにも書いてありますが、
         * このメソッドはめちゃくちゃ遅いので、絶対にUpdateで使用してはいけません。シーンがロードされた時に
         * 一度だけ実行しているからいいです。*/
        Item[] items = FindObjectsOfType<Item>();

        /**
         * 同様にすべてのプレーヤーを探します。これは、コンポーネントの種類ではなく、タグを使っているので、
         * もう少し早いけど、やはりこれもUpdateで使わない方がいいです。（バイザウェイ、プレーヤーオブジェクトの
         * タグを正しく設定しなければ、このスクリプトが動作しないので、注意しましょう。）*/
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        /**
         * プレーヤーの初期設定を行います。もちろん、CharacterスクリプトなどのStartで初期化を行いますが、
         * ここでは、SceneControlとCharacterスクリプトの紐づけだけを行います。
         * foreachループででプレーヤーを一つずつ扱います。
         */
        foreach (GameObject player in players)
        {
            // Characterコンポーネントを取得します。
            Character character = player.GetComponent<Character>();

            // 必ず、取得した情報が正しいかどうかを確認する習慣を身につけましょう。
            if (character != null)
            {
                /**
                 * これはC#の「イベント」機能を使います。中級レベルの機能ですが、極めて便利です。
                 * 本当にC#の優れている機能です。使い方を覚えておきましょう。
                 * Characterスクリプトはプレーヤーが死んだ時に「OnDied」という通知を送信します。
                 * 「character.OnDied += Reload」というのはOnDied通知（イベント）に「購読」するように
                 * します。通知が発生されたら、SceneControlの「Reload」メソッドが呼び出されるように
                 * なります。
                 * 
                 * ゲームのプログラムで「何か」が起きるかも知れない、いつ起きるか分からあにけど、起きたら
                 * こういう風に対応したいというのがよくあります。C#のイベント機能を使うと非常にシンプルに
                 * 対応できます。*/
                character.OnDied += Reload;

                /**
                 * プレーヤーのスポーン位置を変える必要があるかを確認します。スポーン位置を記憶している
                 * Dictionaryにただいま処理しているPlayerの名前が登録されていれば、該当の位置を取り出して、
                 * プレーヤーを動かします。
                 * 
                 * ただし！
                 * 
                 * 注意です！
                 * 
                 * プレーヤー側の初期化が終わっていないと、アニメーションなどが正しく動作しません。
                 * プレーヤーの初期化が終わったのを待つのもC#のイベントが便利です。しかし、ここでは
                 * さらに高度な「匿名関数」を使っています。対応したい動作が単純だと匿名関数も便利です。
                 */
                if (spawnPositions.ContainsKey(character.name))
                {
                    /**
                     * ここは典型的な「クロージャ」になっています。これは、匿名関数と一緒に登場する
                     * 概念で、分かりにくいと思う人が多いでしょう。ここで説明するのはさすがに長く
                     * なってしまいそうですので、諸君、各自クロージャを調べまくれ！
                     */
                    Vector3 position = spawnPositions[character.name]; // スポーン位置を取り出す

                    // OnStartイベントによって実行されるのはこの名前を持たない「匿名関数」です。
                    character.OnStart += () =>
                    {
                        /**
                         * この中のコードはOnStartが送信された時に実行されます。クロージャによって、「position」も
                         * characterも使えます。
                         * 「character?」という部分はC#の便利な機能で、以下のコードの略になります：
                         * if (character != null) {
                         *     character.Respawn(position);
                         * }
                         */
                        character?.Respawn(position);
                    };
                }
            }
            else
            {
                // Characterコンポーネントがないじゃないか！
                Debug.LogError("Player（" + player.name + "）にCharacterコンポーネントがない！");
            }

            /**
             * 次は所持アイテムの復活です。プレーヤーに付いているInventoryコンポーネントを探します。
             * ここはCharacterと違って、なくてもエラーにしません。
             */
            Inventory inventory = player.GetComponent<Inventory>();

            /**
             * プレーヤーにInventoryがあって、且つ記憶したデータにUIDが登録されていれば...
             */
            if (inventory != null && savedItems.ContainsKey(inventory.uid))
            {
                // 記憶したアイテム情報をプレーヤーのInventoryにコピーします。
                inventory.items = new Dictionary<string, int>(savedItems[inventory.uid]);

                // 所持アイテムの効果を一斉発揮させます。
                inventory.ExecuteAllActions();
            }
        }

        /**
        　* 次に、フィールドから消えたアイテムを削除します。これは、所持者と関係ないので、
        　* アイテムを一つずつ確認して削除するかどうかをチェックします。
        　*/
        foreach (Item item in items)
        {
            if (deletedItems.Contains(item.uid)) // HashSetに含まれているか？
            {
                Destroy(item.gameObject);
            }
        }
    }

    /**
     * Reloadはプレーヤーが死んだ時など、シーンを再度読み込みたい時に実行します。
     */
    public void Reload()
    {
        /**
         * 現在のシーンをもう一度読み込みます。
         * 
         * 残念ながらここも少し複雑になります。シーンを読み込むのに時間がかかります。
         * Unityがディスクから必要なデータを読み込んだりして、場合によって数秒もかかって
         * しまうかも知れません。シーンの読み込みが完全に終わったら、初期設定をここで
         * 行いたいですが、どう待てばいいでしょうか？
         * 
         * あ、先生、分かった！C#のイベントはどうか？
         * 
         * それはいい提案ですが、Unityはそういう風に作られていません。（涙）
         * 代わりに「Coroutine」を使わなければなりません。使い方はこうです：
         * 
         * 「IEnumerator」という型の戻り値のメソッドを書きます。ここは「SceneLoadCoroutine」です。
         * 引数は自由ですが、戻り値だけが重要です。
         * このメソッドを実行して、返される戻り値をStartCoroutineに渡します。
         * 
         * 説明の続きはSceneLoadCoroutineを見てください。
         */
        string sceneName = SceneManager.GetActiveScene().name;
        StartCoroutine(SceneLoadCoroutine(sceneName));
    }

    /**
     * このメソッドがシーンの読み込みとその後の処理を行います。
     */
    IEnumerator SceneLoadCoroutine(string sceneName)
    {
        /**
         * Unityの「SceneManager.LoadSceneAsync」は指定したシーンを読み込みます。
         * 上にも書いた通り、それは数秒、数分かかるかも知れません。
         * 
         * しかし、ここに「yield return」というキーワードが付いています。そして、returnのあとに
         * コードが続きます。ナニコレ？
         * 
         * 通常、returnを実行すれば、メソッドと関数の実行がそこで止まって、この後にコードを書いても
         * 実行されないはずです。しかし、C#では、「yield return」を使うと、メソッドの実行を「一時停止」
         * させることができます。
         * 
         * シーンの読み込みは長く時間がかかるかも知れないので、終わるまでにこのメソッドの実行を保留に
         * してくれという指示です。そして、シーンの読み込みが終わったら、yield returnの次の行から
         * 実行が再開します。
         * 
         * この「終わるまでに待つ」問題はC#のイベントでも解決できますが、Unityは設計上の理由で、
         * yield returnを使っています。
         */
        yield return SceneManager.LoadSceneAsync(sceneName);

        // シーンの読み込みが終わったので、次は初期設定を改めて行っても安全です。
        InitializeScene();
    }

    /**
     * 新しいシーンを読み込みたい時にこのメソッドを呼び出します。
     */
    public void LoadScene(string name)
    {
        // スポーン位置データをリセットします。
        spawnPositions.Clear();

        // 消えたアイテムもリセットします。
        deletedItems.Clear();

        // 指定があれば、所持アイテムもリセットします。
        if (clearItemsOnLoad)
        {
            savedItems.Clear();
        }

        // リロード同様にシーンを読み込みます。
        StartCoroutine(SceneLoadCoroutine(name));
    }

    public void SaveState(GameObject target)
    {
        Character character = target.GetComponent<Character>();
        Inventory inventory = target.GetComponent<Inventory>();
        
    }

    /**
     * スポーン位置を設定したい時にこのメソッドを呼び出します。
     */
    public void SetSpawnPosition(Character character, Vector3 position)
    {
        if (character != null) // 用心深くコードを書こう！
        {
            // Dictionaryに新しいVector3を記憶します。すでにあれば、上書きされます。
            spawnPositions[character.name] = position;
        }
        
    }

    /**
     * 所持アイテムの現状を記憶したい時にこれを呼び出します。
     */
    public void SaveInventoryItems(Inventory inventory)
    {
        if (inventory != null)
        {
            /**
             * 所持アイテムの情報をコピーします。
             */
            savedItems[inventory.uid] = new Dictionary<string, int>(inventory.items);

            foreach(string uid in inventory.pickedUpItemIds)
            {
                deletedItems.Add(uid);
            }
        }
    }
}
