using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public TetrominoData[] tetrominoes;
    public Piece activePiece {  get; private set; }
    public Tilemap tilemap{ get; private set; }
    public Vector3Int spawnPosition;

    public int score;
    public TextMeshProUGUI scoreText;
    public int highScore;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI scoreTextGameOver;
    public GameObject gameOverScreen;
    public Button retryButton;

    public Vector2Int boardSize = new Vector2Int(10, 20);

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-this.boardSize.x/2, -this.boardSize.y/2);
            return new RectInt(position, this.boardSize);
        }

    }

    public void Awake()
    {
        this.activePiece=GetComponentInChildren<Piece>();
        this.tilemap = GetComponentInChildren<Tilemap>();
        for (int i = 0; i < this.tetrominoes.Length; i++)
        {
            this.tetrominoes[i].Initialize();
        }

        NewGame();
    }

    private void Start()
    {
        SpawnPiece();
        
    }

    public void SpawnPiece()
    { 
        int random = Random.Range(0, this.tetrominoes.Length);
        TetrominoData data = this.tetrominoes[random];

        this.activePiece.Initialize(this,this.spawnPosition, data);

        if (IsValidPosition(this.activePiece, this.spawnPosition))
        {
            Set(this.activePiece);
        }
        else
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        this.tilemap.ClearAllTiles();

        gameOverScreen.SetActive(true);

    }
    public void NewGame()
    {
        score = 0;
        UpdateScoreUI();

        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateHighScoreUI();
        gameOverScreen.SetActive(false);
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++) {
            Vector3Int tilePosition = piece.cells[i]+ piece.position;
            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }
    public bool IsValidPosition (Piece piece, Vector3Int position)
    {

        RectInt bounds = this.Bounds;

        for(int i = 0; i< piece.cells.Length;i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;
            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            if (this.tilemap.HasTile(tilePosition))
            {
                return false;
            } 
        }

        return true;
    }
    
    public void ClearLines()
    {
        RectInt bounds = this.Bounds;
        int row = bounds.yMin;
        int linesCleared = 0; // Contador de líneas eliminadas en una sola acción
        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
                linesCleared++; // Incrementar el contador cuando se elimina una línea
            }
            else
            {
                row++;
            }
        }
        // Actualizar la puntuación basada en las líneas eliminadas
        if (linesCleared > 0)
        {
            UpdateScore(linesCleared);
        }
    }

    private bool IsLineFull(int row)
    {
        RectInt bounds = this.Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!this.tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }

    private void LineClear(int row)
    {
        RectInt bounds = this.Bounds;

        for(int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            this.tilemap.SetTile(position, null);

        }
        while(row<bounds.yMax)
        {
            for(int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row +1, 0);
                TileBase above = this.tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                this.tilemap.SetTile(position, above);
;
            }

            row++;
        }
    }
    private void UpdateScore(int linesCleared)
    {
        // Ejemplo simple de puntuación: 100 puntos por línea eliminada
        int points = linesCleared * 100;
        score += points;
        UpdateScoreUI();

        // Actualizar el high score si la puntuación actual es mayor
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            UpdateHighScoreUI();
        }
    }
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString("D5");
            scoreTextGameOver.text= score.ToString("D5");
        }
    }
    private void UpdateHighScoreUI()
    {
        if (highScoreText != null)
        {
            highScoreText.text = highScore.ToString("D5");
        }
    }
 

}

