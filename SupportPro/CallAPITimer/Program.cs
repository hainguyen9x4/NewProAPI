using Pro.Common;
using System.Timers;

public static class Program
{
    public static void Main()
    {
        Console.WriteLine("Main thread: starting a timer");

        Console.WriteLine($"Start-CallAPI:{DateTime.Now.ToString()}");
        CallAPI();

        System.Timers.Timer aTimer = new System.Timers.Timer();
        aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
        aTimer.Interval = 1000 * 5 * 60;//5 min
        aTimer.Enabled = true;

        Console.WriteLine("Press \'q\' to quit the sample.");
        while (Console.Read() != 'q') ;
    }
    // This method's signature must match the TimerCallback delegate
    private static void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        Console.WriteLine($"CallAPI:{DateTime.Now.ToString()}");
        CallAPI();
    }
    private static void CallAPI()
    {
        new ApiHelper().Get<bool>($"/api/GetData/StartGetData", "https://localhost:5001");
    }
}