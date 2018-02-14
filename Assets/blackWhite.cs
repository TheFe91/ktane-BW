using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KMHelper;

public class blackWhite : MonoBehaviour {

    public KMAudio Audio;
    public KMBombModule Module;
    public KMBombInfo Info;
    public KMSelectable[] selButtons;
    public Material white, black;

    private bool _isSolved = false, _lightsOn = false;
    private static int _moduleIdCounter = 1;
    private int _moduleId = 0;

    private List<int> blacks, answers;

    // Use this for initialization
    void Start () {
        _moduleId = _moduleIdCounter++;
        Module.OnActivate += Activate;
	}

    private void Awake()
    {
        for (int i = 0; i < 16; i++)
        {
            int j = i;
            selButtons[i].OnInteractEnded += delegate ()
            {
                handlePress(j);
                Debug.LogFormat("[Black&White #{0}] Pressed {1}", _moduleId, j);
            };
        }
    }

    void Activate ()
    {
        Init();
        _lightsOn = true;
    }

    void Init ()
    {
        blacks = new List<int>();
        answers = new List<int>();
        foreach (KMSelectable button in selButtons)
            button.GetComponent<Renderer>().material = white;
        generateStage(1);
    }

    void handlePress (int pressed)
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, selButtons[pressed].transform);

        if (!_lightsOn || _isSolved) return;

        ansChk(pressed);
    }

    bool isEven (int num)
    {
        if (num % 2 == 0) return true;
        else return false;
    }

    void generateStage(int stage)
    {
        Debug.LogFormat("[Black&White #{0}] <Stage 1> START", _moduleId);

        switch (Info.GetStrikes())
        {
            case 0:
                #region TIMER
                blacks.Add(3);
                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 3", _moduleId);
                blacks.Add(4);
                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 4", _moduleId);
                #endregion
                #region INDICATORS
                if (KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.SND) || KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.IND))
                {
                    if (KMBombInfoExtensions.IsIndicatorOn(Info,KMBombInfoExtensions.KnownIndicatorLabel.SND) || KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.IND))
                    {
                        blacks.Add(0);
                        Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 0", _moduleId);
                    }
                    else
                    {
                        blacks.Add(1);
                        Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 1", _moduleId);
                    }
                }
                if (KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.MSA) || KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.TRN))
                {
                    if (KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.MSA) || KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.TRN))
                    {
                        blacks.Add(5);
                        Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 5", _moduleId);
                    }
                    else
                    {
                        blacks.Add(6);
                        Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 6", _moduleId);
                    }
                }
                if (KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.CLR) || KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.CAR))
                {
                    if (KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.CLR) || KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.CAR))
                    {
                        blacks.Add(10);
                        Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 10", _moduleId);
                    }
                    else
                    {
                        blacks.Add(11);
                        Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 11", _moduleId);
                    }
                }
                if (KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.BOB) || KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.NSA))
                {
                    if (KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.BOB) || KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.NSA))
                    {
                        blacks.Add(12);
                        Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 12", _moduleId);
                    }
                    else
                    {
                        blacks.Add(13);
                        Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 13", _moduleId);
                    }
                }
                #endregion
                #region SNL(SerialNumberLast)
                int last = 0;
                foreach (int number in Info.GetSerialNumberNumbers())
                    last = number;
                if (last % 2 == 0)
                {
                    blacks.Add(7);
                    Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 7", _moduleId);
                    blacks.Add(9);
                    Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 9", _moduleId);
                }
                #endregion
                #region SNS(SerialNumberSum)
                int sum = 0;
                foreach (int num in Info.GetSerialNumberNumbers())
                    sum += num;
                if (!isEven(sum))
                {
                    blacks.Add(8);
                    Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 8", _moduleId);
                }
                #endregion
                #region PM(Ports+Modules)
                if (Info.IsPortPresent(KMBombInfoExtensions.KnownPortType.StereoRCA))
                    if ((Info.GetSolvableModuleNames().Count+Info.GetSolvedModuleNames().Count) % 2 == 0)
                    {
                        blacks.Add(2);
                        blacks.Add(14);
                        blacks.Add(15);
                        Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 2, 14 and 15", _moduleId);
                    }
                #endregion
                break;
            case 1:
                break;
            case 2:
                break;
        }
    }

    void ansChk(int pressedButton)
    {
        Debug.LogFormat("[Black&White #{0}] <Stage 1> Pressed button is {1}", _moduleId, pressedButton);

        if (blacks.Contains(pressedButton))
        {
            if (pressedButton == 3)
                if (!Info.GetFormattedTime().Contains("1"))
                    onError();
            else if (pressedButton == 4)
                if (!Info.GetFormattedTime().Contains("2"))
                    onError();

            Debug.LogFormat("[Black&White #{0}] <Stage 1> Answer {1} is correct", _moduleId, pressedButton);
            selButtons[pressedButton].GetComponent<Renderer>().material.color = Color.black;
            answers.Add(pressedButton);
            if (ScrambledEquals(blacks, answers))
            {
                Debug.LogFormat("[Black&White #{0}] <Stage 1> Cleared!", _moduleId);
                Module.HandlePass();
                _isSolved = true;
            }
        }
        else
        {
            onError();
        }
    }

    void onError ()
    {
        Debug.LogFormat("[Black&White #{0}] Answer incorrect! Strike and reset!", _moduleId);
        answers.Clear();
        Module.HandleStrike();
        Init();
    }

    private static bool ScrambledEquals<T>(IEnumerable<T> list1, IEnumerable<T> list2)
    {
        var cnt = new Dictionary<T, int>();
        foreach (T s in list1)
        {
            if (cnt.ContainsKey(s))
            {
                cnt[s]++;
            }
            else
            {
                cnt.Add(s, 1);
            }
        }
        foreach (T s in list2)
        {
            if (cnt.ContainsKey(s))
            {
                cnt[s]--;
            }
            else
            {
                return false;
            }
        }
        return cnt.Values.All(c => c == 0);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
