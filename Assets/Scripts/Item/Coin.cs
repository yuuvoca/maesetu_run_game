using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{

    [SerializeField]
    AudioClip getSe = null;

    GameManager gameManager;
    SoundManager soundManager;
    int score = 100;    //とりあえずデフォルト値を100としておく
    float speed = 5;    //とりあえずデフォルト値を5としておく

    public int Score
    {
        set
        {
            score = Mathf.Max(value, 0);
        }
    }

    public float Speed
    {
        set
        {
            speed = value;
        }
    }

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        soundManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<SoundManager>();
    }

    void Update()
    {
        transform.position = new Vector2(transform.position.x - speed * Time.deltaTime, transform.position.y);
    }

    //プレイヤーが接触したらスコア加算
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(getSe != null)
            {
                soundManager.PlaySeByName(getSe.name);
            }

            gameManager.Score += score;
            Destroy(gameObject);
        }
    }

    //画面外に出たら破棄（※テストプレイ時にシーンビューに映っていると破棄されないので注意）
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

}
