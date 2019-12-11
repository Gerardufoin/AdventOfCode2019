using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Controls : MonoBehaviour
{
    [SerializeField]
    private Button _faster = null;
    [SerializeField]
    private Button _slower = null;
    [SerializeField]
    private Text _speedDisplay = null;

    [SerializeField]
    private List<float> _availableSpeed = new List<float>() { 1f, 2f, 4f, 10f };

    // Start is called before the first frame update
    void Start()
    {
        if (GameDatas.SpeedIndex == 0)
        {
            _slower.interactable = false;
        }
        if (GameDatas.SpeedIndex == _availableSpeed.Count - 1)
        {
            _faster.interactable = false;
        }
        _speedDisplay.text = "x" + _availableSpeed[GameDatas.SpeedIndex];
    }

    public float GetSpeed()
    {
        return _availableSpeed[GameDatas.SpeedIndex];
    }

    public void SpeedUp()
    {
        if (GameDatas.SpeedIndex < _availableSpeed.Count - 1)
        {
            ++GameDatas.SpeedIndex;
        }
        if (GameDatas.SpeedIndex == _availableSpeed.Count - 1)
        {
            _faster.interactable = false;
        }
        _slower.interactable = true;
        _speedDisplay.text = "x" + _availableSpeed[GameDatas.SpeedIndex];
    }

    public void SpeedDown()
    {
        if (GameDatas.SpeedIndex > 0)
        {
            --GameDatas.SpeedIndex;
        }
        if (GameDatas.SpeedIndex == 0)
        {
            _slower.interactable = false;
        }
        _faster.interactable = true;
        _speedDisplay.text = "x" + _availableSpeed[GameDatas.SpeedIndex];
    }

    public void LoadWhite()
    {
        GameDatas.StartColor = Color.white;
        SceneManager.LoadScene(0);
    }
    public void LoadBlack()
    {
        GameDatas.StartColor = Color.black;
        SceneManager.LoadScene(0);
    }
}
