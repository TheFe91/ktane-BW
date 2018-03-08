using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KMHelper;
using System.Collections;
using batteries = KMHelper.KMBombInfoExtensions.KnownBatteryType;
using labels = KMHelper.KMBombInfoExtensions.KnownIndicatorLabel;
using ports = KMHelper.KMBombInfoExtensions.KnownPortType;

public class blackWhite : MonoBehaviour {

    public KMAudio Audio;
    public KMBombModule Module;
    public KMBombInfo Info;
    public GameObject s1GO, s2GO, s3GO, s4GO;
    public KMSelectable[] selButtons;
	public MeshFilter puzzleBackground;
    public Material white;
    public Texture2D[] bgs;
    public Texture2D black;
    public Animator[] stages;

    private bool _isSolved = false, _lightsOn = false;
    private static int _moduleIdCounter = 1;
    private int _moduleId = 0, stage = 1;
    private List<int> addedblacks;
    private string[] stage2Names = { "Column_D", "D1", "D2", "D3", "D4", "C4", "B4", "A4", "Row_4" };
    private string[] stage3Names = { "Column_E", "E1", "E2", "E3", "E4", "E5", "D5", "C5", "B5", "A5", "Row_5" };
    private string[] stage4Names = { "Column_F", "F1", "F2", "F3", "F4", "F5", "F6", "E6", "D6", "C6", "B6", "A6", "Row_6" };

    private List<int> blacks, answers;

    // Use this for initialization
    void Start () {
        _moduleId = _moduleIdCounter++;
        s1GO.SetActive(false);
        s2GO.SetActive(false);
        s3GO.SetActive(false);
        s4GO.SetActive(false);
        Module.OnActivate += Activate;
	}

    private void Awake()
    {
        for (int i = 0; i < 36; i++)
        {
            int j = i;
            selButtons[i].OnInteract += delegate ()
            {
                handlePress(j);
                return false;
            };
        }
    }

    void Activate ()
    {
        s1GO.SetActive(true);
        Init();
        _lightsOn = true;
    }

