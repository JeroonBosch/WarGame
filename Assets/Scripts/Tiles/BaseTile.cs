using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseTile : MonoBehaviour {
    private float _x;
    public float x { get { return _x; } set { _x = value; } }

    private float _y;
    public float y { get { return _y; } set { _y = value; } }

    public Vector2 xy { get { return new Vector2(_x, _y); } set { _x = value.x; _y = value.y; } }

    private Image _image;
    private TileTypes _type;
    public TileTypes type { get { return _type; }}

    // Use this for initialization
    void Awake () {
        _type = new TileTypes();
        _type.Type = TileTypes.ESubState.blue; //Needs randomization;
        _image = gameObject.GetComponent<Image>();
    }

    public void Init()
    {
        _image.color = _type.Color; 
    }

    public void InitRandom()
    {
        _type.Type = TileTypes.ESubState.yellow + Random.Range(0, Constants.AmountOfColors) ;
        Init();
    }

    public bool IsAdjacentTo (GameObject prevTile)
    {
        if (Mathf.Abs(prevTile.transform.position.x - transform.position.x) < 90f && Mathf.Abs(prevTile.transform.position.y - transform.position.y) < 90f)
            return true;

        return false;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
