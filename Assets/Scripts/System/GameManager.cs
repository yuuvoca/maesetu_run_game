using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;	//追加

[RequireComponent(typeof(MoveSceneManager))]
[RequireComponent(typeof(SaveManager))]
[RequireComponent(typeof(SoundManager))]
[DefaultExecutionOrder(-5)]
public class GameManager : SingletonMonoBehaviour<GameManager>
{

	[Header("シーンロード時に自動生成するプレハブを登録")]
	[SerializeField]
	GameObject[] prefabs = null;

	//--ここから追加--
	[Header("ゲーム設定")]
	[SerializeField]
	GameObject playerPrefab = null;
	[SerializeField]
	int maxLevel = 10;
	[SerializeField, Min(1), Tooltip("LvUp毎に障害物の生成間隔を小さくするための除数")]
	float divisor = 1.1f;
	[SerializeField, Tooltip("スコアの桁数")]
	int scoreDigits = 8;
	[SerializeField]
	Vector2 playerSpawnPosition = Vector2.zero;
	[SerializeField, Tooltip("1メートルを何秒で走るか")]
	float secondsPerMeter = 0.05f;
	[SerializeField, Tooltip("1メートル走った時に加算される基本スコア")]
	int scorePerMeter = 10;
	[SerializeField, Tooltip("レベルアップに必要な走行距離")]
	int meterPerLevel = 100;
	[Header("UIの設定")]
	[SerializeField]
	string mileageTextName = "MileageText";
	[SerializeField]
	string scoreTextName = "ScoreText";
	[SerializeField]
	string highScoreTextName = "HighScoreText";
	[SerializeField]
	string levelTextName = "LvText";
	[SerializeField]
	string gameOverCanvasName = "GameOverCanvas(Clone)";	//追加その2
	[SerializeField]
	string retryButtonName = "RetryButton"; //追加その2
	[SerializeField]
	string countDownTextName = "CountdownText"; //追加その3
	[Header("サウンドの設定")]
	[SerializeField]
	AudioClip bgm = null;	//追加その6
	//--追加ここまで--

	MoveSceneManager moveSceneManager;
	SaveManager saveManager;
	SoundManager soundManager;

	//--ここから追加--
	bool gameStarted = false; //追加その3
	bool timerIsActive = false;
	int level = 1;
	int mileage = 0;    //走行距離
	int maxScore = 0;
	int score = 0;
	int highScore = 0;
	Text mileageText;
	Text scoreText;
	Text highScoreText;
	Text levelText;
	Text countdownText; //追加その3
	WallSpawner wallSpawner;
	Coroutine timer;
	Canvas gameOverCanvas;  //追加その2
	Button retryButton;	 //追加その2
	GameObject playerObj;   //追加その2
	CoinSpawner coinSpawner;	//追加その4

	public int Score
	{
		set
		{
			score = Mathf.Clamp(value, 0, maxScore);

			if(score > highScore)
			{
				highScore = score;
			}

			UpdateScoreUi();
		}
		get
		{
			return score;
		}
	}

	public int HighScore
	{
		get
		{
			return highScore;
		}
	}
	//--追加ここまで--

	protected override void Awake()
	{
		base.Awake();

		if (Debug.isDebugBuild)
		{
			
		}

		moveSceneManager = GetComponent<MoveSceneManager>();
		saveManager = GetComponent<SaveManager>();
		soundManager = GetComponent<SoundManager>();
	}

	void Start()
	{
		if (Debug.isDebugBuild && moveSceneManager.SceneName != "Title")	//条件変更
		{
			InstantiateWhenLoadScene();
			LoadComponents();   //追加
			GameStart();	//追加
		}

		//追加その6
		if(bgm != null)
		{
			soundManager.PlayBgmByName(bgm.name);
		}
	}

	void Update()
	{
		if (!gameStarted || moveSceneManager.SceneName == "Title")	//条件変更
		{
			if(timer != null)
			{
				StopCoroutine(timer);
			}

			return;
		}

		if (!timerIsActive)
		{
			timer = StartCoroutine("MileageTimer");
		}
	}

	public void InstantiateWhenLoadScene()
	{
		if(moveSceneManager.SceneName == "Title")
		{
			return;
		}

		foreach (GameObject prefab in prefabs)
		{
			Instantiate(prefab, transform.position, Quaternion.identity);
		}
	}

