using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KMHelper;

public class blackWhite : MonoBehaviour {

    public KMAudio Audio;
    public KMBombModule Module;
    public KMBombInfo Info;
    public KMSelectable[] buttons;
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
            buttons[i].OnInteract += delegate ()
            {
                handlePress(j);
                return false;
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
        generateStage(1);
    }

    void handlePress (int pressed)
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, buttons[pressed].transform);

        if (!_lightsOn || _isSolved) return;

        Debug.LogFormat("[Black&White #{0}] <Stage 1> Pressed button is {1}", _moduleId, pressed);

        buttons[pressed].GetComponent<Renderer>().material = black;

        ansChk(pressed);
    }

    void generateStage(int stage)
    {
        Debug.LogFormat("[Black&White #{0}] <Stage 1> START", _moduleId);

        switch (Info.GetStrikes())
        {
            case 0:
                #region TIMER
                blacks.Add(3);
                blacks.Add(4);
                #endregion
                #region INDICATORS
                if (KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.SND) || KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.IND))
                {
                    if (KMBombInfoExtensions.IsIndicatorOn(Info,KMBombInfoExtensions.KnownIndicatorLabel.SND) || KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.IND))
                    {
                        blacks.Add(0);
                    }
                    else
                    {
                        blacks.Add(1);
                    }
                }
                if (KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.MSA) || KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.TRN))
                {
                    if (KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.MSA) || KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.TRN))
                    {
                        blacks.Add(5);
                    }
                    else
                    {
                        blacks.Add(6);
                    }
                }
                if (KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.CLR) || KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.CAR))
                {
                    if (KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.CLR) || KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.CAR))
                    {
                        blacks.Add(10);
                    }
                    else
                    {
                        blacks.Add(11);
                    }
                }
                if (KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.BOB) || KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.NSA))
                {
                    if (KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.BOB) || KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.NSA))
                    {
                        blacks.Add(12);
                    }
                    else
                    {
                        blacks.Add(13);
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
                    blacks.Add(9);
                }
                #endregion
                #region SNS(SerialNumberSum)
                int sum = 0;
                foreach (int num in Info.GetSerialNumberNumbers())
                    sum += num;
                if (sum % 2 == 0)
                    blacks.Add(8);
                #endregion
                #region PM(Ports+Modules)
                if (Info.IsPortPresent(KMBombInfoExtensions.KnownPortType.StereoRCA))
                    if ((Info.GetSolvableModuleNames().Count+Info.GetSolvedModuleNames().Count) % 2 == 0)
                    {
                        blacks.Add(2);
                        blacks.Add(14);
                        blacks.Add(15);
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
            Debug.LogFormat("[Black&White #{0}] <Stage 1> Answer is correct", _moduleId);
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
            Debug.LogFormat("[Black&White #{0}] Answer incorrect! Strike and reset!", _moduleId);
            answers.Clear();
            Module.HandleStrike();
            Init();
        }
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
