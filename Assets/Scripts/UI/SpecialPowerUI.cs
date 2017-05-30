using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialPowerUI : MonoBehaviour {

    private GameObject _activeObject;
    private bool _active = false;
    private Lean.Touch.LeanFinger _finger;

    private Vector2 _velocity;
    private Vector2 _curPos;
    private Vector2 _lastPos;

    private float _speed = 10f;

    private TileTypes _type;

    private void Awake()
    {
        _type = new TileTypes();
    }

    void Update()
    {

    }

    void LateUpdate () {
        if (_active && _finger != null)
        {
            _activeObject.transform.position = _finger.GetWorldPosition(1f, Camera.current);
            _velocity = new Vector2 (_curPos.x - _lastPos.x, _curPos.y - _lastPos.y);
            _lastPos = _curPos;
            _curPos = _finger.GetWorldPosition(1f, Camera.current);
        }
    }

    public void SetColorType (TileTypes.ESubState state)
    {
        _type.Type = state;
    }

    public void Fly (Player curPlayer)
    {
        _active = false;
        Rigidbody2D rb = _activeObject.GetComponent<Rigidbody2D>();
        rb.velocity = _velocity * _speed;

        curPlayer.EmptyPower(_type.Type);
    }

    public void SetActive (Lean.Touch.LeanFinger finger, Player curPlayer)
    {
        if (_activeObject != null)
            Destroy(_activeObject);

        _active = true;
        _finger = finger;
        _activeObject = Instantiate(Resources.Load<GameObject>("SpecialSelect"));
        _activeObject.name = "SpecialSelect";
        _activeObject.transform.SetParent(this.transform.parent.parent, false);
        _activeObject.GetComponent<MissileUI>().target = RootController.Instance.NextPlayer(curPlayer.playerNumber);


        _curPos = _finger.GetWorldPosition(1f, Camera.current);
        _lastPos = _finger.GetWorldPosition(1f, Camera.current);
    }
}
