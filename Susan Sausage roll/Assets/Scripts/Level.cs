using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Level : MonoBehaviour
{
    private const uint Floor1 = 0x00ff00; //Green
    private const uint Floor2 = 0x00ff01; //Green
    private const uint Floor3 = 0x00ff02; //Green
    private const uint Floor4 = 0x00ff03; //Green
    private const uint Sand = 0x00ffaa; //Green
    private const uint BlueBlock = 0x0000ff; //Green
    private const uint Grill = 0xff0000; //Red
    private const uint SausageSpawnB1 = 0xa0000000; //Pink
    private const uint SausageSpawnB2 = 0xb0000000; //Pink
    private const uint LevelStartB1 = 0x70000000; //Blue
    private const uint LevelStartB2 = 0x80000000; //Blue

    public Texture2D levelFile;
    public Texture2D levelMask;
    public GameObject floor1Piece;
    public GameObject floor2Piece;
    public GameObject floor3Piece;
    public GameObject floor4Piece;
    public GameObject sandPiece;
    public GameObject blueBlockPiece;
    public GameObject grillPiece;
    public GameObject playerPrefab;
    public GameObject sausagePrefab;
    public GameObject levelStartPrefab;
    private static Level _instance;
    public Vector2Int playerSpawn;

    private uint _currentLevel = 0;
    private int _width, _height;
    private Color[] _pixels;
    private Color[] _levelMaskPixels;
    private static HashSet<uint> _walkables;
    private static List<Sausage> _sausages;
    private static List<LevelStart> _levelStarters;
    private static Floor[] _floors;
    public static Stack<GameAction> Actions;

    // Start is called before the first frame update
    public void Start()
    {
        if (_instance == null) _instance = this;
        else Destroy(gameObject);
        _sausages = new List<Sausage>();
        _levelStarters = new List<LevelStart>();
        Actions = new Stack<GameAction>();
        _walkables = new HashSet<uint>();
        _walkables.Add(Floor1);
        _walkables.Add(Floor2);
        _walkables.Add(Floor3);
        _walkables.Add(Floor4);
        _walkables.Add(Sand);
        _walkables.Add(BlueBlock);
        _walkables.Add(Grill);
        _width = levelFile.width;
        _height = levelFile.height;
        _pixels = levelFile.GetPixels();
        _levelMaskPixels = levelMask.GetPixels();
        var foundSausages = new HashSet<Vector2Int>();
        var foundLevelStart = new HashSet<Vector2Int>();
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var s = new Vector2Int(x, y);
                var block = GetBlock(s);
                var entity = GetEnitiy(s);
                GameObject floor = null;
                switch (block)
                {
                    case Floor1:
                        floor = Instantiate(floor1Piece, new Vector3(x, 0.5f, y), Quaternion.identity);
                        break;
                    case Floor2:
                        floor = Instantiate(floor2Piece, new Vector3(x, 0.5f, y), Quaternion.identity);
                        break;
                    case Floor3:
                        floor = Instantiate(floor3Piece, new Vector3(x, 0.5f, y), Quaternion.identity);
                        break;
                    case Floor4:
                        floor = Instantiate(floor4Piece, new Vector3(x, 0.5f, y), Quaternion.identity);
                        break;
                    case Grill:
                        floor = Instantiate(grillPiece, new Vector3(x, 0.5f, y), Quaternion.identity);
                        break;
                    case Sand:
                        floor = Instantiate(sandPiece, new Vector3(x, 0.5f, y), Quaternion.identity);
                        break;
                    case BlueBlock:
                        floor = Instantiate(blueBlockPiece, new Vector3(x, 0.5f, y), Quaternion.identity);
                        break;
                }

                if (floor != null)
                {
                    floor.GetComponent<Floor>().Set(s, ToHex(_levelMaskPixels[x + _width * y]));
                }


                if (entity == SausageSpawnB1)
                    for (int i = 0; i < 4; i++)
                    {
                        var dir = Vector2Int.zero;
                        switch (i)
                        {
                            case 0:
                                dir = Vector2Int.up;
                                break;
                            case 1:
                                dir = Vector2Int.right;
                                break;
                            case 2:
                                dir = Vector2Int.down;
                                break;
                            case 3:
                                dir = Vector2Int.left;
                                break;
                        }

                        var n = s + dir;
                        var a = ToHex(_pixels[n.x + n.y * _width]) & 0xff000000;
                        if (a == SausageSpawnB2 && !foundSausages.Contains(n))
                        {
                            var obj = Instantiate(sausagePrefab, new Vector3(), Quaternion.identity);
                            var sausage = obj.GetComponent<Sausage>();
                            _sausages.Add(sausage);
                            sausage.Set(s, n);
                            break;
                        }
                    }
                else if (entity == LevelStartB1)
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2Int dir = Vector2Int.zero;
                        switch (i)
                        {
                            case 0:
                                dir = Vector2Int.up;
                                break;
                            case 1:
                                dir = Vector2Int.right;
                                break;
                            case 2:
                                dir = Vector2Int.down;
                                break;
                            case 3:
                                dir = Vector2Int.left;
                                break;
                        }

                        var n = s + dir;
                        var a = GetEnitiy(n);
                        if (a == LevelStartB2 && !foundLevelStart.Contains(n))
                        {
                            var obj = Instantiate(levelStartPrefab, new Vector3(), Quaternion.identity);
                            var levelStart = obj.GetComponent<LevelStart>();
                            _levelStarters.Add(levelStart);
                            levelStart.Set(s, n, ToHex(_levelMaskPixels[x + _width * y]));
                            break;
                        }
                    }
            }
        }

        _floors = FindObjectsOfType<Floor>();
        Instantiate(playerPrefab, new Vector3(playerSpawn.x, 1f, playerSpawn.y), Quaternion.identity);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Undo"))
        {
            Undo();
        }
    }

    public static void Undo()
    {
        if (Actions.Count > 0)
        {
            Actions.Pop().Undo();
        }
    }

    public static bool IsWalkable(Vector2Int coord)
    {
        return _walkables.Contains(GetBlock(coord));
    }

    public static Vector2Int GetPlayerSpawn()
    {
        return _instance.playerSpawn;
    }

    public static uint GetBlock(Vector2Int coord)
    {
        if (coord.x < 0 || coord.x >= _instance._width || coord.y < 0 || coord.y >= _instance._height)
        {
            return 0;
        }

        return ToHex(_instance._pixels[coord.x + _instance._width * coord.y]) & 0x00ffffff;
    }

    private static uint ToHex(Color c)
    {
        return ((uint) (c.a * 255) << 24) | ((uint) (c.r * 255) << 16) | ((uint) (c.g * 255) << 8) | (uint) (c.b * 255);
    }

    public static bool IsGrill(Vector2Int coord)
    {
        return GetBlock(coord) == Grill;
    }

    public static Sausage CheckForSausage(Vector2Int coord)
    {
        foreach (var sausage in _sausages)
        {
            if (sausage.Contains(coord))
            {
                return sausage;
            }
        }

        return null;
    }

    public static uint GetEnitiy(Vector2Int coord)
    {
        if (coord.x < 0 || coord.x >= _instance._width || coord.y < 0 || coord.y >= _instance._height)
        {
            return 0;
        }

        return ToHex(_instance._pixels[coord.x + _instance._width * coord.y]) & 0xff000000;
    }

    public static uint GetMaskCode(Vector2Int coord)
    {
        if (coord.x < 0 || coord.x >= _instance._width || coord.y < 0 || coord.y >= _instance._height)
        {
            return 0;
        }

        return ToHex(_instance._levelMaskPixels[coord.x + _instance._width * coord.y]);
    }


    public static uint CheckForLevelStart(Vector2Int pos, Vector2Int dir)
    {
        var b1 = GetEnitiy(pos);
        var b2 = GetEnitiy(pos + dir);
        if (b1 == LevelStartB1 && b2 == LevelStartB2)
        {
            return GetMaskCode(pos);
        }

        return 0;
    }

    public static void DropAllExcludingMask(uint code)
    {
        foreach (var floor in _floors)
        {
            if (floor.Code != code)
            {
                floor.Drop();
            }
        }
    }

    public static void RiseAll()
    {
        foreach (var floor in _floors)
        {
            floor.Rise();
        }
    }
}


public abstract class GameAction
{
    protected List<GameAction> subActions = new List<GameAction>();
    protected abstract bool CanPerform();
    protected abstract void Perform();

    public abstract void Inverse();

    public void Undo()
    {
        Inverse();
        foreach (var subAction in subActions)
        {
            subAction.Undo();
        }
    }

    public void Execute(bool push = true)
    {
        if (CanPerform())
        {
            Perform();
            if (subActions.Count > 0)
            {
                foreach (var subAction in subActions)
                {
                    subAction.Execute(false);
                }
            }

            if (push)
                Level.Actions.Push(this);
        }
    }
}