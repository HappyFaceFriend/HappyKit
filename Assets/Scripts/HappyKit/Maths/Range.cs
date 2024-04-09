using System;

namespace HappyKit
{
    /// <summary>
    /// Range [Start, End)
    /// </summary>
    [System.Serializable]
    struct RangeInt
    {
        public int Start;
        public int End;
        public RangeInt(int startInclusive, int endExclusive)
        {
            Start = startInclusive;
            End = endExclusive;
        }
        public bool Contains(int index) => Start <= index && index < End;

    }

    /// <summary>
    /// Range [Start, End)
    /// </summary>
    [System.Serializable]
    public struct Range
    { 
        /// <summary>
        /// Inclusive start
        /// </summary>
        public float Start;
        /// <summary>
        /// Exclusive end
        /// </summary>
        public float End;
        public Range(float startInclusive, float endExclusive)
        {
            Start = startInclusive;
            End = endExclusive;
        }
        /// <summary>
        /// Checks if value is in the range [Start, End)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(float value) => Start <= value && value < End;

    }
}