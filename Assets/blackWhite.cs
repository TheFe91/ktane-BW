﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KMHelper;

public class blackWhite : MonoBehaviour {

    public KMAudio Audio;
    public KMBombModule Module;
    public KMBombInfo Info;
    public GameObject s1GO, s2GO, s3GO, s4GO;
    public KMSelectable[] selButtons;
    public Material white;
    public Animator s2, s3, s4;

    private bool _isSolved = false, _lightsOn = false;
    private static int _moduleIdCounter = 1;
    private int _moduleId = 0;
    private int stage = 1;

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
        Debug.LogFormat("[Black&White #{0}] <GenerateStage>", _moduleId);

        switch (stage)
        {
            case 1:
                Debug.LogFormat("[Black&White #{0}] <Stage 1> START", _moduleId);
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
                Debug.LogFormat("[Black&White #{0}] <Stage 2> START", _moduleId);
                switch (Info.GetStrikes())
                {
                    case 0:
                        #region TIMER
                        blacks.Add(3);
                        Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 3", _moduleId);
                        blacks.Add(8);
                        Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 8", _moduleId);
                        #endregion
                        #region INDICATORS
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.SND) || KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.IND))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.SND) || KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.IND))
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
                                blacks.Add(7);
                                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 7", _moduleId);
                            }
                            else
                            {
                                blacks.Add(8);
                                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 8", _moduleId);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.CLR) || KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.CAR))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.CLR) || KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.CAR))
                            {
                                blacks.Add(14);
                                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 14", _moduleId);
                            }
                            else
                            {
                                blacks.Add(15);
                                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 15", _moduleId);
                            }
                        }
                        if (KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.BOB) || KMBombInfoExtensions.IsIndicatorPresent(Info, KMBombInfoExtensions.KnownIndicatorLabel.NSA))
                        {
                            if (KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.BOB) || KMBombInfoExtensions.IsIndicatorOn(Info, KMBombInfoExtensions.KnownIndicatorLabel.NSA))
                            {
                                blacks.Add(18);
                                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 18", _moduleId);
                            }
                            else
                            {
                                blacks.Add(19);
                                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 19", _moduleId);
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
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 9", _moduleId);
                            blacks.Add(13);
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 13", _moduleId);
                        }
                        #endregion
                        #region SNS(SerialNumberSum)
                        int sum = 0;
                        foreach (int num in Info.GetSerialNumberNumbers())
                            sum += num;
                        if (!isEven(sum))
                        {
                            blacks.Add(12);
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 12", _moduleId);
                        }
                        #endregion
                        #region PM(Ports+Modules)
                        if (Info.IsPortPresent(KMBombInfoExtensions.KnownPortType.StereoRCA))
                            if ((Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count) % 2 == 0)
                            {
                                blacks.Add(2);
                                blacks.Add(20);
                                blacks.Add(21);
                                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 2, 20 and 21", _moduleId);
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
                                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 14 (C3)", _moduleId);
                            }
                            else
                            {
                                blacks.Add(15);
                                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 15 (D3)", _moduleId);
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
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 1 (B1)", _moduleId);
                        }
                        #endregion
                        #region SNS(SerialNumberSum)
                        sum = 0;
                        foreach (int num in Info.GetSerialNumberNumbers())
                            sum += num;
                        if (!isEven(sum))
                        {
                            blacks.Add(20);
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 20 (C4)", _moduleId);
                        }
                        #endregion
                        #region PM(Ports+Modules)
                        if (Info.IsPortPresent(KMBombInfoExtensions.KnownPortType.StereoRCA))
                            if (isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(2);
                                blacks.Add(9);
                                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 2 (C1) and 9 (D2)", _moduleId);
                            }
						if (Info.IsPortPresent(KMBombInfoExtensions.KnownPortType.DVI))
                            if (!isEven(Info.GetSolvableModuleNames().Count + Info.GetSolvedModuleNames().Count))
                            {
                                blacks.Add(21);
                                Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 21 (D4)", _moduleId);
                            }
                        #endregion
                        #region BATTERIES
                        if (Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.AA) > 0 && Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.D) == 0)
                        {
                            blacks.Add(6);
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 6 (A2)", _moduleId);
                            blacks.Add(7);
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 7 (B2)", _moduleId);
                            blacks.Add(18);
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 18 (A4)", _moduleId);
                        }
                        else if (Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.AA) == 0 && Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.D) > 0)
                        {
                            blacks.Add(0);
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 0 (A1)", _moduleId);
                            blacks.Add(12);
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 12 (A3)", _moduleId);
                            blacks.Add(13);
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 13 (B3)", _moduleId);
                        }
                        else if (Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.AA) > 0 && Info.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.D) > 0)
                        {
                            blacks.Add(0);
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 0 (A1)", _moduleId);
                            blacks.Add(7);
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 7 (B2)", _moduleId);
                            blacks.Add(18);
                            Debug.LogFormat("[Black&White #{0}] <Stage 1> Added 18 (A4)", _moduleId);
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
                        break;
                }
                break;
            case 3:
                Debug.LogFormat("[Black&White #{0}] <Stage 3> START", _moduleId);
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
                Debug.LogFormat("[Black&White #{0}] <Stage 4> START", _moduleId);
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
                            break;
                    }
                    break;
                case 2:
                    switch (Info.GetStrikes())
                    {
                        case 0:
                            if (pressedButton == 3)
                            {
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
                            }
                            break;
                        case 1:
                            break;
                        case 2:
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

            Debug.LogFormat("[Black&White #{0}] <Stage 1> Answer {1} is correct", _moduleId, pressedButton);
            selButtons[pressedButton].GetComponent<Renderer>().material.color = Color.black;
            answers.Add(pressedButton);
            if (ScrambledEquals(blacks, answers))
            {
                Debug.LogFormat("[Black&White #{0}] <Stage {1}> Cleared!", _moduleId, stage);
                Audio.PlaySoundAtTransform("lvl_done", Module.transform);
                selButtons[pressedButton].AddInteractionPunch();
                switch (stage)
                {
                    case 1:
                        s2GO.SetActive(true);
                        s2.Play("stage2");
                        stage++;
                        Init();
                        break;
                    case 2:
                        s3GO.SetActive(true);
                        s3.Play("stage3");
                        stage++;
                        Init();
                        break;
                    case 3:
                        s4GO.SetActive(true);
                        s4.Play("stage4");
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
        switch (stage)
        {
            case 2: s2.Play("stage2Err"); break;
            case 3: s2.Play("stage2Err"); s3.Play("stage3Eerr"); break;
            case 4: s2.Play("stage2Err"); s3.Play("stage3Eerr"); s4.Play("stage4Err");  break;
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

    // Update is called once per frame
    void Update () {
		
	}
}