	//--ここから追加--
	public void InitGame()
	{
		gameStarted = false;  //追加その3
		level = 1;
		mileage = 0;
		maxScore = (int)Mathf.Pow(10, scoreDigits) - 1; //スコアの最大値を作成。例えば、8桁なら99999999
		score = 0;
		timerIsActive = false;  //追加その3
		wallSpawner.isActive = false;   //追加その3
		wallSpawner.InitSpawner();   //追加その3
		coinSpawner.isActive = false;	//追加その4
		coinSpawner.InitSpawner();  //追加その4
		highScore = saveManager.HighScore;	//追加その5

		//追加その3。UI表示の初期化
		UpdateMileageUi();
		UpdateLevelUi();
		UpdateScoreUi();
	}

	public void GameStart()
	{
		InitGame();

		StartCoroutine("Countdown");
	}

	public void GameOver()
	{
		if(playerObj != null)
		{
			Destroy(playerObj);
		}

		gameStarted = false;  //追加その3
		timerIsActive = false;
		wallSpawner.isActive = false;
		gameOverCanvas.enabled = true;
		retryButton.enabled = true;  //追加その3
		coinSpawner.isActive = false;   //追加その4

		//追加その5
		if (highScore > saveManager.HighScore)
		{
			saveManager.HighScore = highScore;
			saveManager.Save();
		}
	}

	public void Retry()
	{
		//シーンにある障害物を全部消去する
		GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
		foreach(GameObject wall in walls)
		{
			Destroy(wall);
		}

		//追加4．コインを消去
		GameObject[] coins = GameObject.FindGameObjectsWithTag("Item");
		foreach (GameObject coin in coins)
		{
			Destroy(coin);
		}

		gameOverCanvas.enabled = false;
		retryButton.enabled = false;  //追加その3

		GameStart();
	}

	IEnumerator Countdown()
	{
		countdownText.enabled = false;
		
		yield return new WaitForSeconds(1);

		countdownText.enabled = true;

		for(int i = 2; i >= 0; i--)
		{
			countdownText.text = (i + 1).ToString();
			yield return new WaitForSeconds(1);
		}

		countdownText.enabled = false;

		playerObj = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
		gameStarted = true;
		wallSpawner.isActive = true;
		coinSpawner.isActive = true;    //追加その4
	}
	
	//シーン読み込み時に各種コンポーネントを取得するメソッド
	public void LoadComponents()
	{
		if (moveSceneManager.SceneName == "Title")
		{
			return;
		}

		wallSpawner = GameObject.FindGameObjectWithTag("WallSpawner").GetComponent<WallSpawner>();
		mileageText = GameObject.Find(mileageTextName).GetComponent<Text>();
		scoreText = GameObject.Find(scoreTextName).GetComponent<Text>();
		highScoreText = GameObject.Find(highScoreTextName).GetComponent<Text>();
		levelText = GameObject.Find(levelTextName).GetComponent<Text>();

		//追加その2
		gameOverCanvas = GameObject.Find(gameOverCanvasName).GetComponent<Canvas>();
		retryButton = GameObject.Find(retryButtonName).GetComponent<Button>();

		//追加その3
		countdownText = GameObject.Find(countDownTextName).GetComponent<Text>();

		//ボタンにクリック時の処理を登録
		retryButton.onClick.AddListener(() => Retry());

		//追加その4
		coinSpawner = GameObject.FindGameObjectWithTag("CoinSpawner").GetComponent<CoinSpawner>();
	}

	IEnumerator MileageTimer()
	{
		timerIsActive = true;

		mileage++;
		LevelUp();
		UpdateMileageUi();

		Score += scorePerMeter * level;

		yield return new WaitForSeconds(secondsPerMeter);

		timerIsActive = false;
	}

	void LevelUp()
	{
		if(level < maxLevel && mileage % meterPerLevel == 0)
		{
			level++;
			UpdateLevelUi();

			//障害物の生成間隔を小さくする
			wallSpawner.MinWaitTime /= divisor;
			wallSpawner.MaxWaitTime /= divisor;

			coinSpawner.MinWaitTime /= divisor;    //追加その4
			coinSpawner.MaxWaitTime /= divisor;    //追加その4
		}
	}

	void UpdateMileageUi()
	{
		mileageText.text = mileage.ToString() + "m";
	}

	void UpdateScoreUi()
	{
		scoreText.text = "Score: " + score.ToString("D" + scoreDigits.ToString());  //ToStringに特定の文字列を渡すと、桁数などを指定できる
		highScoreText.text = "High: " + highScore.ToString("D" + scoreDigits.ToString());
	}

	void UpdateLevelUi()
	{
		levelText.text = "Lv." + level.ToString();
	}

}
