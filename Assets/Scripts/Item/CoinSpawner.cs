using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{

    public bool isActive = false;

    [SerializeField]
    GameObject coinPrefab = null;
    [SerializeField, Min(0.1f)]
    float defaultMinWaitTime = 1;
    [SerializeField, Min(0.1f)]
    float defaultMaxWaitTime = 1;
    [SerializeField, Min(0)]
    float minPositionOffsetY = 0;
    [SerializeField, Min(0)]
    float maxPositionOffsetY = 0;

    bool isSpawning = false;
    float minWaitTime;
    float maxWaitTime;
    Coroutine timer;

    //外部から値を代入するためのプロパティ
    public float MinWaitTime
    {
        set
        {
            //あまりにも小さい値になるとものすごい数のコインが生成されてしまうので、0.1未満にならないようにする
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

    //LateUpdateでコインの生成を実行することで、確実に壁が生成された後でコインを生成することができる
    void LateUpdate()
    {
        if (!isActive)
        {
            //生成中なら中断する
            if (timer != null)
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

    public void InitSpawner()
    {
        minWaitTime = defaultMinWaitTime;
        maxWaitTime = defaultMaxWaitTime;
    }

    IEnumerator SpawnTimer()
    {
        isSpawning = true;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity); //下向きにレイを飛ばす
        float spawnPositionY = hit.point.y; //レイが当たったY座標を取得

        spawnPositionY += Random.Range(minPositionOffsetY, maxPositionOffsetY); //高さをランダムにずらす

        Instantiate(coinPrefab, new Vector2(transform.position.x, spawnPositionY + 0.5f), Quaternion.identity);

        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitTime);

        isSpawning = false;
    }

}