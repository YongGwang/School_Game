using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaTrail : MonoBehaviour
{
    public class TrailObject
    {
        public GameObject TrailObj { get; set; }
        public TrailRenderer Trail { get; set; }
        public int CurrentTargetNum { get; set; }
        public Vector3 targetPosition { get; set; }
        public Color EmmissionColor { get; set; }
    }

    [HideInInspector]
    public List<TrailObject> _Trail;

    [Header("Trail Properties")]
    public GameObject _TrailPrefab;
    public AnimationCurve _trailWidthCurve;
    [Range(0, 8)]
    public int _trailEndCapVertices;
    public Material _TrailMaterial;
    public Gradient _trailColor;

    // Start is called before the first frame update
    void Start()
    {
        _Trail = new List<TrailObject>();
    }
}
