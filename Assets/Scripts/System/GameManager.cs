//省略

Canvas gameOverCanvas;  //追加その2
Button retryButton;   //追加その2
GameObject playerObj;   //追加その2
CoinSpawner coinSpawner;  //追加その4

//省略

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
  coinSpawner.isActive = false;  //追加その4
  coinSpawner.InitSpawner();  //追加その4

  //追加その3。UI表示の初期化
  UpdateMileageUi();
  UpdateLevelUi();
  UpdateScoreUi();
}

//省略

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

//省略

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

//省略