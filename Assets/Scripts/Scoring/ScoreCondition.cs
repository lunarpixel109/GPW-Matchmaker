using System;

namespace Scoring {

    [Serializable]
    public struct ScoreCondition {
        public Property property;
        public float excellentBounds;
        public float goodBounds;
        public float okBounds;
    }
}