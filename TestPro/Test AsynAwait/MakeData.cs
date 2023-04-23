namespace Test_AsynAwait
{
    public class MakeData
    {
        public async void MakeData1()
        {

            var prePare = new PrepareData();
            await prePare.Prepare(1, 5);
            await prePare.Prepare(1, 5);

        }
    }
}
