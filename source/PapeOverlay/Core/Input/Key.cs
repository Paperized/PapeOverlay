using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeOverlay.Core.Input
{
    /// <summary>
    /// Keys used in InputManager
    /// </summary>
    public enum Key
    {
        //
        // Riepilogo:
        //     No documentation.
        Unknown = 0,
        //
        // Riepilogo:
        //     No documentation.
        Escape = 1,
        //
        // Riepilogo:
        //     No documentation.
        D1 = 2,
        //
        // Riepilogo:
        //     No documentation.
        D2 = 3,
        //
        // Riepilogo:
        //     No documentation.
        D3 = 4,
        //
        // Riepilogo:
        //     No documentation.
        D4 = 5,
        //
        // Riepilogo:
        //     No documentation.
        D5 = 6,
        //
        // Riepilogo:
        //     No documentation.
        D6 = 7,
        //
        // Riepilogo:
        //     No documentation.
        D7 = 8,
        //
        // Riepilogo:
        //     No documentation.
        D8 = 9,
        //
        // Riepilogo:
        //     No documentation.
        D9 = 10,
        //
        // Riepilogo:
        //     No documentation.
        D0 = 11,
        //
        // Riepilogo:
        //     No documentation.
        Minus = 12,
        //
        // Riepilogo:
        //     No documentation.
        Equals = 13,
        //
        // Riepilogo:
        //     No documentation.
        Back = 14,
        //
        // Riepilogo:
        //     No documentation.
        Tab = 15,
        //
        // Riepilogo:
        //     No documentation.
        Q = 16,
        //
        // Riepilogo:
        //     No documentation.
        W = 17,
        //
        // Riepilogo:
        //     No documentation.
        E = 18,
        //
        // Riepilogo:
        //     No documentation.
        R = 19,
        //
        // Riepilogo:
        //     No documentation.
        T = 20,
        //
        // Riepilogo:
        //     No documentation.
        Y = 21,
        //
        // Riepilogo:
        //     No documentation.
        U = 22,
        //
        // Riepilogo:
        //     No documentation.
        I = 23,
        //
        // Riepilogo:
        //     No documentation.
        O = 24,
        //
        // Riepilogo:
        //     No documentation.
        P = 25,
        //
        // Riepilogo:
        //     No documentation.
        LeftBracket = 26,
        //
        // Riepilogo:
        //     No documentation.
        RightBracket = 27,
        //
        // Riepilogo:
        //     No documentation.
        Return = 28,
        //
        // Riepilogo:
        //     No documentation.
        LeftControl = 29,
        //
        // Riepilogo:
        //     No documentation.
        A = 30,
        //
        // Riepilogo:
        //     No documentation.
        S = 31,
        //
        // Riepilogo:
        //     No documentation.
        D = 32,
        //
        // Riepilogo:
        //     No documentation.
        F = 33,
        //
        // Riepilogo:
        //     No documentation.
        G = 34,
        //
        // Riepilogo:
        //     No documentation.
        H = 35,
        //
        // Riepilogo:
        //     No documentation.
        J = 36,
        //
        // Riepilogo:
        //     No documentation.
        K = 37,
        //
        // Riepilogo:
        //     No documentation.
        L = 38,
        //
        // Riepilogo:
        //     No documentation.
        Semicolon = 39,
        //
        // Riepilogo:
        //     No documentation.
        Apostrophe = 40,
        //
        // Riepilogo:
        //     No documentation.
        Grave = 41,
        //
        // Riepilogo:
        //     No documentation.
        LeftShift = 42,
        //
        // Riepilogo:
        //     No documentation.
        Backslash = 43,
        //
        // Riepilogo:
        //     No documentation.
        Z = 44,
        //
        // Riepilogo:
        //     No documentation.
        X = 45,
        //
        // Riepilogo:
        //     No documentation.
        C = 46,
        //
        // Riepilogo:
        //     No documentation.
        V = 47,
        //
        // Riepilogo:
        //     No documentation.
        B = 48,
        //
        // Riepilogo:
        //     No documentation.
        N = 49,
        //
        // Riepilogo:
        //     No documentation.
        M = 50,
        //
        // Riepilogo:
        //     No documentation.
        Comma = 51,
        //
        // Riepilogo:
        //     No documentation.
        Period = 52,
        //
        // Riepilogo:
        //     No documentation.
        Slash = 53,
        //
        // Riepilogo:
        //     No documentation.
        RightShift = 54,
        //
        // Riepilogo:
        //     No documentation.
        Multiply = 55,
        //
        // Riepilogo:
        //     No documentation.
        LeftAlt = 56,
        //
        // Riepilogo:
        //     No documentation.
        Space = 57,
        //
        // Riepilogo:
        //     No documentation.
        Capital = 58,
        //
        // Riepilogo:
        //     No documentation.
        F1 = 59,
        //
        // Riepilogo:
        //     No documentation.
        F2 = 60,
        //
        // Riepilogo:
        //     No documentation.
        F3 = 61,
        //
        // Riepilogo:
        //     No documentation.
        F4 = 62,
        //
        // Riepilogo:
        //     No documentation.
        F5 = 63,
        //
        // Riepilogo:
        //     No documentation.
        F6 = 64,
        //
        // Riepilogo:
        //     No documentation.
        F7 = 65,
        //
        // Riepilogo:
        //     No documentation.
        F8 = 66,
        //
        // Riepilogo:
        //     No documentation.
        F9 = 67,
        //
        // Riepilogo:
        //     No documentation.
        F10 = 68,
        //
        // Riepilogo:
        //     No documentation.
        NumberLock = 69,
        //
        // Riepilogo:
        //     No documentation.
        ScrollLock = 70,
        //
        // Riepilogo:
        //     No documentation.
        NumberPad7 = 71,
        //
        // Riepilogo:
        //     No documentation.
        NumberPad8 = 72,
        //
        // Riepilogo:
        //     No documentation.
        NumberPad9 = 73,
        //
        // Riepilogo:
        //     No documentation.
        Subtract = 74,
        //
        // Riepilogo:
        //     No documentation.
        NumberPad4 = 75,
        //
        // Riepilogo:
        //     No documentation.
        NumberPad5 = 76,
        //
        // Riepilogo:
        //     No documentation.
        NumberPad6 = 77,
        //
        // Riepilogo:
        //     No documentation.
        Add = 78,
        //
        // Riepilogo:
        //     No documentation.
        NumberPad1 = 79,
        //
        // Riepilogo:
        //     No documentation.
        NumberPad2 = 80,
        //
        // Riepilogo:
        //     No documentation.
        NumberPad3 = 81,
        //
        // Riepilogo:
        //     No documentation.
        NumberPad0 = 82,
        //
        // Riepilogo:
        //     No documentation.
        Decimal = 83,
        //
        // Riepilogo:
        //     No documentation.
        Oem102 = 86,
        //
        // Riepilogo:
        //     No documentation.
        F11 = 87,
        //
        // Riepilogo:
        //     No documentation.
        F12 = 88,
        //
        // Riepilogo:
        //     No documentation.
        F13 = 100,
        //
        // Riepilogo:
        //     No documentation.
        F14 = 101,
        //
        // Riepilogo:
        //     No documentation.
        F15 = 102,
        //
        // Riepilogo:
        //     No documentation.
        Kana = 112,
        //
        // Riepilogo:
        //     No documentation.
        AbntC1 = 115,
        //
        // Riepilogo:
        //     No documentation.
        Convert = 121,
        //
        // Riepilogo:
        //     No documentation.
        NoConvert = 123,
        //
        // Riepilogo:
        //     No documentation.
        Yen = 125,
        //
        // Riepilogo:
        //     No documentation.
        AbntC2 = 126,
        //
        // Riepilogo:
        //     No documentation.
        NumberPadEquals = 141,
        //
        // Riepilogo:
        //     No documentation.
        PreviousTrack = 144,
        //
        // Riepilogo:
        //     No documentation.
        AT = 145,
        //
        // Riepilogo:
        //     No documentation.
        Colon = 146,
        //
        // Riepilogo:
        //     No documentation.
        Underline = 147,
        //
        // Riepilogo:
        //     No documentation.
        Kanji = 148,
        //
        // Riepilogo:
        //     No documentation.
        Stop = 149,
        //
        // Riepilogo:
        //     No documentation.
        AX = 150,
        //
        // Riepilogo:
        //     No documentation.
        Unlabeled = 151,
        //
        // Riepilogo:
        //     No documentation.
        NextTrack = 153,
        //
        // Riepilogo:
        //     No documentation.
        NumberPadEnter = 156,
        //
        // Riepilogo:
        //     No documentation.
        RightControl = 157,
        //
        // Riepilogo:
        //     No documentation.
        Mute = 160,
        //
        // Riepilogo:
        //     No documentation.
        Calculator = 161,
        //
        // Riepilogo:
        //     No documentation.
        PlayPause = 162,
        //
        // Riepilogo:
        //     No documentation.
        MediaStop = 164,
        //
        // Riepilogo:
        //     No documentation.
        VolumeDown = 174,
        //
        // Riepilogo:
        //     No documentation.
        VolumeUp = 176,
        //
        // Riepilogo:
        //     No documentation.
        WebHome = 178,
        //
        // Riepilogo:
        //     No documentation.
        NumberPadComma = 179,
        //
        // Riepilogo:
        //     No documentation.
        Divide = 181,
        //
        // Riepilogo:
        //     No documentation.
        PrintScreen = 183,
        //
        // Riepilogo:
        //     No documentation.
        RightAlt = 184,
        //
        // Riepilogo:
        //     No documentation.
        Pause = 197,
        //
        // Riepilogo:
        //     No documentation.
        Home = 199,
        //
        // Riepilogo:
        //     No documentation.
        Up = 200,
        //
        // Riepilogo:
        //     No documentation.
        PageUp = 201,
        //
        // Riepilogo:
        //     No documentation.
        Left = 203,
        //
        // Riepilogo:
        //     No documentation.
        Right = 205,
        //
        // Riepilogo:
        //     No documentation.
        End = 207,
        //
        // Riepilogo:
        //     No documentation.
        Down = 208,
        //
        // Riepilogo:
        //     No documentation.
        PageDown = 209,
        //
        // Riepilogo:
        //     No documentation.
        Insert = 210,
        //
        // Riepilogo:
        //     No documentation.
        Delete = 211,
        //
        // Riepilogo:
        //     No documentation.
        LeftWindowsKey = 219,
        //
        // Riepilogo:
        //     No documentation.
        RightWindowsKey = 220,
        //
        // Riepilogo:
        //     No documentation.
        Applications = 221,
        //
        // Riepilogo:
        //     No documentation.
        Power = 222,
        //
        // Riepilogo:
        //     No documentation.
        Sleep = 223,
        //
        // Riepilogo:
        //     No documentation.
        Wake = 227,
        //
        // Riepilogo:
        //     No documentation.
        WebSearch = 229,
        //
        // Riepilogo:
        //     No documentation.
        WebFavorites = 230,
        //
        // Riepilogo:
        //     No documentation.
        WebRefresh = 231,
        //
        // Riepilogo:
        //     No documentation.
        WebStop = 232,
        //
        // Riepilogo:
        //     No documentation.
        WebForward = 233,
        //
        // Riepilogo:
        //     No documentation.
        WebBack = 234,
        //
        // Riepilogo:
        //     No documentation.
        MyComputer = 235,
        //
        // Riepilogo:
        //     No documentation.
        Mail = 236,
        //
        // Riepilogo:
        //     No documentation.
        MediaSelect = 237
    }
}
