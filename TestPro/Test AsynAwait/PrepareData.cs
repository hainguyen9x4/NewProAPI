namespace Test_AsynAwait
{
    public class PrepareData
    {
        public async Task<int> Prepare(int timeDelay, int maxValue)
        {

            // You can do work here that doesn't rely on the string from GetStringAsync.
            var value = this.GetHashCode();
            var rand = new Random();

            Console.WriteLine($"Start PrepareData-value: {value.ToString()}");

            Thread.Sleep(rand.Next(timeDelay * 1000, maxValue * 1000));

            Console.WriteLine($"-->End PrepareData-value: {value.ToString()}");
            return value;
        }
    }
}
