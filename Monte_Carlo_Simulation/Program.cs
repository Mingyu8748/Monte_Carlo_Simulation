using System.Net.Http.Headers;
using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace Monte_Carlo_Simulation;

class Program
{
    static void Main(string[] args)
    {
        var mu = 0.2; // annualized volatility parameter
        Console.WriteLine("The annualized mu return parameter used in this simulator is: " + mu);
        var sigma = 0.15; // annualized volatility parameter
        Console.WriteLine("The annualized sigma volatility parameter used in this simulator is : " + sigma);
        // user input parameters
        Console.WriteLine("Use the what variance reduction method for this simulation? Enter Antithetic or ControlVariate or Both : ");
        string? varianceReduction = Console.ReadLine();
        Console.WriteLine("Enter the T parameter, the number of steps will be 252 * T (daily time step):");
        var tString = Console.ReadLine();
        var T = Convert.ToDouble(tString);
        Console.WriteLine("Enter the stock price at time 0 S0 parameter:");
        var s0String = Console.ReadLine();
        var s0 = Convert.ToDouble(s0String);
        Console.WriteLine("Enter the striking price K parameter:");
        var kString = Console.ReadLine();
        var K = Convert.ToDouble(kString);
        Console.WriteLine("Enter the risk-free rate r parameter:");
        var rString = Console.ReadLine();
        var r = Convert.ToDouble(rString);
        Console.WriteLine("Enter the number of paths N parameter:");
        var nString = Console.ReadLine();
        var N = Convert.ToInt64(nString);
        Console.WriteLine("Enter the payment amount for digital option,enter 0 if its not a digital option");
        var pstring = Console.ReadLine();
        var p = Convert.ToDouble(pstring);
        //Multi-threading
        Console.WriteLine("Do you want to use multi-threading for the stock price simulation? Enter Yes or No");
        var multithread_option = Console.ReadLine();
        Thread t1 = new Thread(StockSimulator.StockPath);
        Thread t2 = new Thread(StockSimulator.StockPath);
        t1.Start(new Param(N, T, mu, sigma, s0, varianceReduction));
        t2.Start(new Param(N, T, mu, sigma, s0, varianceReduction));
        t1.Join();
        t2.Join();
        // model results
        var o = new Option();
        double[,] price = StockSimulator.StockPath(N, T, mu, sigma, s0, varianceReduction,
            RandomNumberGenerator.Generate(N, T));
        var asiancall = o.AsianCall(N, T, mu, sigma, s0, K, r, price);
        //var callGreeks = o.CallGreeks(N, T, mu, sigma, s0, K, r, varianceReduction);
        Console.WriteLine("The Asian call option price is " + asiancall);
        var asianput = o.AsianPut(N, T, mu, sigma, s0, K, r, price);
        Console.WriteLine("The Asian put option price is " + asianput);
        var digitalcall  = o.DigitalCall(N, T, mu, sigma, s0, K, r, p,price);
        Console.WriteLine("The digital call option price is " + digitalcall);
        var digitalput  = o.DigitalPut(N, T, mu, sigma, s0, K, r, p,price);
        Console.WriteLine("The digital put option price is " + digitalput);
        var lookbackcall  = o.LookbackCall(N, T, mu, sigma, s0, K, r, price);
        Console.WriteLine("The lookback call option price is " + lookbackcall);        
        var lookbackput  = o.LookbackPut(N, T, mu, sigma, s0, K, r,price);
        Console.WriteLine("The lookback put option price is " + lookbackput);
        var rangeoption  = o.RangeOption(N, T, mu, sigma, s0, K, r,price);
        Console.WriteLine("The Range option price is " + rangeoption);
        //Console.WriteLine("The standard error of European call option price Estimation is " +
                          //call["standard error estimation"]);
        //Console.WriteLine("The call option delta is " + callGreeks["delta"]);
        //Console.WriteLine("The call option gamma is " + callGreeks["gamma"]);
        //Console.WriteLine("The call option vega is " + callGreeks["vega"]);
        //Console.WriteLine("The call option rho is " + callGreeks["rho"]);
        //var put = o.EuropeanPut(N, T, mu, sigma, s0, K, r, price);
        //var putGreeks = o.PutGreeks(N, T, mu, sigma, s0, K, r, varianceReduction);
        //Console.WriteLine("The European call option price is " + put["put option price"]);
        //Console.WriteLine("The standard error of European call option price Estimation is " +
         //                 put["standard error estimation"]);
        //Console.WriteLine("The call option delta is " + putGreeks["delta"]);
        //Console.WriteLine("The call option gamma is " + putGreeks["gamma"]);
        //Console.WriteLine("The call option vega is " + putGreeks["vega"]);
        //Console.WriteLine("The call option theta is " + putGreeks["theta"]);
        //Console.WriteLine("The call option rho is " + putGreeks["rho"]);
    }
}