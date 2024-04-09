
namespace HappyKit
{
    public static class Curves
    {
        public static float EaseIn(float x)
        {
            return x * x;
        }
        public static float EaseOut(float x)
        {
            x = x - 1;
            return 1 - x * x;
        }
        public static float Linear(float x)
        {
            return x;
        }
        public static float Constant(float x)
        {
            return 1;
        }
    }

}