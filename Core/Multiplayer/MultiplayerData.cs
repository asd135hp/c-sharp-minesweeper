﻿using SplashKitSDK;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MultiplayerMinesweeper.Core.Multiplayer
{
    public enum GameState
    {
        Connecting,
        Waiting,
        Playing,
        HostWin,
        GuestWin,
        HostExited,
        GuestExited
    }

    public class MultiplayerData : IJson
    {
        public MultiplayerData(GameSettings settings)
        {
            GameSettings = settings;
            Host = new PlayerData(settings);
            Guest = new PlayerData(settings);
            ChangeGameState(GameState.Connecting);
        }

        public GameState State { get; private set; }
        public readonly PlayerData Host, Guest;
        public readonly GameSettings GameSettings;

        /// <summary>
        /// Change game state of this object
        /// (and possibly on the game's JSON data on the server, too)
        /// </summary>
        /// <param name="state">State of the game now</param>
        /// <param name="runTask">Will the method run the task to change game state on the server?</param>
        /// <param name="waitTask">WIll the method force main thread to wait for uploading task to be done?</param>
        public void ChangeGameState(GameState state, bool runTask = true, bool waitTask = false)
        {
            State = state;
            if (runTask)
            {
                var task = Task.Run(() =>
                {
                    Firebase.Put(((int)State).ToString(), $"{Constants.GAMENAME}{GameSettings.GameID}/state")
                        .Wait(Constants.TIMEOUT);
                });

                if (waitTask) task.Wait();
            }
        }

        /// <summary>
        /// Merge with current multiplayer data object with a json string
        /// </summary>
        /// <param name="jsonString">JSON string with expected fields for player data</param>
        /// <param name="role">Role of multiplayer game connection (host vs guest)</param>
        public void Merge(string jsonString, MultiplayerRole role) => Merge(SplashKit.CreateJson(jsonString), role);

        /// <summary>
        /// Merge with current multiplayer data object with a json object
        /// </summary>
        /// <param name="jsonString">JSON object with expected fields for player data</param>
        /// <param name="role">Role of multiplayer game connection (host vs guest)</param>
        public void Merge(Json json, MultiplayerRole role)
        {
            List<string> board = new List<string>();
            int time = json.ReadInteger("time");
            int flag = json.ReadInteger("flag");
            json.ReadArray("board", ref board);

            Merge(time, flag, board.ToArray(), role);
        }

        /// <summary>
        /// Merge with current multiplayer data object with all needed data
        /// </summary>
        /// <param name="time">Time played for certain role</param>
        /// <param name="flag">Flags remaining for certain role</param>
        /// <param name="board">Stringified board for merging</param>
        /// <param name="role">Role of multiplayer game connection (host vs guest)</param>
        public void Merge(int time, int flag, string[] board, MultiplayerRole role)
        {
            switch (role)
            {
                case MultiplayerRole.Guest:
                    Guest.Time = time;
                    Guest.Flag = flag;
                    Guest.Board.MergeBoard(board);
                    break;
                case MultiplayerRole.Host:
                    Host.Time = time;
                    Host.Flag = flag;
                    Host.Board.MergeBoard(board);
                    break;
            }
        }

        public string ToJsonString()
        {
            var json = SplashKit.CreateJson();
            json.AddString("host", Host.ToJsonString());
            json.AddString("guest", Guest.ToJsonString());
            json.AddString("settings", GameSettings.ToJsonString());
            return SplashKit.JsonToString(json);
        }

        public void FromJson(Json json)
        {
            // Only when the game session is playing that json is parsed and merged
            if(State == GameState.Playing)
            {
                Json host = json.ReadObject("host"),
                    guest = json.ReadObject("guest");

                if(host != null && guest != null)
                {
                    Host.FromJson(host);
                    Guest.FromJson(guest);
                }

                if (json.HasKey("state")) State = (GameState)json.ReadInteger("state");
            }
        }
        public void FromJson(string jsonString)
        {
            try
            {
                FromJson(SplashKit.CreateJson(jsonString));
            }
            finally
            {
                // empty block here, just to catch the exception without much penalty
            }
        }
    }
}
