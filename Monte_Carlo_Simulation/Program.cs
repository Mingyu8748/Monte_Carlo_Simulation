

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
        string VarianceReduction = Console.ReadLine();
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
        Console.WriteLine("Enter the base number of Van der Corput sequence generator");
        // model results
        var o = new Option();
        var call =o.EuropeanCall(N, T, mu, sigma, s0,K, r,VarianceReduction);
        var callGreeks = o.CallGreeks(N, T, mu, sigma, s0, K, r,VarianceReduction);
        Console.WriteLine("The European call option price is " + call["call option price"]);
        Console.WriteLine("The standard error of European call option price Estimation is " + call["standard error estimation"]);
        Console.WriteLine("The call option delta is " + callGreeks["delta"]);
        Console.WriteLine("The call option gamma is " + callGreeks["gamma"]);
        Console.WriteLine("The call option vega is " + callGreeks["vega"]);
        Console.WriteLine("The call option theta is " + callGreeks["theta"]);
        Console.WriteLine("The call option rho is " + callGreeks["rho"]);
        var put = o.EuropeanPut(N, T, mu, sigma, s0, K, r,VarianceReduction);
        var putGreeks = o.PutGreeks(N, T, mu, sigma, s0, K, r,VarianceReduction);
        Console.WriteLine("The European call option price is " + put["put option price"]);
        Console.WriteLine("The standard error of European call option price Estimation is " + put["standard error estimation"]);
        Console.WriteLine("The call option delta is " + putGreeks["delta"]);
        Console.WriteLine("The call option gamma is " + putGreeks["gamma"]);
        Console.WriteLine("The call option vega is " + putGreeks["vega"]);
        Console.WriteLine("The call option theta is " + putGreeks["theta"]);
        Console.WriteLine("The call option rho is " + putGreeks["rho"]);
    }
}


class RandomNumberGenerator
{
    public double[,] RandomNumber(Int64 N, double T)
    {
            double[,] randomNumberArray = new double[N,(int) (T * 252)];
            var row = 1;
            var rand = new Random();

            while (row <= N)
            {
                var column = 1;
                while (column <= (int) (T * 252))
                {
                    var u1 = 1.0 - rand.NextDouble();
                    var u2 = 1.0 - rand.NextDouble();

                    randomNumberArray[row - 1, column - 1] =
                        Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

                    column++;
                }

                row++;

            }

            return randomNumberArray;
    }
}

class StockSimulation
{
    public double[,] StockPath(Int64 N, double T, double mu, double sigma, double s0, string VarianceReduction)
    {
        var dailymu = mu / 252;
        var dailysigma = sigma / (Math.Pow(252, 0.5));
        var random = new RandomNumberGenerator();
        double[,] inputRandomNumber = random.RandomNumber(N, T);
        double[,] stockpriceArray = new double [N, (int) (T * 252) + 1];
        // assign s0 to the first column of simulated stock price metric
        var index = 1;
        while (index <= N)
        {
            stockpriceArray[index - 1, 0] = s0;
            index++;
        }

        //simulation for stock price
        if (VarianceReduction == "Antithetic")
        {
            const double dt = 0.00396825; //default dt as 1 day (1/252)
            var rowpart1 = 1;
            while (rowpart1 <= (int)N/2)
            {
                var colpart1 = 2;
                while (colpart1 <= (int) (T * 252) + 1)
                {
                    stockpriceArray[rowpart1 - 1, colpart1 - 1] =
                        stockpriceArray[rowpart1 - 1, colpart1 - 2] *
                        Math.Exp((dailymu - 0.5 * Math.Pow(dailysigma, 2)) * dt +
                                 sigma * Math.Pow(dt, 0.5) *
                                 inputRandomNumber[rowpart1 - 1, colpart1 - 2]); //default dt as 1 day (1/252)
                    colpart1++;
                }

                rowpart1++;
            }

            var rowpart2 = (int) N/2;
            var colpart2 = 2;
            while (rowpart2 <= N)
            {
                while (colpart2 <= (int) (T * 252) + 1)
                {
                    stockpriceArray[rowpart2 - 1, colpart2 - 1] =
                        stockpriceArray[rowpart2 - 1, colpart2 - 2] *
                        Math.Exp((dailymu - 0.5 * Math.Pow(dailysigma, 2)) * dt -
                                 sigma * Math.Pow(dt, 0.5) *
                                 inputRandomNumber[rowpart2 - 1, colpart2 - 2]); //default dt as 1 day (1/252)
                    colpart2++;
                }

                rowpart2++;
            }
        }
        else
        {
            const double dt = 0.00396825; //default dt as 1 day (1/252)
            var row = 1;
            while (row <= N)
            {
                var col = 2;
                while (col <= (int) (T * 252) + 1)
                {
                    stockpriceArray[row - 1, col - 1] =
                        stockpriceArray[row - 1, col - 2] *
                        Math.Exp((mu - 0.5 * Math.Pow(sigma, 2)) * dt +
                                 sigma * Math.Pow(dt, 0.5) *
                                 inputRandomNumber[row - 1, col - 2]); //default dt as 1 day (1/252)
                    col++;
                }

                row++;
            }
        }

        return stockpriceArray;

    }
}

