using System;
using System.Collections.Generic;
using Application.Game.TicTacToy;

namespace Application.Services.UserData
{
    [Serializable]
    public class GameSessionData
    {
        public string StartTime;
        public List<StepCommand> SequenceCommands = new List<StepCommand>(9);
        public GameResultType Result;
    }

    [Serializable]
    public class StepCommand
    {
        public SlotType SlotType;
        public int SlotIndex;
    }
}