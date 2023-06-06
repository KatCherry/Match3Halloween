using System;

namespace Match3
{
    [Serializable]
    public class GameData
    {
        public int ScoreCount;
        public int StepCount;

        public GameData(int scoreCount, int stepCount)
        {
            ScoreCount = scoreCount;
            StepCount = stepCount;
        }
    }
}