    void Init ()
    {
        blacks = new List<int>();
        answers = new List<int>();
        addedblacks = new List<int>();
        foreach (KMSelectable button in selButtons)
            button.GetComponent<Renderer>().material = white;
        generateStage(stage);
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
        Debug.LogFormat("[Black&White #{0}] <Stage {1}> START", _moduleId, stage);
        switch (stage)
        {
            case 1:
                switch (Info.GetStrikes())
                {
                    case 0:
                        #region TIMER
                        blacks.Add(8);
                        Debug.LogFormat("[Black&White #{0}] <Stage {1}> Timer: Added 8 (C2)", _moduleId, stage);
                        #endregion
                        #region INDICATORS
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.SND) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.IND))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.SND) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.IND))
                            {
                                blacks.Add(0);
                                addedblacks.Add(0);
                            }
                            else
                            {
                                blacks.Add(1);
                                addedblacks.Add(1);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.CLR) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.CAR))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.CLR) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.CAR))
                            {
                                blacks.Add(13);
                                addedblacks.Add(13);
                            }
                            else
                            {
                                blacks.Add(14);
                                addedblacks.Add(14);
                            }
                        }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> Indicators: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        #region SNL (SerialNumberLast)
                        int last = 0;
                        foreach (int number in Info.GetSerialNumberNumbers())
                            last = number;
                        if (last % 2 == 0)
                        {
                            blacks.Add(2);
                            blacks.Add(7);
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> SNL: Added 2 (C1) and 7 (B2)", _moduleId, stage);
                        }
                        #endregion
                        #region SNS (SerialNumberSum)
                        int sum = 0;
                        foreach (int num in Info.GetSerialNumberNumbers())
                            sum += num;
                        if (!isEven(sum))
                        {
                            blacks.Add(6);
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> SNS: Added 6 (A2)", _moduleId, stage);
                        }
                        #endregion
                        #region PM (Ports+Modules)
                        if (Info.IsPortPresent(ports.DVI))
                            if (!isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(12);
                                Debug.LogFormat("[Black&White #{0}] <Stage {1}> PM: Added 12 (A3)", _moduleId, stage);
                            }
                        #endregion
                        break;
                    case 1:
                        #region TIMER
                        blacks.Add(12);
                        Debug.LogFormat("[Black&White #{0}] <Stage {1}> Timer: Added 12 (A3)", _moduleId, stage);
                        #endregion
                        #region INDICATORS
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.SND) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.IND))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.SND) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.IND))
                            {
                                blacks.Add(0);
                                addedblacks.Add(0);
                            }
                            else
                            {
                                blacks.Add(1);
                                addedblacks.Add(1);
                            }
                        }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> Indicators: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        #region SNL (SerialNumberLast)
                        last = 0;
                        foreach (int number in Info.GetSerialNumberNumbers())
                            last = number;
                        if (last % 2 == 0)
                        {
                            blacks.Add(2);
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> SNL: Added 2 (C1)", _moduleId, stage);
                        }
                        #endregion
                        #region SNS (SerialNumberSum)
                        sum = 0;
                        foreach (int num in Info.GetSerialNumberNumbers())
                            sum += num;
                        if (!isEven(sum))
                        {
                            blacks.Add(6);
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> SNS: Added 6 (A2)", _moduleId, stage);
                        }
                        #endregion
                        #region PM (Ports+Modules)
                        if (Info.IsPortPresent(ports.StereoRCA))
                            if (isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(8);
                                addedblacks.Add(8);
                            }
                        if (Info.IsPortPresent(ports.DVI))
                            if (!isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(14);
                                addedblacks.Add(14);
                            }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> PM: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        #region BATTERIES
                        if (Info.GetBatteryCount(batteries.AA) > 0 && Info.GetBatteryCount(batteries.D) == 0)
                        {
                            blacks.Add(7);
                            addedblacks.Add(7);
                        }
                        else if ((Info.GetBatteryCount(batteries.AA) == 0 && Info.GetBatteryCount(batteries.D) > 0) || (Info.GetBatteryCount(batteries.AA) > 0 && Info.GetBatteryCount(batteries.D) > 0))
                        {
                            blacks.Add(13);
                            addedblacks.Add(13);
                        }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> Batteries: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        break;
                    case 2:
                        #region INDICATORS
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.MSA) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.TRN))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.MSA) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.TRN))
                            {
                                blacks.Add(7);
                                addedblacks.Add(7);
                            }
                            else
                            {
                                blacks.Add(8);
                                addedblacks.Add(8);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.CLR) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.CAR))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.CLR) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.CAR))
                            {
                                blacks.Add(12);
                                addedblacks.Add(12);
                            }
                            else
                            {
                                blacks.Add(13);
                                addedblacks.Add(13);
                            }
                        }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> Indicators: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        #region SNL (SerialNumberLast)
                        last = 0;
                        foreach (int number in Info.GetSerialNumberNumbers())
                            last = number;
                        if (last % 2 == 0)
                        {
                            blacks.Add(2);
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> SNL: Added 2 (C1)", _moduleId, stage);
                        }
                        #endregion
                        #region SNS (SerialNumberSum)
                        sum = 0;
                        foreach (int num in Info.GetSerialNumberNumbers())
                            sum += num;
                        if (!isEven(sum))
                        {
                            blacks.Add(1);
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> SNS: Added 1 (B1)", _moduleId, stage);
                        }
                        #endregion
                        #region PM (Ports+Modules)
                        if (Info.IsPortPresent(ports.DVI))
                            if (!isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(14);
                                Debug.LogFormat("[Black&White #{0}] <Stage {1}> PM: Added 14 (C3)", _moduleId, stage);
                            }
                        #endregion
                        #region BATTERIES
                        if (Info.GetBatteryCount(batteries.AA) > 0 && Info.GetBatteryCount(batteries.D) == 0)
                        {
                            blacks.Add(6);
                            addedblacks.Add(6);
                        }
                        else if (Info.GetBatteryCount(batteries.AA) == 0 && Info.GetBatteryCount(batteries.D) > 0)
                        {
                            blacks.Add(0);
                            addedblacks.Add(0);
                        }
                        else if (Info.GetBatteryCount(batteries.AA) > 0 && Info.GetBatteryCount(batteries.D) > 0)
                        {
                            blacks.Add(6);
                            addedblacks.Add(6);
                        }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> Batteries: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        break;
                }
                break;
            case 2:
                switch (Info.GetStrikes())
                {
                    case 0:
                        #region TIMER
                        blacks.Add(3);
                        blacks.Add(6);
                        Debug.LogFormat("[Black&White #{0}] <Stage {1}> Timer: Added 3 (D1) and 6 (A2)", _moduleId, stage);
                        #endregion
                        #region INDICATORS
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.SND) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.IND))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.SND) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.IND))
                            {
                                blacks.Add(0);
                                addedblacks.Add(0);
                            }
                            else
                            {
                                blacks.Add(1);
                                addedblacks.Add(1);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.MSA) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.TRN))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.MSA) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.TRN))
                            {
                                blacks.Add(7);
                                addedblacks.Add(7);
                            }
                            else
                            {
                                blacks.Add(8);
                                addedblacks.Add(8);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.CLR) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.CAR))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.CLR) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.CAR))
                            {
                                blacks.Add(14);
                                addedblacks.Add(14);
                            }
                            else
                            {
                                blacks.Add(15);
                                addedblacks.Add(15);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.BOB) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.NSA))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.BOB) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.NSA))
                            {
                                blacks.Add(18);
                                addedblacks.Add(18);
                            }
                            else
                            {
                                blacks.Add(19);
                                addedblacks.Add(19);
                            }
                        }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black & White #{0}] <Stage {1}> Indicators: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        #region SNL(SerialNumberLast)
                        int last = 0;
                        foreach (int number in Info.GetSerialNumberNumbers())
                            last = number;
                        if (isEven(last))
                        {
                            blacks.Add(9);
                            blacks.Add(13);
							Debug.LogFormat("[Black&White #{0}] <Stage {1}> SNL: Added 9 (D2) and 13 (B3)", _moduleId, stage);
                        }
                        #endregion
                        #region SNS(SerialNumberSum)
                        int sum = 0;
                        foreach (int num in Info.GetSerialNumberNumbers())
                            sum += num;
                        if (!isEven(sum))
                        {
                            blacks.Add(21);
							Debug.LogFormat("[Black&White #{0}] <Stage {1}> SNS: Added 21 (D4)", _moduleId, stage);
                        }
                        #endregion
                        #region PM(Ports+Modules)
                        if (Info.IsPortPresent(ports.StereoRCA))
                            if (isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(2);
                                addedblacks.Add(2);
                            }
                        if (Info.IsPortPresent(ports.DVI))
                            if (!isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(12);
                                blacks.Add(20);
                                addedblacks.Add(12);
                                addedblacks.Add(20);
                            }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black & White #{0}] <Stage {1}> Indicators: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        break;
                    case 1:
                        #region TIMER
                        blacks.Add(3);
                        blacks.Add(8);
                        blacks.Add(19);
                        Debug.LogFormat("[Black&White #{0}] <Stage {1}> Timer: dded 3 (D1), 8 (C2) and 19 (B4)", _moduleId, stage);
                        #endregion
                        #region INDICATORS
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.CLR) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.CAR))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.CLR) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.CAR))
                            {
                                blacks.Add(14);
                                addedblacks.Add(14);
                            }
                            else
                            {
                                blacks.Add(15);
                                addedblacks.Add(15);
                            }
                        }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black & White #{0}] <Stage {1}> Indicators: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        #region SNL(SerialNumberLast)
                        last = 0;
                        foreach (int number in Info.GetSerialNumberNumbers())
                            last = number;
                        if (isEven(last))
                        {
                            blacks.Add(1);
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> SNL: Added 1 (B1)", _moduleId, stage);
                        }
                        #endregion
                        #region SNS(SerialNumberSum)
                        sum = 0;
                        foreach (int num in Info.GetSerialNumberNumbers())
                            sum += num;
                        if (!isEven(sum))
                        {
                            blacks.Add(20);
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> SNS: Added 20 (C4)", _moduleId, stage);
                        }
                        #endregion
                        #region PM(Ports+Modules)
                        if (Info.IsPortPresent(ports.StereoRCA))
                            if (isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(2);
                                blacks.Add(9);
                                addedblacks.Add(2);
                                addedblacks.Add(9);
                            }
						if (Info.IsPortPresent(ports.DVI))
                            if (!isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(21);
                                addedblacks.Add(21);
                            }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black & White #{0}] <Stage {1}> PM: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        #region BATTERIES
                        if (Info.GetBatteryCount(batteries.AA) > 0 && Info.GetBatteryCount(batteries.D) == 0)
                        {
                            blacks.Add(6);
                            blacks.Add(7);
                            blacks.Add(18);
                            addedblacks.Add(6);
                            addedblacks.Add(7);
                            addedblacks.Add(18);
                        }
                        else if (Info.GetBatteryCount(batteries.AA) == 0 && Info.GetBatteryCount(batteries.D) > 0)
                        {
                            blacks.Add(0);
                            blacks.Add(12);
                            blacks.Add(13);
                            addedblacks.Add(0);
                            addedblacks.Add(12);
                            addedblacks.Add(13);
                        }
                        else if (Info.GetBatteryCount(batteries.AA) > 0 && Info.GetBatteryCount(batteries.D) > 0)
                        {
                            blacks.Add(0);
                            blacks.Add(7);
                            blacks.Add(18);
                            addedblacks.Add(0);
                            addedblacks.Add(7);
                            addedblacks.Add(18);
                        }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black & White #{0}] <Stage {1}> Batteries: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        break;
                    case 2:
                        #region TIMER
                        blacks.Add(3);
                        blacks.Add(8);
                        blacks.Add(12);
                        blacks.Add(18);
                        Debug.LogFormat("[Black&White #{0}] <Stage {1}> Timer: Added 3 (D1), 8 (C2), 12 (A3) and 18 (A4)", _moduleId, stage);
                        #endregion
                        #region INDICATORS
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.MSA) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.TRN))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.MSA) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.TRN))
                            {
                                blacks.Add(6);
                                addedblacks.Add(6);
                            }
                            else
                            {
                                blacks.Add(7);
                                addedblacks.Add(7);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.CLR) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.CAR))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.CLR) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.CAR))
                            {
                                blacks.Add(13);
                                addedblacks.Add(13);
                            }
                            else
                            {
                                blacks.Add(14);
                                addedblacks.Add(14);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.BOB) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.NSA))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.BOB) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.NSA))
                            {
                                blacks.Add(20);
                                addedblacks.Add(20);
                            }
                            else
                            {
                                blacks.Add(21);
                                addedblacks.Add(21);
                            }
                        }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black & White #{0}] <Stage {1}> Indicators: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        #region SNL (SerialNumberLast)
                        last = 0;
                        foreach (int number in Info.GetSerialNumberNumbers())
                            last = number;
                        if (isEven(last))
                        {
                            blacks.Add(2);
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> SNL: Added 2 (C1)", _moduleId, stage);
                        }
                        #endregion
                        #region SNS (SerialNumberSum)
                        sum = 0;
                        foreach (int num in Info.GetSerialNumberNumbers())
                            sum += num;
                        if (!isEven(sum))
                        {
                            blacks.Add(0);
                            blacks.Add(19);
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> SNS: Added 0 (A1) and 19 (B4)", _moduleId, stage);
                        }
                        #endregion
                        #region PM (Ports+Modules)
                        if (Info.IsPortPresent(ports.StereoRCA))
                            if (isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(1);
                                Debug.LogFormat("[Black&White #{0}] <Stage {1}> PM: Added 1 (B1)", _moduleId, stage);
                            }
                        #endregion
                        #region BATTERIES
                        if (Info.GetBatteryCount(batteries.AA) > 0 && Info.GetBatteryCount(batteries.D) == 0)
                        {
                            blacks.Add(9);
                            addedblacks.Add(9);
                        }
                        else if ((Info.GetBatteryCount(batteries.AA) == 0 && Info.GetBatteryCount(batteries.D) > 0) || (Info.GetBatteryCount(batteries.AA) > 0 && Info.GetBatteryCount(batteries.D) > 0))
                        {
                            blacks.Add(15);
                            addedblacks.Add(15);
                        }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black & White #{0}] <Stage {1}> PM: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        break;
                }
                break;
            case 3:
                switch (Info.GetStrikes())
                {
                    case 0:
                        #region TIMER
                        blacks.Add(0);
                        blacks.Add(8);
                        blacks.Add(24);
                        Debug.LogFormat("[Black&White #{0}] <Stage {1}> Timer: Added 0 (A1), 8 (C2) and 24 (A5)", _moduleId, stage);
                        #endregion
                        #region INDICATORS
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.SND) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.IND))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.SND) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.IND))
                            {
                                blacks.Add(2);
                                addedblacks.Add(2);
                            }
                            else
                            {
                                blacks.Add(3);
                                addedblacks.Add(3);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.CLR) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.CAR))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.CLR) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.CAR))
                            {
                                blacks.Add(13);
                                addedblacks.Add(13);
                            }
                            else
                            {
                                blacks.Add(14);
                                addedblacks.Add(14);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.BOB) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.NSA))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.BOB) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.NSA))
                            {
                                blacks.Add(18);
                                addedblacks.Add(18);
                            }
                            else
                            {
                                blacks.Add(19);
                                addedblacks.Add(19);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.FRQ) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.SIG))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.FRQ) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.SIG))
                            {
                                blacks.Add(25);
                                addedblacks.Add(25);
                            }
                            else
                            {
                                blacks.Add(26);
                                addedblacks.Add(26);
                            }
                        }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black & White #{0}] <Stage {1}> Indicators: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        #region SNL (SerialNumberLast)
                        int last = 0;
                        foreach (int number in Info.GetSerialNumberNumbers())
                            last = number;
                        if (last % 2 == 0)
                        {
                            blacks.Add(6);
                            blacks.Add(15);
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> SNL: Added 6 (A2) and 15 (D3)", _moduleId, stage);
                        }
                        #endregion
                        #region SNS (SerialNumberSum)
                        int sum = 0;
                        foreach (int num in Info.GetSerialNumberNumbers())
                            sum += num;
                        if (!isEven(sum))
                        {
                            blacks.Add(9);
                            blacks.Add(28);
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> SNL: Added 9 (D2) and 28 (E5)", _moduleId, stage);
                        }
                        #endregion
                        #region PM (Ports+Modules)
                        if (Info.IsPortPresent(ports.StereoRCA))
                            if (isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(4);
                                blacks.Add(10);
                                addedblacks.Add(4);
                                addedblacks.Add(10);
                            }
                        if (Info.IsPortPresent(ports.DVI))
                            if (!isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(12);
                                blacks.Add(20);
                                addedblacks.Add(12);
                                addedblacks.Add(20);
                            }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black & White #{0}] <Stage {1}> PM: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        #region BATTERIES
                        if (Info.GetBatteryCount(batteries.AA) > 0 && Info.GetBatteryCount(batteries.D) == 0)
                        {
                            blacks.Add(1);
                            blacks.Add(21);
                            blacks.Add(22);
                            addedblacks.Add(1);
                            addedblacks.Add(21);
                            addedblacks.Add(22);
                        }
                        else if (Info.GetBatteryCount(batteries.AA) == 0 && Info.GetBatteryCount(batteries.D) > 0)
                        {
                            blacks.Add(7);
                            blacks.Add(27);
                            blacks.Add(16);
                            addedblacks.Add(7);
                            addedblacks.Add(27);
                            addedblacks.Add(16);
                        }
                        else if (Info.GetBatteryCount(batteries.AA) > 0 && Info.GetBatteryCount(batteries.D) > 0)
                        {
                            blacks.Add(1);
                            blacks.Add(22);
                            blacks.Add(27);
                            addedblacks.Add(1);
                            addedblacks.Add(22);
                            addedblacks.Add(27);
                        }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black & White #{0}] <Stage {1}> Batteries: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        break;
                    case 1:
                        #region TIMER
                        blacks.Add(9);
                        blacks.Add(18);
                        Debug.LogFormat("[Black&White #{0}] <Stage {1}> Timer: Added 9 (D2) and 18 (A4)", _moduleId, stage);
                        #endregion
                        #region INDICATORS
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.SND) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.IND))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.SND) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.IND))
                            {
                                blacks.Add(1);
                                blacks.Add(3);
                                addedblacks.Add(1);
                                addedblacks.Add(3);
                            }
                            else
                            {
                                blacks.Add(2);
                                blacks.Add(4);
                                addedblacks.Add(2);
                                addedblacks.Add(4);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.BOB) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.NSA))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.BOB) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.NSA))
                            {
                                blacks.Add(21);
                                addedblacks.Add(21);
                            }
                            else
                            {
                                blacks.Add(22);
                                addedblacks.Add(22);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.FRQ) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.SIG))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.FRQ) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.SIG))
                            {
                                blacks.Add(25);
                                addedblacks.Add(25);
                            }
                            else
                            {
                                blacks.Add(26);
                                addedblacks.Add(26);
                            }
                        }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black & White #{0}] <Stage {1}> Indicators: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        #region SNL (SerialNumberLast)
                        last = 0;
                        foreach (int number in Info.GetSerialNumberNumbers())
                            last = number;
                        if (last % 2 == 0)
                        {
                            blacks.Add(14);
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> SNL: Added 14 (C3)", _moduleId, stage);
                        }
                        #endregion
                        #region SNS (SerialNumberSum)
                        sum = 0;
                        foreach (int num in Info.GetSerialNumberNumbers())
                            sum += num;
                        if (!isEven(sum))
                        {
                            blacks.Add(15);
                            blacks.Add(20);
                            blacks.Add(24);
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> SNS: Added 15 (D3), 20 (C4) and 28 (A5)", _moduleId, stage);
                        }
                        #endregion
                        #region PM (Ports+Modules)
                        if (Info.IsPortPresent(ports.StereoRCA))
                            if (isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(8);
                                addedblacks.Add(8);
                            }
                        if (Info.IsPortPresent(ports.DVI))
                            if (!isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(12);
                                addedblacks.Add(12);
                            }
                        if(Info.IsPortPresent(ports.Parallel))
                            if(isEven(Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(28);
                                addedblacks.Add(28);
                            }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black & White #{0}] <Stage {1}> PM: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        #region BATTERIES
                        if (Info.GetBatteryCount(batteries.AA) > 0 && Info.GetBatteryCount(batteries.D) == 0)
                        {
                            blacks.Add(6);
                            blacks.Add(7);
                            blacks.Add(16);
                            blacks.Add(19);
                            addedblacks.Add(6);
                            addedblacks.Add(7);
                            addedblacks.Add(16);
                            addedblacks.Add(19);
                        }
                        else if (Info.GetBatteryCount(batteries.AA) == 0 && Info.GetBatteryCount(batteries.D) > 0)
                        {
                            blacks.Add(0);
                            blacks.Add(10);
                            blacks.Add(13);
                            blacks.Add(25);
                            addedblacks.Add(0);
                            addedblacks.Add(10);
                            addedblacks.Add(13);
                            addedblacks.Add(25);
                        }
                        else if (Info.GetBatteryCount(batteries.AA) > 0 && Info.GetBatteryCount(batteries.D) > 0)
                        {
                            blacks.Add(0);
                            blacks.Add(13);
                            blacks.Add(16);
                            blacks.Add(19);
                            addedblacks.Add(0);
                            addedblacks.Add(13);
                            addedblacks.Add(16);
                            addedblacks.Add(19);
                        }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black & White #{0}] <Stage {1}> Indicators: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        break;
                    case 2:
                        #region TIMER
                        blacks.Add(8);
                        blacks.Add(12);
                        blacks.Add(21);
                        blacks.Add(28);
                        Debug.LogFormat("[Black&White #{0}] <Stage {1}> Timer: Added 8 (C2), 12 (A3), 21 (D4) and 28 (E5)", _moduleId, stage);
                        #endregion
                        #region INDICATORS
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.SND) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.IND))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.SND) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.IND))
                            {
                                blacks.Add(2);
                                addedblacks.Add(2);
                            }
                            else
                            {
                                blacks.Add(3);
                                addedblacks.Add(3);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.MSA) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.TRN))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.MSA) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.TRN))
                            {
                                blacks.Add(9);
                                addedblacks.Add(9);
                            }
                            else
                            {
                                blacks.Add(10);
                                addedblacks.Add(10);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.NSA) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.BOB))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.NSA) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.BOB))
                            {
                                blacks.Add(19);
                                addedblacks.Add(19);
                            }
                            else
                            {
                                blacks.Add(20);
                                addedblacks.Add(20);
                            }
                        }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black & White #{0}] <Stage {1}> Indicators: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        #region SNL (SerialNumberLast)
                        last = 0;
                        foreach (int number in Info.GetSerialNumberNumbers())
                            last = number;
                        if (last % 2 == 0)
                        {
                            blacks.Add(1);
                            blacks.Add(14);
                            blacks.Add(27);
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> SNL: Added 1 (B1), 14 (C3) and 27 (D5)", _moduleId, stage);
                        }
                        #endregion
                        #region SNS (SerialNumberSum)
                        sum = 0;
                        foreach (int num in Info.GetSerialNumberNumbers())
                            sum += num;
                        if (!isEven(sum))
                        {
                            blacks.Add(4);
                            blacks.Add(7);
                            blacks.Add(15);
                            blacks.Add(25);
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> SNS: Added 4 (E1), 7 (B2), 15 (D3) and 25 (B5)", _moduleId, stage);
                        }
                        #endregion
                        #region PM (Ports+Modules)
                        if (Info.IsPortPresent(ports.StereoRCA))
                            if (isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(0);
                                blacks.Add(6);
                                addedblacks.Add(0);
                                addedblacks.Add(6);
                            }
                        if (Info.IsPortPresent(ports.DVI))
                            if (!isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(13);
                                addedblacks.Add(13);
                            }
                        if (Info.IsPortPresent(ports.Parallel))
                            if (isEven(Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(26);
                                addedblacks.Add(26);
                            }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black & White #{0}] <Stage {1}> PM: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        #region BATTERIES
                        if (Info.GetBatteryCount(batteries.AA) > 0 && Info.GetBatteryCount(batteries.D) == 0)
                        {
                            blacks.Add(22);
                            blacks.Add(24);
                            addedblacks.Add(22);
                            addedblacks.Add(24);
                        }
                        else if (Info.GetBatteryCount(batteries.AA) == 0 && Info.GetBatteryCount(batteries.D) > 0)
                        {
                            blacks.Add(18);
                            blacks.Add(16);
                            addedblacks.Add(18);
                            addedblacks.Add(16);
                        }
                        else if (Info.GetBatteryCount(batteries.AA) > 0 && Info.GetBatteryCount(batteries.D) > 0)
                        {
                            blacks.Add(18);
                            blacks.Add(22);
                            addedblacks.Add(18);
                            addedblacks.Add(22);
                        }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black & White #{0}] <Stage {1}> Batteries: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        break;
                }
                break;
            case 4:
                switch (Info.GetStrikes())
                {
                    case 0:
                        #region TIMER
                        blacks.Add(4);
                        blacks.Add(17);
                        blacks.Add(18);
                        blacks.Add(26);
                        blacks.Add(35);
                        Debug.LogFormat("[Black&White #{0}] <Stage {1}> Timer: Added 4 (E1), 17 (F3), 18 (A4), 26 (C5) and 35 (F6)", _moduleId, stage);
                        #endregion
                        #region INDICATORS
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.SND) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.IND))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.SND) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.IND))
                            {
                                blacks.Add(0);
                                blacks.Add(2);
                                addedblacks.Add(0);
                                addedblacks.Add(2);
                            }
                            else
                            {
                                blacks.Add(1);
                                blacks.Add(3);
                                addedblacks.Add(1);
                                addedblacks.Add(3);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.MSA) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.TRN))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.MSA) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.TRN))
                            {
                                blacks.Add(7);
                                addedblacks.Add(7);
                            }
                            else
                            {
                                blacks.Add(8);
                                addedblacks.Add(8);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.NSA) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.BOB))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.NSA) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.BOB))
                            {
                                blacks.Add(21);
                                addedblacks.Add(21);
                            }
                            else
                            {
                                blacks.Add(22);
                                addedblacks.Add(22);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.FRQ) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.SIG))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.FRQ) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.SIG))
                            {
                                blacks.Add(27);
                                addedblacks.Add(27);
                            }
                            else
                            {
                                blacks.Add(28);
                                addedblacks.Add(28);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.FRK))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.FRK))
                            {
                                blacks.Add(32);
                                addedblacks.Add(32);
                            }
                            else
                            {
                                blacks.Add(33);
                                addedblacks.Add(33);
                            }
                        }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> Indicators: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        #region SNL (SerialNumberLast)
                        int last = 0;
                        foreach (int number in Info.GetSerialNumberNumbers())
                            last = number;
                        if (last % 2 == 0)
                        {
                            blacks.Add(9);
                            blacks.Add(12);
                            blacks.Add(15);
                            blacks.Add(23);
                            blacks.Add(34);
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> SNL: Added 9 (D2), 12 (A3), 15 (D3), 23 (F4) and 34 (E5)", _moduleId, stage);
                        }
                        #endregion
                        #region SNS (SerialNumberSum)
                        int sum = 0;
                        foreach (int num in Info.GetSerialNumberNumbers())
                            sum += num;
                        if (!isEven(sum))
                        {
                            blacks.Add(5);
                            blacks.Add(6);
                            blacks.Add(14);
                            blacks.Add(29);
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> SNS: Added 5 (F1), 6 (A2), 14 (C3) and 29 (F5)", _moduleId, stage);
                        }
                        #endregion
                        #region PM (Ports+Modules)
                        if (Info.IsPortPresent(ports.StereoRCA))
                            if (isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(11);
                                addedblacks.Add(11);
                            }
                        if (Info.IsPortPresent(ports.DVI))
                            if (!isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(20);
                                addedblacks.Add(20);
                            }
                        if (Info.IsPortPresent(ports.Parallel))
                            if (isEven(Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(24);
                                addedblacks.Add(24);
                            }
                        if (Info.IsPortPresent(ports.RJ45))
                            if (!isEven(Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(30);
                                addedblacks.Add(30);
                            }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> PM: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        #region BATTERIES
                        if (Info.GetBatteryCount(batteries.AA) > 0 && Info.GetBatteryCount(batteries.D) == 0)
                        {
                            blacks.Add(13);
                            blacks.Add(16);
                            blacks.Add(25);
                            addedblacks.Add(13);
                            addedblacks.Add(16);
                            addedblacks.Add(25);
                        }
                        else if (Info.GetBatteryCount(batteries.AA) == 0 && Info.GetBatteryCount(batteries.D) > 0)
                        {
                            blacks.Add(10);
                            blacks.Add(19);
                            blacks.Add(31);
                            addedblacks.Add(10);
                            addedblacks.Add(19);
                            addedblacks.Add(31);
                        }
                        else if (Info.GetBatteryCount(batteries.AA) > 0 && Info.GetBatteryCount(batteries.D) > 0)
                        {
                            blacks.Add(10);
                            blacks.Add(19);
                            blacks.Add(25);
                            addedblacks.Add(10);
                            addedblacks.Add(19);
                            addedblacks.Add(25);
                        }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> Batteries: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        break;
                    case 1:
                        #region TIMER
                        blacks.Add(2);
                        blacks.Add(10);
                        blacks.Add(17);
                        blacks.Add(19);
                        blacks.Add(27);
                        blacks.Add(34);
                        Debug.LogFormat("[Black&White #{0}] <Stage {1}> Timer: Added 2 (C1), 10 (E2), 17 (F3), 19 (B4), 27 (D5) and 35 (E6)", _moduleId, stage);
                        #endregion
                        #region INDICATORS
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.SND) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.IND))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.SND) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.IND))
                            {
                                blacks.Add(4);
                                addedblacks.Add(4);
                            }
                            else
                            {
                                blacks.Add(5);
                                addedblacks.Add(5);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.MSA) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.TRN))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.MSA) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.TRN))
                            {
                                blacks.Add(8);
                                addedblacks.Add(8);
                            }
                            else
                            {
                                blacks.Add(9);
                                addedblacks.Add(9);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.CLR) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.CAR))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.CLR) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.CAR))
                            {
                                blacks.Add(15);
                                addedblacks.Add(15);
                            }
                            else
                            {
                                blacks.Add(16);
                                addedblacks.Add(16);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.FRQ) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.SIG))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.FRQ) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.SIG))
                            {
                                blacks.Add(25);
                                addedblacks.Add(25);
                            }
                            else
                            {
                                blacks.Add(26);
                                addedblacks.Add(26);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.FRK))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.FRK))
                            {
                                blacks.Add(30);
                                addedblacks.Add(30);
                            }
                            else
                            {
                                blacks.Add(31);
                                addedblacks.Add(31);
                            }
                        }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> Indicators: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        #region SNL (SerialNumberLast)
                        last = 0;
                        foreach (int number in Info.GetSerialNumberNumbers())
                            last = number;
                        if (last % 2 == 0)
                        {
                            blacks.Add(3);
                            blacks.Add(7);
                            blacks.Add(24);
                            blacks.Add(35);
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> SNL: Added 3 (D1), 7 (B2), 24 (A5) and 35 (F6)", _moduleId, stage);
                        }
                        #endregion
                        #region SNS (SerialNumberSum)
                        sum = 0;
                        foreach (int num in Info.GetSerialNumberNumbers())
                            sum += num;
                        if (!isEven(sum))
                        {
                            blacks.Add(13);
                            blacks.Add(23);
                            blacks.Add(32);
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> SNS: Added 13 (B3), 23 (F4) and 32 (C6)", _moduleId, stage);
                        }
                        #endregion
                        #region PM (Ports+Modules)
                        if (Info.IsPortPresent(ports.StereoRCA))
                            if (isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(1);
                                blacks.Add(11);
                                addedblacks.Add(1);
                                addedblacks.Add(11);
                            }
                        if (Info.IsPortPresent(ports.DVI))
                            if (!isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(12);
                                blacks.Add(18);
                                addedblacks.Add(12);
                                addedblacks.Add(18);
                            }
                        if (Info.IsPortPresent(ports.Parallel))
                            if (isEven(Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(29);
                                addedblacks.Add(29);
                            }
                        if (Info.IsPortPresent(ports.RJ45))
                            if (!isEven(Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(33);
                                addedblacks.Add(33);
                            }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> PM: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        #region BATTERIES
                        if (Info.GetBatteryCount(batteries.AA) > 0 && Info.GetBatteryCount(batteries.D) == 0)
                        {
                            blacks.Add(6);
                            blacks.Add(20);
                            blacks.Add(28);
                            addedblacks.Add(6);
                            addedblacks.Add(20);
                            addedblacks.Add(28);
                        }
                        else if (Info.GetBatteryCount(batteries.AA) == 0 && Info.GetBatteryCount(batteries.D) > 0)
                        {
                            blacks.Add(0);
                            blacks.Add(14);
                            blacks.Add(22);
                            addedblacks.Add(0);
                            addedblacks.Add(14);
                            addedblacks.Add(22);
                        }
                        else if (Info.GetBatteryCount(batteries.AA) > 0 && Info.GetBatteryCount(batteries.D) > 0)
                        {
                            blacks.Add(0);
                            blacks.Add(14);
                            blacks.Add(28);
                            addedblacks.Add(0);
                            addedblacks.Add(14);
                            addedblacks.Add(28);
                        }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> Batteries: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        break;
                    case 2:
                        #region TIMER
                        blacks.Add(5);
                        blacks.Add(10);
                        blacks.Add(15);
                        blacks.Add(22);
                        blacks.Add(24);
                        blacks.Add(31);
                        Debug.LogFormat("[Black&White #{0}] <Stage {1}> Timer: Added 5 (F1), 10 (E2), 15 (D3), 22 (E4), 24 (A5) and 31 (B6)", _moduleId, stage);
                        #endregion
                        #region INDICATORS
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.SND) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.IND))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.SND) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.IND))
                            {
                                blacks.Add(0);
                                blacks.Add(3);
                                addedblacks.Add(0);
                                addedblacks.Add(3);
                            }
                            else
                            {
                                blacks.Add(1);
                                blacks.Add(4);
                                addedblacks.Add(1);
                                addedblacks.Add(4);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.BOB) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.NSA))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.BOB) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.NSA))
                            {
                                blacks.Add(18);
                                addedblacks.Add(18);
                            }
                            else
                            {
                                blacks.Add(19);
                                addedblacks.Add(19);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.FRQ) || KMBombInfoExtensions.IsIndicatorPresent(Info, labels.SIG))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.FRQ) || KMBombInfoExtensions.IsIndicatorOn(Info, labels.SIG))
                            {
                                blacks.Add(28);
                                addedblacks.Add(28);
                            }
                            else
                            {
                                blacks.Add(29);
                                addedblacks.Add(29);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, labels.FRK))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, labels.FRK))
                            {
                                blacks.Add(32);
                                addedblacks.Add(32);
                            }
                            else
                            {
                                blacks.Add(33);
                                addedblacks.Add(33);
                            }
                        }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> Indicators: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        #region SNL (SerialNumberLast)
                        last = 0;
                        foreach (int number in Info.GetSerialNumberNumbers())
                            last = number;
                        if (last % 2 == 0)
                        {
                            blacks.Add(9);
                            blacks.Add(12);
                            blacks.Add(23);
                            blacks.Add(34);
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> SNL: Added 9 (D2), 12 (A3), 23 (F4) and 34 (E6)", _moduleId, stage);
                        }
                        #endregion
                        #region SNS (SerialNumberSum)
                        sum = 0;
                        foreach (int num in Info.GetSerialNumberNumbers())
                            sum += num;
                        if (!isEven(sum))
                        {
                            blacks.Add(8);
                            blacks.Add(17);
                            blacks.Add(26);
                            blacks.Add(35);
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> SNS: Added 8 (C2), 17 (F3), 26 (C5) and 35 (F6)", _moduleId, stage);
                        }
                        #endregion
                        #region PM (Ports+Modules)
                        if (Info.IsPortPresent(ports.StereoRCA))
                            if (isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(2);
                                blacks.Add(6);
                                blacks.Add(11);
                                addedblacks.Add(2);
                                addedblacks.Add(6);
                                addedblacks.Add(11);
                            }
                        if (Info.IsPortPresent(ports.DVI))
                            if (!isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(16);
                                addedblacks.Add(16);
                            }
                        if (Info.IsPortPresent(ports.Parallel))
                            if (isEven(Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(25);
                                addedblacks.Add(25);
                            }
                        if (Info.IsPortPresent(ports.RJ45))
                            if (!isEven(Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(30);
                                addedblacks.Add(30);
                            }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> PM: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        #region BATTERIES
                        if (Info.GetBatteryCount(batteries.AA) > 0 && Info.GetBatteryCount(batteries.D) == 0)
                        {
                            blacks.Add(7);
                            blacks.Add(20);
                            blacks.Add(21);
                            addedblacks.Add(7);
                            addedblacks.Add(20);
                            addedblacks.Add(21);
                        }
                        else if (Info.GetBatteryCount(batteries.AA) == 0 && Info.GetBatteryCount(batteries.D) > 0)
                        {
                            blacks.Add(13);
                            blacks.Add(14);
                            blacks.Add(27);
                            addedblacks.Add(13);
                            addedblacks.Add(14);
                            addedblacks.Add(27);
                        }
                        else if (Info.GetBatteryCount(batteries.AA) > 0 && Info.GetBatteryCount(batteries.D) > 0)
                        {
                            blacks.Add(13);
                            blacks.Add(20);
                            blacks.Add(27);
                            addedblacks.Add(13);
                            addedblacks.Add(20);
                            addedblacks.Add(27);
                        }
                        if (addedblacks.Count > 0)
                            Debug.LogFormat("[Black&White #{0}] <Stage {1}> Batteries: Added {2}", _moduleId, stage, logAddedBlacks());
                        addedblacks.Clear();
                        #endregion
                        break;
                }
                break;
        }
    }

    string logAddedBlacks ()
    {
        string toReturn = "";
        foreach (int black in addedblacks)
        {
            toReturn += black.ToString() + " (" + buttonToCell(black) + "), ";
        }
        return toReturn.Substring(0, toReturn.Length - 2);
    }

    void ansChk(int pressedButton)
    {
        Debug.LogFormat("[Black&White #{0}] <Stage {1}> Pressed button is {2} ({3})", _moduleId, stage, pressedButton, buttonToCell(pressedButton));

        if (blacks.Contains(pressedButton))
        {   
            //Timer check
            switch (stage)
            {
                case 1:
                    switch(Info.GetStrikes())
                    {
                        case 0:
                            if (pressedButton == 8)
                            {
                                if (!Info.GetFormattedTime().Contains("2"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            break;
                        case 1:
                            if (pressedButton == 12)
                            {
                                if (!Info.GetFormattedTime().Contains("3"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            break;
                    }
                    break;
                case 2:
                    switch (Info.GetStrikes())
                    {
                        case 0:
                            if (pressedButton == 3)
                                if (!Info.GetFormattedTime().Contains("1"))
                                {
                                    onError();
                                    return;
                                }
                            else if (pressedButton == 6)
                                if (!Info.GetFormattedTime().Contains("2"))
                                {
                                    onError();
                                    return;
                                }
                            break;
                        case 1:
                            if (pressedButton == 3)
                                if (!Info.GetFormattedTime().Contains("1"))
                                {
                                    onError();
                                    return;
                                }
                            else if (pressedButton == 8)
                                if (!Info.GetFormattedTime().Contains("2"))
                                {
                                    onError();
                                    return;
                                }
                            else if (pressedButton == 19)
                                if (!Info.GetFormattedTime().Contains("4"))
                                {
                                    onError();
                                    return;
                                }
                            break;
                        case 2:
                            if (pressedButton == 3)
                                if (!Info.GetFormattedTime().Contains("1"))
                                {
                                    onError();
                                    return;
                                }
                            else if (pressedButton == 8)
                                if (!Info.GetFormattedTime().Contains("2"))
                                {
                                    onError();
                                    return;
                                }
                            else if (pressedButton == 12)
                                if (!Info.GetFormattedTime().Contains("3"))
                                {
                                    onError();
                                    return;
                                }
                            else if (pressedButton == 18)
                                if (!Info.GetFormattedTime().Contains("4"))
                                {
                                    onError();
                                    return;
                                }
                            break;
                    }
                    break;
                case 3:
                    switch (Info.GetStrikes())
                    {
                        case 0:
                            if (pressedButton == 0)
                            {
                                if (!Info.GetFormattedTime().Contains("1"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            else if (pressedButton == 8)
                            {
                                if (!Info.GetFormattedTime().Contains("2"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            else if (pressedButton == 24)
                            {
                                if (!Info.GetFormattedTime().Contains("5"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            break;
                        case 1:
                            if (pressedButton == 9)
                            {
                                if (!Info.GetFormattedTime().Contains("2"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            else if (pressedButton == 18)
                            {
                                if (!Info.GetFormattedTime().Contains("4"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            break;
                        case 2:
                            if (pressedButton == 8)
                            {
                                if (!Info.GetFormattedTime().Contains("2"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            else if (pressedButton == 12)
                            {
                                if (!Info.GetFormattedTime().Contains("3"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            else if (pressedButton == 21)
                            {
                                if (!Info.GetFormattedTime().Contains("4"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            else if (pressedButton == 28)
                            {
                                if (!Info.GetFormattedTime().Contains("5"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            break;
                    }
                    break;
                case 4:
                    switch (Info.GetStrikes())
                    {
                        case 0:
                            if (pressedButton == 4)
                            {
                                if (!Info.GetFormattedTime().Contains("1"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            else if (pressedButton == 17)
                            {
                                if (!Info.GetFormattedTime().Contains("3"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            else if (pressedButton == 18)
                            {
                                if (!Info.GetFormattedTime().Contains("4"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            else if (pressedButton == 26)
                            {
                                if (!Info.GetFormattedTime().Contains("5"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            else if (pressedButton == 35)
                            {
                                if (!Info.GetFormattedTime().Contains("6"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            break;
                        case 1:
                            if (pressedButton == 2)
                            {
                                if (!Info.GetFormattedTime().Contains("1"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            else if (pressedButton == 10)
                            {
                                if (!Info.GetFormattedTime().Contains("2"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            else if (pressedButton == 17)
                            {
                                if (!Info.GetFormattedTime().Contains("3"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            else if (pressedButton == 19)
                            {
                                if (!Info.GetFormattedTime().Contains("4"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            else if (pressedButton == 27)
                            {
                                if (!Info.GetFormattedTime().Contains("5"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            else if (pressedButton == 34)
                            {
                                if (!Info.GetFormattedTime().Contains("6"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            break;
                        case 2:
                            if (pressedButton == 5)
                            {
                                if (!Info.GetFormattedTime().Contains("1"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            else if (pressedButton == 10)
                            {
                                if (!Info.GetFormattedTime().Contains("2"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            else if (pressedButton == 15)
                            {
                                if (!Info.GetFormattedTime().Contains("3"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            else if (pressedButton == 22)
                            {
                                if (!Info.GetFormattedTime().Contains("4"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            else if (pressedButton == 24)
                            {
                                if (!Info.GetFormattedTime().Contains("5"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            else if (pressedButton == 31)
                            {
                                if (!Info.GetFormattedTime().Contains("6"))
                                {
                                    onError();
                                    return;
                                }
                            }
                            break;
                    }
                    break;
            }

            Debug.LogFormat("[Black&White #{0}] <Stage {1}> Answer {2} is correct", _moduleId, stage, pressedButton);
			selButtons[pressedButton].GetComponent<Renderer>().material.mainTexture = black;
            answers.Add(pressedButton);
            if (ScrambledEquals(blacks, answers))
            {
                Debug.LogFormat("[Black&White #{0}] <Stage {1}> Cleared!", _moduleId, stage);
                Audio.PlaySoundAtTransform("lvl_done", Module.transform);
                selButtons[pressedButton].AddInteractionPunch();
                switch (stage)
                {
					case 1:
                        puzzleBackground.GetComponent<Renderer>().material.mainTexture = bgs[1];
                        s2GO.SetActive (true);
						StartCoroutine ("anim2");
	                    stage++;
	                    Init();
	                    break;
					case 2:
                        puzzleBackground.GetComponent<Renderer>().material.mainTexture = bgs[2];
                        s3GO.SetActive (true);
						StartCoroutine ("anim3");
	                    stage++;
	                    Init();
	                    break;
					case 3:
                        puzzleBackground.GetComponent<Renderer>().material.mainTexture = bgs[3];
                        s4GO.SetActive (true);
						StartCoroutine ("anim4");
	                    stage++;
	                    Init();
	                    break;
	                case 4:
	                    Debug.LogFormat("[Black&White #{0}] Module Defused!", _moduleId);
	                    Module.HandlePass();
	                    _isSolved = true;
	                    break;
                }
            }
        }
        else
        {
            onError();
        }
    }

    private string buttonToCell(int pressedButton)
    {
        switch (pressedButton)
        {
            case 0: return "A1";
            case 1: return "B1";
            case 2: return "C1";
            case 3: return "D1";
            case 4: return "E1";
            case 5: return "F1";
            case 6: return "A2";
            case 7: return "B2";
            case 8: return "C2";
            case 9: return "D2";
            case 10: return "E2";
            case 11: return "F2";
            case 12: return "A3";
            case 13: return "B3";
            case 14: return "C3";
            case 15: return "D3";
            case 16: return "E3";
            case 17: return "F3";
            case 18: return "A4";
            case 19: return "B4";
            case 20: return "C4";
            case 21: return "D4";
            case 22: return "E4";
            case 23: return "F4";
            case 24: return "A5";
            case 25: return "B5";
            case 26: return "C5";
            case 27: return "D5";
            case 28: return "E5";
            case 29: return "F5";
            case 30: return "A6";
            case 31: return "B6";
            case 32: return "C6";
            case 33: return "D6";
            case 34: return "E6";
            case 35: return "F6";
        }
        return "";
    }

    void onError ()
    {
        Debug.LogFormat("[Black&White #{0}] Answer incorrect! Strike and reset!", _moduleId);
        puzzleBackground.GetComponent<Renderer>().material.mainTexture = bgs[0];
        switch (stage)
        {
            case 2: StartCoroutine("stage2Error"); break;
            case 3: StartCoroutine("stage3Error"); break;
            case 4: StartCoroutine("stage4Error"); break;
        }
        stage = 1;
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

    #region COROUTINES
    IEnumerator anim2()
    {
        foreach(string state in stage2Names)
        {
            stages[0].Play(state);
            Audio.PlaySoundAtTransform("pop", transform);
            yield return new WaitForSeconds(0.15f);
        }
    }

    IEnumerator anim3()
    {
        foreach (string state in stage3Names)
        {
            stages[1].Play(state);
            Audio.PlaySoundAtTransform("pop", transform);
            yield return new WaitForSeconds(0.15f);
        }
    }

    IEnumerator anim4()
    {
        foreach (string state in stage4Names)
        {
            stages[2].Play(state);
            Audio.PlaySoundAtTransform("pop", transform);
            yield return new WaitForSeconds(0.15f);
        }
    }

    IEnumerator stage2Error()
    {
        for (int i = 0; i < 1; i++)
        {
            stages[0].Play("stage2Err");
            yield return new WaitForSeconds(1f);
        }
        s2GO.SetActive(false);
    }
    IEnumerator stage3Error()
    {
        for (int i = 0; i < 1; i++)
        {
            stages[0].Play("stage2Err");
            stages[1].Play("stage3Err");
            yield return new WaitForSeconds(1f);
        }
        s2GO.SetActive(false);
        s3GO.SetActive(false);
    }

    IEnumerator stage4Error()
    {
        for (int i = 0; i < 1; i++)
        {
            stages[0].Play("stage2Err");
            stages[1].Play("stage3Err");
            stages[2].Play("stage4Err");
            yield return new WaitForSeconds(1f);
        }
        s2GO.SetActive(false);
        s3GO.SetActive(false);
        s4GO.SetActive(false);
    }
    #endregion

    // Update is called once per frame
    void Update () {
		
    }
}
