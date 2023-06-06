using System.Collections.Generic;

namespace Match3
{
    public class BoardModel
    {
        public UIBoard BoardUI { get; private set; }
        public List<TileInfo> TileInfos { get; private set; }
        public int CountInLine { get; private set; }
        public int ColumnCount { get; private set; }
        public int RowCount { get; private set; }
        public int CountOfBlocks { get; private set; }

        public int StepsCountForLevel { get; private set; }

        public int ScoreCountForLevel { get; private set; }

        public BoardModel(UIBoard boardUI, List<TileInfo> tileInfos, int countInLine, int columnCount, int rowCount, int countOfBlock, int stepsCount, int scoreCount)
        {
            BoardUI = boardUI;
            TileInfos = tileInfos;
            CountInLine = countInLine;
            ColumnCount = columnCount;
            RowCount = rowCount;
            CountOfBlocks = countOfBlock;
            StepsCountForLevel = stepsCount;
            ScoreCountForLevel = scoreCount;
        }
    }
}