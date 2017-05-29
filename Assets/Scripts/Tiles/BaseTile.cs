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

    private float _destroyCounter = 0;
    private bool _destroyCounting = false;
    private float _destroyTime = 1.2f;
    private List<BaseTile> removeFromList;

    // Use this for initialization
    void Awake () {
        _type = new TileTypes();
        _type.Type = TileTypes.ESubState.blue; //Needs randomization;
        _image = gameObject.GetComponent<Image>();
    }

    public void Init()
    {
        //_image.color = _type.Color;
        _image.sprite = _type.Sprite;
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
        if (_destroyCounting)
        {
            _destroyCounter += Time.deltaTime;
            if (_destroyCounter > _destroyTime)
            {
                DestroyMe();
            }
        }

    }

    public void PromptDestroy (List<BaseTile> removeFromList)
    {
        if (removeFromList != null)
        {
            this.removeFromList = removeFromList;
            SetDestroyAnimation();
            _destroyCounting = true;
        }
        else
            Debug.Log("ERROR. Could not destroy tile.");
    }

    private void DestroyMe ()
    {
        Destroy(this.gameObject);
        removeFromList.Remove(this);
    }

    public void SetDestroyAnimation ()
    {
        Animator anim = this.gameObject.AddComponent<Animator>();
        anim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/TileSplosionController");
    }
}
