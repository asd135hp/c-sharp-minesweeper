namespace MultiplayerMinesweeper.Core.StopWatch
{
    class Counter
    {
        public Counter()
        {
            Value = 0;
            ID = "Random";
        }
        public Counter(string id) : this()
        {
            ID = id;
        }

        public readonly string ID;
        public int Value { get; private set; }

        public void Increment() => Value++;
        public void Reset() => Value = 0;
    }
}