class Option
    {

        public Dictionary<string, double> EuropeanCall(Int64 N, double T, double mu, double sigma, double s0, double k, double r, string VarianceReduction)
        {
            var s = new StockSimulation();
            double [,] price = s.StockPath(N, T, mu, sigma, s0,  VarianceReduction);
            // payoff & option price calculation
            var sum = 0.0;
                for (int i = 0; i < N; i++)
                {
                    var payoff = price[i, (int) (T * 252)] - k;
                    if (payoff > 0) ;
                    {
                        sum += payoff;
                    }
                }
            var mean = sum / N;
            var callprice = mean * Math.Exp(-r * T);
            //Control variate method 
            var K_2 = 50;
            var sum2 = 0.0;
            for (int i = 0; i < N; i++)
                {
                    var payoff2 = price[i, (int) (T * 252)] - K_2;
                    if (payoff2 > 0) ;
                    {
                        sum2 += payoff2;
                    }
                }
            var bnum = (sum2 - sum2 / N) * (sum -mean) ;
            var bdenom = Math.Pow((sum - mean), 2);
            var b = bnum / bdenom;
            // standard error of the call price
            var diffsum = 0.0;
            for (int i = 0; i < N; i++)
            {
                var payoff = price[i,(int) (T * 252)] - k;
                var eachprice = payoff * Math.Exp(-r * T) ;
                var diff = Math.Pow((eachprice - callprice), 2);
                diffsum += diff;
            }

            var std = diffsum / (N - 1);
            var std_error = std/Math.Pow(N, 0.5);

            return new Dictionary<string, double>
            {
                {"call option price", callprice},
                {"standard error estimation", std_error}
            };

        }

        public Dictionary<string, double> EuropeanPut(Int64 N, double T, double mu, double sigma, double s0, double k, double r, string antithetic)
        {
            var s= new StockSimulation();
            double [,] price = s.StockPath(N, T, mu, sigma, s0, antithetic);
            var sum = 0.0;
            for (int i = 0; i < N; i++)
            {
                var payoff = k-price[i,(int) (T * 252)];
                if (payoff > 0);
                {
                    sum += payoff;
                }
            }

            var putprice = (sum/N) * Math.Exp(-r*T);


            // standard error of the call price
            var diffsum = 0.0;
            for (int i = 0; i < N; i++)
            {
                var payoff = price[i,(int) (T * 252)] - k;
                var eachprice = payoff * Math.Exp(-r * T) ;
                var diff = Math.Pow((eachprice - putprice), 2);
                diffsum += diff;
            }

            var std = diffsum / (N - 1);
            var std_error = std/Math.Pow(N, 0.5);

            return new Dictionary<string, double>
            {
                {"put option price", putprice},
                {"standard error estimation", std_error}
            };

        }

        public Dictionary<string, double> CallGreeks(Int64 N, double T, double mu, double sigma, double s0, double k, double r, string antithetic)
        {
            var deltaS = s0 / 10; //10% change
            var deltaSigma = sigma / 10; //10% change
            var deltaT = T / 10;
            var deltaR = r / 10;
            var originalPrice = EuropeanCall(N, T, mu, sigma, s0, k, r, antithetic)["call option price"];
            // Delta
            var priceplusS0 = EuropeanCall(N, T, mu, sigma, s0 + deltaS, k, r,antithetic)["call option price"];
            var priceminusS0 = EuropeanCall(N, T, mu, sigma, s0 - deltaS, k, r,antithetic)["call option price"];
            var delta = (priceplusS0 - priceminusS0) / (2 * deltaS);
            //gamma
            var gamma = (priceplusS0 - 2 * originalPrice + priceminusS0) / Math.Pow(deltaS,2);
            // vega
            var priceplusSigma = EuropeanCall(N, T, mu, sigma+deltaSigma, s0, k, r,antithetic)["call option price"];
            var priceminusSigma = EuropeanCall(N, T, mu, sigma-deltaSigma, s0, k, r,antithetic)["call option price"];
            var vega = (priceplusSigma - priceminusSigma) / (2 * deltaSigma);
            // Theta
            var priceplusT = EuropeanCall(N, T + deltaT, mu, sigma, s0, k, r,antithetic)["call option price"];
            var theta = (priceplusT - originalPrice) / deltaT;
            // Rho 
            var priceplusR = EuropeanCall(N, T, mu, sigma, s0, k, r+deltaR,antithetic)["call option price"];
            var priceminusR = EuropeanCall(N, T, mu, sigma, s0, k, r-deltaR, antithetic)["call option price"];
            var rho = (priceplusR - priceminusR) / (2 * deltaR);
            return new Dictionary<string, double>
            {
                {"delta", delta},
                {"gamma", gamma},
                {"vega", vega},
                {"theta", theta},
                {"rho", rho},

            };
        }

        public Dictionary<string, double> PutGreeks(long N, double T, double mu, double sigma, double s0, double k, double r, string antithetic)
        {
            var deltaS = s0 / 10; //10% change
            var deltaSigma = sigma / 10; //10% change
            var deltaT = T / 10;
            var deltaR = r / 10;
            var originalPrice = EuropeanPut(N, T, mu, sigma, s0, k, r,antithetic)["put option price"];
            // Delta
            var priceplusS0 = EuropeanPut(N, T, mu, sigma, s0 + deltaS, k, r,antithetic)["put option price"];
            var priceminusS0 = EuropeanPut(N, T, mu, sigma, s0 - deltaS, k, r,antithetic)["put option price"];
            var delta = (priceplusS0 - priceminusS0) / (2 * deltaS);
            //gamma
            var gamma = (priceplusS0 - 2 * originalPrice + priceminusS0) / Math.Pow(deltaS,2);
            // vega
            var priceplusSigma = EuropeanPut(N, T, mu, sigma+deltaSigma, s0, k, r,antithetic)["put option price"];
            var priceminusSigma = EuropeanPut(N, T, mu, sigma-deltaSigma, s0, k, r,antithetic)["put option price"];
            var vega = (priceplusSigma - priceminusSigma) / (2 * deltaSigma);
            // Theta
            var priceplusT = EuropeanPut(N, T + deltaT, mu, sigma, s0, k, r,antithetic)["put option price"];
            var theta = (priceplusT - originalPrice) / deltaT;
            // Rho
            var priceplusR = EuropeanPut(N, T, mu, sigma, s0, k, r+deltaR,antithetic)["put option price"];
            var priceminusR = EuropeanPut(N, T, mu, sigma, s0, k, r-deltaR,antithetic)["put option price"];
            var rho = (priceplusR - priceminusR) / (2 * deltaR);
            return new Dictionary<string, double>
            {
                {"delta", delta},
                {"gamma", gamma},
                {"vega", vega},
                {"theta", theta},
                {"rho", rho},

            };
        }
    }


