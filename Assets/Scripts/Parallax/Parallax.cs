using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    float length;
    float startPos;

    enum UpdateType { Normal, Late, Fixed }

    //[SerializeField]
    //ParallaxLayer backgroundBack;
    //[SerializeField]
    //ParallaxLayer backgroundMiddle;
    //[SerializeField]
    //ParallaxLayer backgroundFront;

    [SerializeField]
    UpdateType updateType;

    [SerializeField]
    List<ParallaxLayer> layers = new List<ParallaxLayer>();

    [SerializeField]
    float parallaxEffect;

    private void Start()
    {
        layers.Clear();
        layers.Add(new ParallaxLayer(transform.Find("Back").gameObject, 1f, 0.64f));
        //layers.Add(new ParallaxLayer(transform.Find("Middle").gameObject, 0.805f, 0.10f));
        layers.Add(new ParallaxLayer(transform.Find("Front").gameObject, 0.635f, 0.25f));
    }

    private void Update()
    {
        if (updateType == UpdateType.Normal)
        {
            foreach (ParallaxLayer layer in layers)
            {
                layer.CalculateDistanceMoved();
                layer.Move();
            }
        }
    }

    private void LateUpdate()
    {
        if (updateType == UpdateType.Late)
        {
            foreach (ParallaxLayer layer in layers)
            {
                layer.CalculateDistanceMoved();
                layer.Move();
            }
        }
    }

    private void FixedUpdate()
    {

        if (updateType == UpdateType.Fixed)
        {
            foreach (ParallaxLayer layer in layers)
            {
                layer.CalculateDistanceMoved();
                layer.Move();
            }
        }
    }
}

[System.Serializable]
public class ParallaxLayer
{
    [SerializeField]
    float _length;
    [SerializeField]
    Vector2 _startPos;
    [SerializeField]
    GameObject _obj;
    [SerializeField]
    float _horizontalParallax;
    [SerializeField]
    float _verticalParallax;

    [SerializeField]
    Vector2 _distanceMovedFromOrigin;

    public float Length
    {
        get { return _length; }
    }
    public Vector2 StartPos
    {
        get { return _startPos; }
    }
    public GameObject Obj
    {
        get { return _obj; }
    }
    public float HorizontalParallax
    {
        get { return _horizontalParallax; }
    }
    public float VerticalParallax
    {
        get { return _verticalParallax; }
    }

    public Vector2 DistanceMovedFromOrigin
    {
        get { return _distanceMovedFromOrigin; }
    }

    public ParallaxLayer(GameObject obj, float horizontalParallax, float verticalParallax)
    {
        _obj = obj;
        _startPos = obj.transform.position;
        _length = obj.GetComponent<SpriteRenderer>().bounds.size.x;
        _horizontalParallax = horizontalParallax;
        _verticalParallax = verticalParallax;
    }

    public void CalculateDistanceMoved()
    {
        _distanceMovedFromOrigin = new Vector2(Camera.main.transform.position.x * _horizontalParallax, Camera.main.transform.position.y * _verticalParallax);
    }

    public void Move()
    {
        //_obj.transform.position = new Vector3(_startPos + _distanceMovedFromOrigin, _obj.transform.position.y, _obj.transform.position.z);
        _obj.transform.position = new Vector3(_startPos.x + _distanceMovedFromOrigin.x, _startPos.y + _distanceMovedFromOrigin.y, _obj.transform.position.z);
    }
}