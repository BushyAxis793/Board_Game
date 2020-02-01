using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    //---------------RED--------------------
    public void SetRedHumanType(bool on)
    {
        if (on) SaveSettings.players[0] = "HUMAN";
    }
    public void SetRedCPUType(bool on)
    {
        if (on) SaveSettings.players[0] = "CPU";
    }
    //---------------GREEN--------------------
    public void SetGreenHumanType(bool on)
    {
        if (on) SaveSettings.players[1] = "HUMAN";
    }
    public void SetGreenCPUType(bool on)
    {
        if (on) SaveSettings.players[1] = "CPU";
    }
    //---------------YELLOW--------------------
    public void SetYellowHumanType(bool on)
    {
        if (on) SaveSettings.players[2] = "HUMAN";
    }
    public void SetYellowCPUType(bool on)
    {
        if (on) SaveSettings.players[2] = "CPU";
    }
    //---------------BLUE--------------------
    public void SetBlueHumanType(bool on)
    {
        if (on) SaveSettings.players[3] = "HUMAN";
    }
    public void SetBlueCPUType(bool on)
    {
        if (on) SaveSettings.players[3] = "CPU";
    }
}

public static class SaveSettings
{   //0     1     2     3
    //RED GREEN YELLOW BLUE
    public static string[] players = new string[4];

    public static string[] winners = new string[3] { string.Empty, string.Empty, string.Empty };
}
