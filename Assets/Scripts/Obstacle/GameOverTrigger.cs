using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverTrigger : MonoBehaviour
{

    GameManager gameManager;
   // public GameObject resultUI;
    //public GameObject floor;
    GameObject maincanvas;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        maincanvas = GameObject.Find("MainCanvas(Clone)");

        Debug.Log(maincanvas);
    }

    //プレイヤーが触れたらゲームオーバーにする
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameManager.GameOver();

            Debug.Log("ゲームオーバー完治しました");

            //resultUI.gameObject.SetActive(true);
            //floor.gameObject.SetActive(false);
            maincanvas.gameObject.SetActive(false);

            //maincanvas.gameObject.SetActive(true);


            maincanvas.gameObject.SetActive(true);




        }

        else
        {
            //resultUI.gameObject.SetActive(false);
        }
    }

}
