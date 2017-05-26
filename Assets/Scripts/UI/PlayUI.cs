using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayUI : MonoBehaviour
{
    private Transform _player1;
    private Transform _player2;

    private Transform _canvas;
    private Transform _gameboard;
    private GameObject _selectedUI;
    private TileGridController _gridController;

    private Lean.Touch.LeanFinger _dragFinger;
    private List<GameObject> _dragTiles;
    private LineRenderer _line;

    private float _timer;
    private Player _curPlayer;

    private void Start()
    {
        _canvas = transform.Find("Canvas");
        _gameboard = _canvas.Find("GameBoard");
        _gridController = _gameboard.GetComponent<TileGridController>();
        _player1 = _canvas.Find("Player1");
        _player2 = _canvas.Find("Player2");
        _line = _canvas.GetComponent<LineRenderer>();
        _dragTiles = new List<GameObject>();

        SetColor(_player1, 0);
        SetColor(_player2, 1);

        RootController.Instance.GetPlayer(0).SetUI(_player1);
        RootController.Instance.GetPlayer(1).SetUI(_player2);

        _curPlayer = RootController.Instance.GetPlayer(0);
        RootController.Instance.GetPlayer(1).SetTimerActive(false);
    }

    private void Update()
    {
        if (_dragTiles.Count < 1)
            _timer += Time.deltaTime;

        _curPlayer.SetTimer(_timer);

        if (_timer > Constants.MoveTimeInSeconds)
        {
            PassTurn();
        }
    }
    private void FixedUpdate()
    {
        if (_dragFinger != null) {
            GameObject newTile = FindNearestTile();
            if (newTile != null && !_dragTiles.Contains(newTile))
            {
                if (_dragTiles.Count == 0)
                    _dragTiles.Add(newTile);
                else if (newTile.GetComponent<BaseTile>().type.Type == _dragTiles[0].GetComponent<BaseTile>().type.Type)
                {
                    if (newTile.GetComponent<BaseTile>().IsAdjacentTo(_dragTiles[_dragTiles.Count - 1]))
                        _dragTiles.Add(newTile);
                }
            }

            _line.positionCount = _dragTiles.Count;
            for (int i = 0; i < _dragTiles.Count; i++)
            {
                Vector3 pos = _dragTiles[i].transform.position;
                Vector3 linePos = new Vector3(pos.x, pos.y, -0.5f);
                _line.SetPosition(i, linePos);
            }
        }
    }

    private GameObject FindNearestTile()
    {
        GameObject go = null;

        float minDist = Mathf.Infinity;
        Vector3 currentPos = _dragFinger.GetWorldPosition(1f);
        foreach (GameObject tile in _gameboard.GetComponent<TileGridController>().AllTilesAsGameObject())
        {
            float dist = Vector3.Distance(tile.transform.position, currentPos);
            if (dist < minDist)
            {
                go = tile;
                minDist = dist;
            }
        }

        return go;
    }

    private void OnEnable()
    {
        Lean.Touch.LeanTouch.OnFingerDown += OnFingerDown;
        Lean.Touch.LeanTouch.OnFingerSwipe += OnFingerSwipe;
        Lean.Touch.LeanTouch.OnFingerUp += OnFingerUp;
    }

    private void OnDisable()
    {
        Lean.Touch.LeanTouch.OnFingerDown -= OnFingerDown;
        Lean.Touch.LeanTouch.OnFingerSwipe -= OnFingerSwipe;
        Lean.Touch.LeanTouch.OnFingerUp -= OnFingerUp;
    }

    private void SetColor(Transform playerSelect, int playerNumber)
    {
        Player player = RootController.Instance.GetPlayer(playerNumber);
        if (player)
        {
            Image color1 = playerSelect.Find("Color1").GetComponent<Image>();
            color1.color = player.type1.Color;

            Image color2 = playerSelect.Find("Color2").GetComponent<Image>();
            color2.color = player.type2.Color;
        }
    }

    void OnFingerDown(Lean.Touch.LeanFinger finger)
    {
        _selectedUI = null;

        GraphicRaycaster gRaycast = _canvas.GetComponent<GraphicRaycaster>();
        PointerEventData ped = new PointerEventData(null);
        ped.position = finger.GetSnapshotScreenPosition(1f);
        List<RaycastResult> results = new List<RaycastResult>();
        gRaycast.Raycast(ped, results);

        if (results != null)
        {
            _selectedUI = results[0].gameObject;
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.name == "Color1" || result.gameObject.name == "Color2")
                    _selectedUI = result.gameObject;
            }

            if (_selectedUI.tag == "Tile")
            {
                _dragTiles.Clear();
                _dragFinger = finger;
            }
                
        }
    }

    void OnFingerUp(Lean.Touch.LeanFinger finger)
    {
        _dragFinger = null;

        if (_dragTiles != null)
        {
            if (_dragTiles.Count >= 3)
                Combo();

            _dragTiles.Clear();
            _line.positionCount = 0;
        }
        
    }

    void OnFingerSwipe(Lean.Touch.LeanFinger finger)
    {

        if (_selectedUI != null)
        {
            Debug.Log(_selectedUI.name);
            if (_selectedUI.name == "Color1" || _selectedUI.name == "Color2")
            {
                if (_selectedUI.transform.parent == _curPlayer.transform)
                {
                    if (_selectedUI.name == "Color1" && _curPlayer.CheckPowerLevel_1() || _selectedUI.name == "Color2" && _curPlayer.CheckPowerLevel_2())
                    {
                        Rigidbody2D rb = _selectedUI.GetComponent<Rigidbody2D>();
                        if (rb)
                            rb.AddForce(finger.SwipeScaledDelta * 20f);
                        _selectedUI = null;

                        RootController.Instance.NextPlayer(_curPlayer.playerNumber).ReceiveDamage(Mathf.FloorToInt(Constants.SpecialMoveFillRequirement));
                        EndTurn();
                    }
                }

            }
        }
    }

    void Combo ()
    {
        foreach (GameObject go in _dragTiles)
        {
            _gridController.DestroyTile(go);
        }
        RootController.Instance.NextPlayer(_curPlayer.playerNumber).ReceiveDamage(_dragTiles.Count);
        _curPlayer.FillPower(_dragTiles[0].GetComponent<BaseTile>().type.Type, _dragTiles.Count);

        EndTurn();
    }

    void PassTurn ()
    {
        _curPlayer.Heal(5);
        EndTurn();
    }

    void EndTurn ()
    {
        Player nextPlayer = RootController.Instance.NextPlayer(_curPlayer.playerNumber);

        _timer = 0;
        _curPlayer.SetTimerActive(false);

        _curPlayer = nextPlayer;
        nextPlayer.SetTimerActive(true);
    }
}