using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaseManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _casePrefab = null;
    [SerializeField]
    private PaintingRobot _robotPrefab = null;
    [SerializeField]
    private float _SpawnTimer = 1.5f;

    private CameraController _camera = null;
    private Controls _controls = null;
    private GameObject _firstCase = null;
    private GameObject _caseHolder = null;

    private (int x, int y) _min = (0, 0);
    private (int x, int y) _max = (0, 0);
    private float _timer = 0f;
    private PaintingRobot _robot = null;

    void Start()
    {
        _caseHolder = new GameObject("Cases");
        _firstCase = GameObject.Instantiate(_casePrefab, Vector3.zero, Quaternion.identity, _caseHolder.transform);
        MeshRenderer renderer = _firstCase.GetComponentInChildren<MeshRenderer>();
        MaterialPropertyBlock mat = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(mat);
        mat.SetColor("_Color", GameDatas.StartColor);
        renderer.SetPropertyBlock(mat);
        _camera = FindObjectOfType<CameraController>();
        _controls = FindObjectOfType<Controls>();
    }

    private void Update()
    {
        if (_robot == null && (_timer += Time.deltaTime) > _SpawnTimer)
        {
            _robot = GameObject.Instantiate(_robotPrefab, _robotPrefab.transform.position, _robotPrefab.transform.rotation);
            _robot.SetCurrentCase(_firstCase);
        }
    }

    public void NewDestination(Vector3 dest)
    {
        if (dest.x < _min.x)
        {
            _min.x = (int)dest.x;
            AddCases((_min.x, _min.y), (_min.x, _max.y));
        }
        if (dest.x > _max.x)
        {
            _max.x = (int)dest.x;
            AddCases((_max.x, _min.y), (_max.x, _max.y));
        }
        if (dest.z < _min.y)
        {
            _min.y = (int)dest.z;
            AddCases((_min.x, _min.y), (_max.x, _min.y));
        }
        if (dest.z > _max.y)
        {
            _max.y = (int)dest.z;
            AddCases((_min.x, _max.y), (_max.x, _max.y));
        }
        _camera.SetCenter(new Vector3((_min.x + _max.x) / 2, 0, (_min.y + _max.y) / 2));
        _camera.SetScale(Mathf.Max(Mathf.Abs(_min.x) + _max.x + 1, Mathf.Abs(_min.y) + _max.y + 1));
    }

    public void AddCases((int x, int y) from, (int x, int y) to)
    {
        for (int y = from.y; y < to.y + 1; ++y)
        {
            for (int x = from.x; x < to.x + 1; ++x)
            {
                Animator anim = GameObject.Instantiate(_casePrefab, new Vector3(x, 0, y), Quaternion.identity, _caseHolder.transform).GetComponent<Animator>();
                anim.SetFloat("Speed", _controls.GetSpeed());
            }
        }
    }
}
