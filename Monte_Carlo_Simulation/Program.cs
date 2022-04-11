

using System.Security.Cryptography;

namespace Monte_Carlo_Simulation;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter the T parameter:");
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
        Console.WriteLine("Enter the volatility sigma parameter:");
        var sigmaString = Console.ReadLine();
        var sigma = Convert.ToDouble(sigmaString);
        Console.WriteLine("Enter the mu sigma parameter:");
        var muString = Console.ReadLine();
        var mu = Convert.ToDouble(muString);
        Console.WriteLine("Enter the number of paths N parameter:");
        var nString = Console.ReadLine();
        var N = Convert.ToInt64(nString);
        var o = new Option();
        var call =o.EuropeanCall(N, T, mu, sigma, s0,K, r);
        var callGreeks = o.CallGreeks(N, T, mu, sigma, s0, K, r);
        Console.WriteLine("The European call option price is " + call["call option price"]);
        Console.WriteLine("The standard error of European call option price Estimation is " + call["standard error estimation"]);
        Console.WriteLine("The call option delta is " + callGreeks["delta"]);
        Console.WriteLine("The call option gamma is " + callGreeks["gamma"]);
        Console.WriteLine("The call option vega is " + callGreeks["vega"]);
        Console.WriteLine("The call option theta is " + callGreeks["theta"]);
        Console.WriteLine("The call option rho is " + callGreeks["rho"]);
        var put = o.EuropeanPut(N, T, mu, sigma, s0, K, r);
        var putGreeks = o.PutGreeks(N, T, mu, sigma, s0, K, r);
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
    public double[,] StockPath(Int64 N, double T, double mu, double sigma, double s0)
    {
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

        return stockpriceArray;

    }
}

class Option
    {

        public Dictionary<string, double> EuropeanCall(Int64 N, double T, double mu, double sigma, double s0, double k, double r)
        {
            var s= new StockSimulation();
            double [,] price = s.StockPath(N, T, mu, sigma, s0);
            // payoff & option price calculation
            var sum = 0.0;
            for (int i = 0; i < N; i++)
            {
               var payoff = price[i,(int) (T * 252)] - k;
               if (payoff > 0);
               {
                   sum += payoff;
               }
            }

            var mean = sum / N;
            var callprice = mean * Math.Exp(-r * T);

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

        public Dictionary<string, double> EuropeanPut(Int64 N, double T, double mu, double sigma, double s0, double k, double r)
        {
            var s= new StockSimulation();
            double [,] price = s.StockPath(N, T, mu, sigma, s0);
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

        public Dictionary<string, double> CallGreeks(Int64 N, double T, double mu, double sigma, double s0, double k, double r)
        {
            var deltaS = s0 / 10; //10% change
            var deltaSigma = sigma / 10; //10% change
            var deltaT = T / 10;
            var deltaR = r / 10;
            var originalPrice = EuropeanCall(N, T, mu, sigma, s0, k, r)["call option price"];
            // Delta
            var priceplusS0 = EuropeanCall(N, T, mu, sigma, s0 + deltaS, k, r)["call option price"];
            var priceminusS0 = EuropeanCall(N, T, mu, sigma, s0 - deltaS, k, r)["call option price"];
            var delta = (priceplusS0 - priceminusS0) / (2 * deltaS);
            //gamma 
            var gamma = (priceplusS0 - 2 * originalPrice + priceminusS0) / Math.Pow(deltaS,2);
            // vega
            var priceplusSigma = EuropeanCall(N, T, mu, sigma+deltaSigma, s0, k, r)["call option price"];
            var priceminusSigma = EuropeanCall(N, T, mu, sigma-deltaSigma, s0, k, r)["call option price"];
            var vega = (priceplusSigma - priceminusSigma) / (2 * deltaSigma);
            // Theta
            var priceplusT = EuropeanCall(N, T + deltaT, mu, sigma, s0, k, r)["call option price"];
            var theta = (priceplusT - originalPrice) / deltaT;
            // Rho 
            var priceplusR = EuropeanCall(N, T, mu, sigma, s0, k, r+deltaR)["call option price"];
            var priceminusR = EuropeanCall(N, T, mu, sigma, s0, k, r-deltaR)["call option price"];
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
        
        public Dictionary<string, double> PutGreeks(long N, double T, double mu, double sigma, double s0, double k, double r)
        {
            var deltaS = s0 / 10; //10% change
            var deltaSigma = sigma / 10; //10% change
            var deltaT = T / 10;
            var deltaR = r / 10;
            var originalPrice = EuropeanPut(N, T, mu, sigma, s0, k, r)["put option price"];
            // Delta
            var priceplusS0 = EuropeanPut(N, T, mu, sigma, s0 + deltaS, k, r)["put option price"];
            var priceminusS0 = EuropeanPut(N, T, mu, sigma, s0 - deltaS, k, r)["put option price"];
            var delta = (priceplusS0 - priceminusS0) / (2 * deltaS);
            //gamma 
            var gamma = (priceplusS0 - 2 * originalPrice + priceminusS0) / Math.Pow(deltaS,2);
            // vega
            var priceplusSigma = EuropeanPut(N, T, mu, sigma+deltaSigma, s0, k, r)["put option price"];
            var priceminusSigma = EuropeanPut(N, T, mu, sigma-deltaSigma, s0, k, r)["put option price"];
            var vega = (priceplusSigma - priceminusSigma) / (2 * deltaSigma);
            // Theta
            var priceplusT = EuropeanPut(N, T + deltaT, mu, sigma, s0, k, r)["put option price"];
            var theta = (priceplusT - originalPrice) / deltaT;
            // Rho 
            var priceplusR = EuropeanPut(N, T, mu, sigma, s0, k, r+deltaR)["put option price"];
            var priceminusR = EuropeanPut(N, T, mu, sigma, s0, k, r-deltaR)["put option price"];
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


