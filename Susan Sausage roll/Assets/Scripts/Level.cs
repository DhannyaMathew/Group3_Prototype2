using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


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
    public GameObject[] flowers;
    public float flowersSpawnChance = 0.2f;
    private static Level _instance;
    public Vector2Int playerSpawn;

    private uint _currentLevel = 0;
    private int _width, _height;
    private Color[] _pixels;
    private Color[] _levelMaskPixels;
    private static HashSet<uint> _walkables;
    private static List<Sausage> _sausages;
    private static List<LevelStart> _levelStarters;
    private static List<Grill> _grills;
    private static List<uint> _levelsCompleted;
    private static Floor[] _floors;
    public static Stack<GameAction> Actions;

    // Start is called before the first frame update
    public void Start()
    {
        if (_instance == null) _instance = this;
        else Destroy(gameObject);
        _sausages = new List<Sausage>();
        _levelStarters = new List<LevelStart>();
        _levelsCompleted = new List<uint>();
        _grills = new List<Grill>();
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
                var pos = new Vector3(x, 0.5f, y);
                var entity = GetEntitiy(s);
                GameObject floor = null;
                switch (block)
                {
                    case Floor1:
                        floor = Instantiate(floor1Piece, pos, Quaternion.identity);
                        TrySpawnFlowers(floor.transform, pos);
                        break;
                    case Floor2:
                        floor = Instantiate(floor2Piece, new Vector3(x, 0.5f, y), Quaternion.identity);
                        TrySpawnFlowers(floor.transform, pos);
                        break;
                    case Floor3:
                        floor = Instantiate(floor3Piece, new Vector3(x, 0.5f, y), Quaternion.identity);
                        TrySpawnFlowers(floor.transform, pos);
                        break;
                    case Floor4:
                        floor = Instantiate(floor4Piece, new Vector3(x, 0.5f, y), Quaternion.identity);
                        break;
                    case Grill:
                        floor = Instantiate(grillPiece, new Vector3(x, 0.5f, y), Quaternion.identity);
                        _grills.Add(floor.GetComponent<Grill>());
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
                            sausage.Set(s, n, GetMaskCode(s));
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
                        var a = GetEntitiy(n);
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

    private void TrySpawnFlowers(Transform parent, Vector3 pos)
    {
        if (Random.Range(0, 1f) < flowersSpawnChance)
        {
            var obj = Instantiate(flowers[Random.Range(0, flowers.Length)], parent);
            obj.transform.position = pos;
            obj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 4) * 90, 0);
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Undo"))
        {
            Undo();
        }
    }

    public static void CompleteLevel(uint level)
    {
        _levelsCompleted.Add(level);
        foreach (var sausage in _sausages)
        {
            if (sausage.Code == level)
            {
                sausage.Remove();
            }
        }

        ClearActionStack();
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
        var floor = GetFloor(coord);
        if (floor == null)
        {
            return false;
        }

        return _walkables.Contains(GetBlock(coord)) && !floor.drop;
    }

    public static Floor GetFloor(Vector2Int coord)
    {
        foreach (var floor in _floors)
        {
            if (floor.Pos.Equals(coord))
                return floor;
        }

        return null;
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
        foreach (var grill in _grills)
        {
            if (grill.gameObject.GetComponent<Floor>().Pos.Equals(coord))
            {
                return grill.IsOn;
            }
        }

        return false;
    }

    public static Sausage CheckForSausage(Vector2Int coord)
    {
        foreach (var sausage in _sausages)
        {
            if (sausage.Contains(coord) && sausage.IsNotDestroyed)
            {
                return sausage;
            }
        }

        return null;
    }

    public static uint GetEntitiy(Vector2Int coord)
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

    public static bool LevelCompleted(uint code)
    {
        return _levelsCompleted.Contains(code);
    }


    public static LevelStart CheckForLevelStart(Vector2Int pos, Vector2Int dir)
    {
        foreach (var levelStarter in _levelStarters)
        {
            if (levelStarter.Contains(pos, dir))
            {
                return levelStarter;
            }
        }

        return null;
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

        foreach (var sausage in _sausages)
        {
            if (sausage.Code != code)
            {
                sausage.Fall();
            }
        }

        foreach (var levelStarter in _levelStarters)
        {
            levelStarter.Deactivate();
        }

        foreach (var grill in _grills)
        {
            if (grill.gameObject.GetComponent<Floor>().Code == code)
            {
                grill.TurnOn();
            }
        }
    }

    public static void RiseAll()
    {
        foreach (var floor in _floors)
        {
            floor.Rise();
        }

        foreach (var sausage in _sausages)
        {
            sausage.Rise();
        }

        foreach (var levelStarter in _levelStarters)
        {
            levelStarter.Activate();
        }

        foreach (var grill in _grills)
        {
            grill.TurnOff();
        }
    }

    public static bool stackOverride = false;

    public static void ClearActionStack()
    {
        stackOverride = true;
        Actions.Clear();
    }

    public static bool AllSausagesCooked(uint code)
    {
        bool result = true;
        foreach (var sausage in _sausages)
        {
            if (sausage.Code == code)
            {
                result = result && sausage.Cooked();
            }
        }

        return result;
    }

    public static LevelStart GetLevelStarter(uint code)
    {
        foreach (var levelStarter in _levelStarters)
        {
            if (levelStarter.Code == code)
            {
                return levelStarter;
            }
        }

        return null;
    }
}


public abstract class GameAction
{
    private bool didExecute;
    protected List<GameAction> subActions = new List<GameAction>();
    protected abstract bool CanPerform();
    protected abstract void Perform();

    public abstract void Inverse();

    public void Undo()
    {
        if (didExecute)
        {
            Inverse();
            foreach (var subAction in subActions)
            {
                subAction.Undo();
            }
        }
    }

    public void Execute(bool push = true)
    {
        if (CanPerform())
        {
            Perform();
            didExecute = true;
            if (subActions.Count > 0)
            {
                foreach (var subAction in subActions)
                {
                    subAction.Execute(false);
                }
            }

            if (push)
            {
                if (!Level.stackOverride)
                {

                    Level.Actions.Push(this);
                }
                else
                {
                    Level.stackOverride = false;
                }
            }
        }
    }
}