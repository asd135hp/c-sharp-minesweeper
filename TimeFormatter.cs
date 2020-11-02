namespace MultiplayerMinesweeper.Core
{
    public static class TimeFormatter
    {
        private static string AddZero(int component) => component >= 10 ? component.ToString() : $"0{component}";
        public static string GetTime(int time)
        {
            int hours = time / 3600,
                minutes = (time - hours) / 60,
                seconds = time - hours * 3600 - minutes * 60;

            return hours != 0 ?
                $"{AddZero(hours)}:{AddZero(minutes)}:{AddZero(seconds)}" :
                $"{AddZero(minutes)}:{AddZero(seconds)}";
        }
    }
}
