using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PaintingRobot : MonoBehaviour
{
    public IntcodeProgram Program;
    public float Speed = 2f;
    public float RotationSpeed = 200f;

    private Day11.IntcodeComputer _computer;
    private CaseManager _caseManager;
    private MeshRenderer _currentCase;
    private Controls _controls = null;
    private bool _started = true;
    private bool _moving = false;
    private UnityEngine.Vector3 _dest = UnityEngine.Vector3.zero;
    private UnityEngine.Vector3 _dir = new UnityEngine.Vector3(0, 180f, 0);
    private int _direction = 0;

    private MaterialPropertyBlock _prop;

    // Start is called before the first frame update
    void Start()
    {
        _computer = new Day11.IntcodeComputer(Program.Intcode);
        _caseManager = FindObjectOfType<CaseManager>();
        _controls = FindObjectOfType<Controls>();
        _dest.y = transform.position.y;
        _prop = new MaterialPropertyBlock();
    }

    public void SetCurrentCase(GameObject obj)
    {
        _currentCase = obj.GetComponentInChildren<MeshRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_moving && _started)
        {
            _currentCase.GetPropertyBlock(_prop);
            List<BigInteger> outputs = _computer.Execute((_prop.GetColor("_Color") == Color.white ? 1 : 0));
            _started = _computer.Running && !_computer.Error;
            if (_computer.Error)
            {
                Debug.LogError(_computer.eMessage);
                return;
            }
            Paint((outputs.Count > 0 ? (int)outputs[0] : -1));
            Turn((outputs.Count > 1 ? (int)outputs[1] : -1));
            _moving = true;
        }
        if (_moving)
        {
            Move();
        }
    }

    void Move()
    {
        if (UnityEngine.Quaternion.Angle(transform.rotation, UnityEngine.Quaternion.Euler(_dir)) > 0.1f)
        {
            transform.rotation = UnityEngine.Quaternion.RotateTowards(transform.rotation, UnityEngine.Quaternion.Euler(_dir), RotationSpeed * _controls.GetSpeed() * Time.deltaTime);
            return;
        }
        transform.position = UnityEngine.Vector3.Lerp(transform.position, _dest, Time.deltaTime * _controls.GetSpeed() * Speed);
        if (UnityEngine.Vector3.Distance(transform.position, _dest) < 0.1f)
        {
            _moving = false;
        }
    }

    private void Turn(int order)
    {
        if (order >= 0 && order <= 1)
        {
            _direction = (_direction + (order == 0 ? -1 : 1)) % 4;
            if (_direction < 0)
            {
                _direction = 3;
            }
            _dir.y += (order == 0 ? -90 : 90);
            if (_direction % 2 == 0)
            {
                _dest.z += (_direction == 0 ? 1 : -1);
            }
            else
            {
                _dest.x -= (_direction == 1 ? -1 : 1);
            }
            _caseManager.NewDestination(_dest);
        }
    }

    void Paint(int order)
    {
        if (order < 0) return;
        _currentCase.GetPropertyBlock(_prop);
        _prop.SetColor("_Color", (order == 0 ? Color.black : Color.white));
        _currentCase.SetPropertyBlock(_prop);
    }

    void OnTriggerEnter(Collider other)
    {
        SetCurrentCase(other.gameObject);
    }
}
