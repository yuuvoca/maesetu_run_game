using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpawner : MonoBehaviour
{

    public bool isActive = false;

    [SerializeField]
    GameObject wallPrefab = null;
    [SerializeField, Min(0.1f)]
    float defaultMinWaitTime = 1;
    [SerializeField, Min(0.1f)]
    float defaultMaxWaitTime = 1;
    [SerializeField]
    Vector2 defaultMinSize = Vector2.one;
    [SerializeField]
    Vector2 defaultMaxSize = Vector2.one;

    bool isSpawning = false;
    float minWaitTime;
    float maxWaitTime;
    Vector2 minSize;
    Vector2 maxSize;
    Coroutine timer;

    //外部から値を代入するためのプロパティ
    public float MinWaitTime
    {
        set
        {
            //あまりにも小さい値になるとものすごい数の障害物が生成されてしまうので、0.1未満にならないようにする
            minWaitTime = Mathf.Max(value, 0.1f);
        }
        get
        {
            return minWaitTime;
        }
    }

    public float MaxWaitTime
    {
        set
        {
            maxWaitTime = Mathf.Max(value, 0.1f);
        }
        get
        {
            return maxWaitTime;
        }
    }

    void Start()
    {
        InitSpawner();
    }

    void Update()
    {
        if (!isActive)
        {
            //生成中なら中断する
            if(timer != null)
            {
                StopCoroutine(timer);
                isSpawning = false;
            }

            return;
        }

        //生成中じゃないなら生成開始
        if (!isSpawning)
        {
            timer = StartCoroutine("SpawnTimer");
        }
    }

    //初期化用メソッド
    public void InitSpawner()
    {
        minWaitTime = defaultMinWaitTime;
        maxWaitTime = defaultMaxWaitTime;
        minSize = defaultMinSize;
        maxSize = defaultMaxSize;
    }

    //生成処理を行うコルーチン
    IEnumerator SpawnTimer()
    {
        isSpawning = true;

        GameObject wallObj = Instantiate(wallPrefab, transform.position, Quaternion.identity);
        Wall wall = wallObj.GetComponent<Wall>();

        float sizeX = Random.Range(minSize.x, maxSize.x);
        float sizeY = Random.Range(minSize.y, maxSize.y);
        wall.SetWall(new Vector2(sizeX, sizeY));

        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitTime);

        isSpawning = false;
    }

}
