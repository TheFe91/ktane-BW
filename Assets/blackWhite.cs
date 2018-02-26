using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KMHelper;
using System.Collections;

public class blackWhite : MonoBehaviour {

    public KMAudio Audio;
    public KMBombModule Module;
    public KMBombInfo Info;
    public GameObject s1GO, s2GO, s3GO, s4GO;
    public KMSelectable[] selButtons;
	public MeshFilter puzzleBackground;
    public Material white, greyMat;
    public Texture2D[] bgs;
    public Texture2D black;
    public Animator[] stages;

    private bool _isSolved = false, _lightsOn = false;
    private static int _moduleIdCounter = 1;
    private int _moduleId = 0;
    private int stage = 1;
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
            selButtons[i].OnInteractEnded += delegate ()
            {
                handlePress(j);
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
                        Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 8 (C2)", _moduleId);
                        #endregion
                        #region INDICATORS
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.SND) || KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.IND))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.SND) || KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.IND))
                            {
                                blacks.Add(0);
                                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 0 (A1)", _moduleId);
                            }
                            else
                            {
                                blacks.Add(1);
                                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 1 (A2)", _moduleId);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.CLR) || KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.CAR))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.CLR) || KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.CAR))
                            {
                                blacks.Add(13);
                                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 13 (B3)", _moduleId);
                            }
                            else
                            {
                                blacks.Add(14);
                                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 14 (C3)", _moduleId);
                            }
                        }
                        #endregion
                        #region SNL (SerialNumberLast)
                        int last = 0;
                        foreach (int number in Info.GetSerialNumberNumbers())
                            last = number;
                        if (last % 2 == 0)
                        {
                            blacks.Add(2);
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 2 (C1)", _moduleId);
                            blacks.Add(7);
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 7 (B2)", _moduleId);
                        }
                        #endregion
                        #region SNS (SerialNumberSum)
                        int sum = 0;
                        foreach (int num in Info.GetSerialNumberNumbers())
                            sum += num;
                        if (!isEven(sum))
                        {
                            blacks.Add(6);
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 6 (A2)", _moduleId);
                        }
                        #endregion
                        #region PM (Ports+Modules)
                        if (Info.IsPortPresent(KMBombInfoExtensions.KnownPortType.DVI))
                            if (!isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(12);
                                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 12 (A3)", _moduleId);
                            }
                        #endregion
                        break;
                    case 1:
                        #region TIMER
                        blacks.Add(12);
                        Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 12 (A3)", _moduleId);
                        #endregion
                        #region INDICATORS
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.SND) || KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.IND))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.SND) || KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.IND))
                            {
                                blacks.Add(0);
                                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 0 (A1)", _moduleId);
                            }
                            else
                            {
                                blacks.Add(1);
                                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 1 (A2)", _moduleId);
                            }
                        }
                        #endregion
                        #region SNL (SerialNumberLast)
                        last = 0;
                        foreach (int number in Info.GetSerialNumberNumbers())
                            last = number;
                        if (last % 2 == 0)
                        {
                            blacks.Add(2);
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 2 (C1)", _moduleId);
                        }
                        #endregion
                        #region SNS (SerialNumberSum)
                        sum = 0;
                        foreach (int num in Info.GetSerialNumberNumbers())
                            sum += num;
                        if (!isEven(sum))
                        {
                            blacks.Add(6);
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 6 (A2)", _moduleId);
                        }
                        #endregion
                        #region PM (Ports+Modules)
                        if (Info.IsPortPresent(KMBombInfoExtensions.KnownPortType.StereoRCA))
                            if (isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(8);
                                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 8 (C2)", _moduleId);
                            }
                        if (Info.IsPortPresent(KMBombInfoExtensions.KnownPortType.DVI))
                            if (!isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(14);
                                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 14 (C3)", _moduleId);
                            }
                        #endregion
                        #region BATTERIES
                        if (Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.AA) > 0 && Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.D) == 0)
                        {
                            blacks.Add(7);
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 7 (B2)", _moduleId);
                        }
                        else if (Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.AA) == 0 && Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.D) > 0)
                        {
                            blacks.Add(13);
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 13 (B3)", _moduleId);
                        }
                        else if (Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.AA) > 0 && Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.D) > 0)
                        {
                            blacks.Add(13);
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 13 (B3)", _moduleId);
                        }
                        #endregion
                        break;
                    case 2:
                        #region INDICATORS
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.SND) || KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.IND))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.SND) || KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.IND))
                            {
                                blacks.Add(7);
                                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 7 (B2)", _moduleId);
                            }
                            else
                            {
                                blacks.Add(1);
                                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 8 (C2)", _moduleId);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.MSA) || KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.MSA))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.TRN) || KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.TRN))
                            {
                                blacks.Add(12);
                                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 12 (A3)", _moduleId);
                            }
                            else
                            {
                                blacks.Add(13);
                                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 13 (B3)", _moduleId);
                            }
                        }
                        #endregion
                        #region SNL (SerialNumberLast)
                        last = 0;
                        foreach (int number in Info.GetSerialNumberNumbers())
                            last = number;
                        if (last % 2 == 0)
                        {
                            blacks.Add(2);
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 2 (C1)", _moduleId);
                        }
                        #endregion
                        #region SNS (SerialNumberSum)
                        sum = 0;
                        foreach (int num in Info.GetSerialNumberNumbers())
                            sum += num;
                        if (!isEven(sum))
                        {
                            blacks.Add(1);
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 1 (B1)", _moduleId);
                        }
                        #endregion
                        #region PM (Ports+Modules)
                        if (Info.IsPortPresent(KMBombInfoExtensions.KnownPortType.DVI))
                            if (!isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(14);
                                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 14 (C3)", _moduleId);
                            }
                        #endregion
                        #region BATTERIES
                        if (Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.AA) > 0 && Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.D) == 0)
                        {
                            blacks.Add(6);
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 6 (A2)", _moduleId);
                        }
                        else if (Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.AA) == 0 && Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.D) > 0)
                        {
                            blacks.Add(0);
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 0 (A1)", _moduleId);
                        }
                        else if (Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.AA) > 0 && Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.D) > 0)
                        {
                            blacks.Add(6);
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 6 (A2)", _moduleId);
                        }
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
						Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 3 (D1)", _moduleId);
                        blacks.Add(6);
						Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 6 (A2)", _moduleId);
                        #endregion
                        #region INDICATORS
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.SND) || KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.IND))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.SND) || KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.IND))
                            {
                                blacks.Add(0);
								Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 0 (A1)", _moduleId);
                            }
                            else
                            {
                                blacks.Add(1);
								Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 1 (B1)", _moduleId);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.MSA) || KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.TRN))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.MSA) || KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.TRN))
                            {
                                blacks.Add(7);
								Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 7 (B2)", _moduleId);
                            }
                            else
                            {
                                blacks.Add(8);
								Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 8 (C2)", _moduleId);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.CLR) || KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.CAR))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.CLR) || KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.CAR))
                            {
                                blacks.Add(14);
								Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 14 (C3)", _moduleId);
                            }
                            else
                            {
                                blacks.Add(15);
								Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 15 (D3)", _moduleId);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.BOB) || KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.NSA))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.BOB) || KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.NSA))
                            {
                                blacks.Add(18);
								Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 18 (A4)", _moduleId);
                            }
                            else
                            {
                                blacks.Add(19);
								Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 19 (B4)", _moduleId);
                            }
                        }
                        #endregion
                        #region SNL(SerialNumberLast)
                        int last = 0;
                        foreach (int number in Info.GetSerialNumberNumbers())
                            last = number;
                        if (isEven(last))
                        {
                            blacks.Add(9);
							Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 9 (D2)", _moduleId);
                            blacks.Add(13);
							Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 13 (B3)", _moduleId);
                        }
                        #endregion
                        #region SNS(SerialNumberSum)
                        int sum = 0;
                        foreach (int num in Info.GetSerialNumberNumbers())
                            sum += num;
                        if (!isEven(sum))
                        {
                            blacks.Add(21);
							Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 21 (D4)", _moduleId);
                        }
                        #endregion
                        #region PM(Ports+Modules)
                        if (Info.IsPortPresent(KMBombInfoExtensions.KnownPortType.StereoRCA))
                            if ((Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count) % 2 == 0)
                            {
                                blacks.Add(2);
                                blacks.Add(12);
                                blacks.Add(20);
								Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 2 (C1), 12 (A3) and 20 (C4)", _moduleId);
                            }
                        #endregion
                        break;
                    case 1:
                        #region TIMER
                        blacks.Add(3);
                        Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 3 (D1)", _moduleId);
                        blacks.Add(8);
                        Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 8 (C2)", _moduleId);
                        blacks.Add(19);
                        Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 19 (B4)", _moduleId);
                        #endregion
                        #region INDICATORS
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.CLR) || KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.CAR))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.CLR) || KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.CAR))
                            {
                                blacks.Add(14);
                                Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 14 (C3)", _moduleId);
                            }
                            else
                            {
                                blacks.Add(15);
                                Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 15 (D3)", _moduleId);
                            }
                        }
                        #endregion
                        #region SNL(SerialNumberLast)
                        last = 0;
                        foreach (int number in Info.GetSerialNumberNumbers())
                            last = number;
                        if (isEven(last))
                        {
                            blacks.Add(1);
                            Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 1 (B1)", _moduleId);
                        }
                        #endregion
                        #region SNS(SerialNumberSum)
                        sum = 0;
                        foreach (int num in Info.GetSerialNumberNumbers())
                            sum += num;
                        if (!isEven(sum))
                        {
                            blacks.Add(20);
                            Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 20 (C4)", _moduleId);
                        }
                        #endregion
                        #region PM(Ports+Modules)
                        if (Info.IsPortPresent(KMBombInfoExtensions.KnownPortType.StereoRCA))
                            if (isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(2);
                                blacks.Add(9);
                                Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 2 (C1) and 9 (D2)", _moduleId);
                            }
						if (Info.IsPortPresent(KMBombInfoExtensions.KnownPortType.DVI))
                            if (!isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(21);
                                Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 21 (D4)", _moduleId);
                            }
                        #endregion
                        #region BATTERIES
                        if (Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.AA) > 0 && Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.D) == 0)
                        {
                            blacks.Add(6);
                            Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 6 (A2)", _moduleId);
                            blacks.Add(7);
                            Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 7 (B2)", _moduleId);
                            blacks.Add(18);
                            Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 18 (A4)", _moduleId);
                        }
                        else if (Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.AA) == 0 && Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.D) > 0)
                        {
                            blacks.Add(0);
                            Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 0 (A1)", _moduleId);
                            blacks.Add(12);
                            Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 12 (A3)", _moduleId);
                            blacks.Add(13);
                            Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 13 (B3)", _moduleId);
                        }
                        else if (Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.AA) > 0 && Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.D) > 0)
                        {
                            blacks.Add(0);
                            Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 0 (A1)", _moduleId);
                            blacks.Add(7);
                            Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 7 (B2)", _moduleId);
                            blacks.Add(18);
                            Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 18 (A4)", _moduleId);
                        }
                        #endregion
                        break;
                    case 2:
                        #region TIMER
                        blacks.Add(3);
                        Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 3", _moduleId);
                        blacks.Add(8);
                        Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 8", _moduleId);
                        blacks.Add(12);
                        Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 12", _moduleId);
                        blacks.Add(18);
                        Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 18", _moduleId);
                        #endregion
                        #region INDICATORS
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.MSA) || KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.TRN))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.MSA) || KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.TRN))
                            {
                                blacks.Add(6);
                                Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 6 (A2)", _moduleId);
                            }
                            else
                            {
                                blacks.Add(7);
                                Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 7 (B2)", _moduleId);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.CLR) || KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.CAR))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.CLR) || KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.CAR))
                            {
                                blacks.Add(13);
                                Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 13 (B3)", _moduleId);
                            }
                            else
                            {
                                blacks.Add(14);
                                Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 14 (C3)", _moduleId);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.BOB) || KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.NSA))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.BOB) || KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.NSA))
                            {
                                blacks.Add(20);
                                Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 20 (C4)", _moduleId);
                            }
                            else
                            {
                                blacks.Add(21);
                                Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 21 (D4)", _moduleId);
                            }
                        }
                        #endregion
                        #region SNL (SerialNumberLast)
                        last = 0;
                        foreach (int number in Info.GetSerialNumberNumbers())
                            last = number;
                        if (isEven(last))
                        {
                            blacks.Add(2);
                            Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 2 (C1)", _moduleId);
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
                            Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 0 (A1) and 19 (B4)", _moduleId);
                        }
                        #endregion
                        #region PM (Ports+Modules)
                        if (Info.IsPortPresent(KMBombInfoExtensions.KnownPortType.StereoRCA))
                            if (isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(1);
                                Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 1 (B1)", _moduleId);
                            }
                        #endregion
                        #region BATTERIES
                        if (Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.AA) > 0 && Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.D) == 0)
                        {
                            blacks.Add(9);
                            Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 9 (D2)", _moduleId);
                        }
                        else if (Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.AA) == 0 && Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.D) > 0)
                        {
                            blacks.Add(15);
                            Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 15 (D3)", _moduleId);
                        }
                        else if (Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.AA) > 0 && Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.D) > 0)
                        {
                            blacks.Add(15);
                            Debug.LogFormat("[Black&White #{0}] <Stage 2> Added 15 (D3)", _moduleId);
                        }
                        #endregion
                        break;
                }
                break;
            case 3:
                switch (Info.GetStrikes())
                {
                    case 0:

                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                }
                break;
            case 4:
                switch (Info.GetStrikes())
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                }
                break;
        }
    }

    void ansChk(int pressedButton)
    {
        Debug.LogFormat("[Black&White #{0}] <Stage 1> Pressed button is {1} ({2})", _moduleId, pressedButton, buttonToCell(pressedButton));

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
            case 2: stages[0].Play("stage2Err"); break;
            case 3: stages[0].Play("stage2Err"); stages[1].Play("stage3Err"); break;
            case 4: stages[0].Play("stage2Err"); stages[1].Play("stage3Err"); stages[2].Play("stage4Err");  break;
        }
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

    // Update is called once per frame
    void Update () {
		if(Input.GetKeyDown("d"))
        {
            s2GO.SetActive(true);
            puzzleBackground.GetComponent<Renderer>().material.mainTexture = bgs[1];
            StartCoroutine("anim2");
        }
        if (Input.GetKeyDown("e"))
        {
            s3GO.SetActive(true);
            puzzleBackground.GetComponent<Renderer>().material.mainTexture = bgs[2];
            StartCoroutine("anim3");
        }
        if (Input.GetKeyDown("f"))
        {
            s4GO.SetActive(true);
            puzzleBackground.GetComponent<Renderer>().material.mainTexture = bgs[3];
            StartCoroutine("anim4");
        }
    }
}
