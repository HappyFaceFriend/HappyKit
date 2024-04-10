
namespace HappyKit
{
    public static class Curves
    {
        /// <summary>
        /// x * x
        /// </summary>
        public static float EaseIn(float x)
        {
            return x * x;
        }
        /// <summary>
        /// 1 - (x-1)^2
        /// </summary>
        public static float EaseOut(float x)
        {
            x = x - 1;
            return 1 - x * x;
        }
        /// <summary>
        /// x
        /// </summary>
        public static float Linear(float x)
        {
            return x;
        }
        /// <summary>
        /// 1
        /// </summary>
        public static float Constant(float x)
        {
            return 1;
        }
    }

}