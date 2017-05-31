﻿using UnityEngine;
using System.Collections;

public class TileExplosionUI : MonoBehaviour
{
    private RectTransform _rt;
    private Vector2 _startPosition;
    private Player _target;

    private float _travelTime = .66f;
    private float _travellingFor = 0f;
    private float _random;

    private bool _damageApplied = false;

    public void Init(Player target)
    {
        _target = target;
        _rt = GetComponent<RectTransform>();
        _startPosition = _rt.localPosition;
        _random = Random.Range(-1f, 1f);

        RootController.Instance.DisableControls();

        //_travelTime = .4f + count * .5f;
        Destroy(this.gameObject, _travelTime);
    }

    private void OnDestroy()
    {
        RootController.Instance.EnableControls();
        if (!_damageApplied)
            ApplyDamage();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_target != null)
        {
            Vector2 endPosition = _target.transform.GetComponent<RectTransform>().localPosition; 

            _travellingFor += Time.deltaTime; //time in seconds
            float t = _travellingFor / _travelTime;
            t = Mathf.Min(t, 1f);

            Vector2 p0 = _startPosition;
            Vector2 p1 = new Vector2(_startPosition.x + 400 * _random, _startPosition.y);
            Vector2 p2 = new Vector2(endPosition.x + 400 * _random, endPosition.y);
            //Vector2 p2 = endPosition;
            Vector3 p3 = endPosition;
            _rt.localPosition = CalculateBezierPoint(t, p0, p1, p2, p3);
        }
    }

    //P0 is start position, P1 is start curve, P2 is end-curve, P3 is end position
    Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector2 p = uuu * p0; //first term
        p += 3 * uu * t * p1; //second term
        p += 3 * u * tt * p2; //third term
        p += ttt * p3; //fourth term

        return p;
    }

    private void ApplyDamage()
    {
        _damageApplied = true;
        _target.NormalExplosion();
        _target.ReceiveDamage(1);
    }
}
