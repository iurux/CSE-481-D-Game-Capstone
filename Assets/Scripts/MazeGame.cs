using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MazeGame : MonoBehaviour
{
    [Header("UI")]
    public RawImage mazeImage;
    public GameObject mazeCanvas;

    [Header("Result")]
    public GameObject studentCard;   // 解锁物品（可选）

    // ===== Maze settings =====
    const int TILE = 16;
    const int COLS = 20;
    const int ROWS = 20;

    Texture2D tex;

    // ===== Player state =====
    Vector2Int player = new Vector2Int(1, 1);
    int rule = 1; // 1 / 2 / 3
    bool solved = false;

    // ===== Level 1 map =====
    int[,] map = new int[20, 20]
    {
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
        {1,0,0,0,1,2,2,2,1,0,0,0,2,2,2,0,0,0,3,1},
        {1,0,1,0,1,2,1,2,1,0,1,0,1,1,2,1,1,0,1,1},
        {1,0,1,0,0,2,1,2,0,0,1,0,0,0,2,0,1,0,0,1},
        {1,0,1,1,1,1,1,2,1,1,1,1,1,0,2,1,1,1,0,1},
        {1,0,0,0,0,0,0,2,0,0,0,0,1,0,2,2,2,0,0,1},
        {1,1,1,1,1,1,0,1,1,1,1,0,1,1,1,1,2,1,0,1},
        {1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,1,2,0,0,1},
        {1,0,1,1,1,1,1,0,1,0,1,1,1,1,0,1,2,1,1,1},
        {1,0,0,0,0,0,1,0,0,0,2,2,2,1,0,0,2,2,0,1},
        {1,1,1,1,1,0,1,1,1,1,1,1,2,1,1,1,1,2,0,1},
        {1,0,0,0,1,0,2,2,2,0,0,1,2,0,0,0,1,2,0,1},
        {1,0,1,0,1,1,1,1,2,1,0,1,1,1,1,0,1,2,0,1},
        {1,0,1,0,0,0,0,0,2,1,0,0,0,0,1,0,2,2,0,1},
        {1,0,0,0,1,1,1,1,2,1,1,1,1,0,1,1,1,2,0,1},
        {1,1,1,0,1,2,2,2,2,0,0,0,1,0,0,0,0,2,0,1},
        {1,0,0,0,1,2,1,1,1,1,1,0,1,1,1,1,0,2,0,1},
        {1,0,1,1,1,2,1,0,0,0,1,0,0,0,0,1,0,2,0,1},
        {1,0,0,0,0,2,2,2,1,0,1,1,1,1,0,0,0,0,0,1},
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    };

    void Start()
    {
        tex = new Texture2D(COLS * TILE, ROWS * TILE);
        tex.filterMode = FilterMode.Point;
        mazeImage.texture = tex;

        Draw();
    }

    void Update()
    {
        if (solved) return;

        bool ruleChanged = false;

        // ===== Switch rule =====
        if (Keyboard.current.digit1Key.wasPressedThisFrame) { rule = 1; ruleChanged = true; }
        if (Keyboard.current.digit2Key.wasPressedThisFrame) { rule = 2; ruleChanged = true; }
        if (Keyboard.current.digit3Key.wasPressedThisFrame) { rule = 3; ruleChanged = true; }

        int dx = 0, dy = 0;
        if (Keyboard.current.wKey.wasPressedThisFrame) dy = -1;
        if (Keyboard.current.sKey.wasPressedThisFrame) dy = 1;
        if (Keyboard.current.aKey.wasPressedThisFrame) dx = -1;
        if (Keyboard.current.dKey.wasPressedThisFrame) dx = 1;

        if (rule == 3) { dx *= -1; dy *= -1; }

        if (dx != 0 || dy != 0)
        {
            TryMove(dx, dy);
            Draw();
        }
        else if (ruleChanged)
        {
            Draw();
        }
    }

    void TryMove(int dx, int dy)
    {
        int nx = player.x + dx;
        int ny = player.y + dy;

        // ===== Boundary check =====
        if (nx < 0 || ny < 0 || nx >= COLS || ny >= ROWS)
            return;

        int tile = map[ny, nx];
        if (tile == 1) return;
        if (tile == 2 && rule != 2) return;
        if (tile == 0 && rule == 2) return;

        player = new Vector2Int(nx, ny);

        if (tile == 3 && rule == 3)
            PuzzleSolved();
    }

    void PuzzleSolved()
    {
        if (solved) return;
        solved = true;

        Debug.Log("Maze solved!");

        MazeProgress.mazeSolved = true;

        mazeCanvas.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (studentCard != null)
            studentCard.SetActive(true);

        ScreenFlicker flicker = FindObjectOfType<ScreenFlicker>();
        if (flicker != null)
            flicker.StopFlicker();
    }

    // ===== Drawing =====
    void Draw()
    {
        for (int y = 0; y < ROWS; y++)
        {
            for (int x = 0; x < COLS; x++)
            {
                DrawTile(x, ROWS - 1 - y, GetTileColor(map[y, x]));
            }
        }

        // draw player
        DrawRect(player.x, ROWS - 1 - player.y, ColorForPlayer());

        tex.Apply();
    }

    Color GetTileColor(int tile)
    {
        if (tile == 1) return new Color(0.35f, 0.35f, 0.35f);
        if (tile == 2 && rule == 2) return new Color(0.6f, 0f, 0.8f);
        if (tile == 3 && rule == 3) return Color.green;
        return Color.black;
    }

    Color ColorForPlayer()
    {
        if (rule == 1) return Color.white;
        if (rule == 2) return Color.cyan;
        return Color.yellow;
    }

    void DrawTile(int x, int y, Color c)
    {
        for (int py = 0; py < TILE; py++)
            for (int px = 0; px < TILE; px++)
                tex.SetPixel(x * TILE + px, y * TILE + py, c);
    }

    void DrawRect(int x, int y, Color c)
    {
        for (int py = 3; py < TILE - 3; py++)
            for (int px = 3; px < TILE - 3; px++)
                tex.SetPixel(x * TILE + px, y * TILE + py, c);
    }
}