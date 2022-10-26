namespace Monte_Carlo_Simulation;

class Option
{
    public Dictionary<string, double> EuropeanCall(Int64 n, double t, double mu, double sigma, double s0, double k,
        double r, double[,] price)
    {
        // payoff & option price calculation
        var sum = 0.0;
        for (int i = 0; i < n; i++)
        {
            var payoff = price[i, (int) (t * 252)] - k;
            if (payoff > 0) ;
            {
                sum += payoff;
            }
        }

        var mean = sum / n;
        var callprice = mean * Math.Exp(-r * t);
        //Control variate method 
        var K_2 = 50;
        var sum2 = 0.0;
        for (int i = 0; i < n; i++)
        {
            var payoff2 = price[i, (int) (t * 252)] - K_2;
            if (payoff2 > 0) ;
            {
                sum2 += payoff2;
            }
        }

        var bnum = (sum2 - sum2 / n) * (sum - mean);
        var bdenom = Math.Pow((sum - mean), 2);
        var b = bnum / bdenom;
        // standard error of the call price
        var diffsum = 0.0;
        for (int i = 0; i < n; i++)
        {
            var payoff = price[i, (int) (t * 252)] - k;
            var eachprice = payoff * Math.Exp(-r * t);
            var diff = Math.Pow((eachprice - callprice), 2);
            diffsum += diff;
        }

        var std = diffsum / (n - 1);
        var std_error = std / Math.Pow(n, 0.5);

        return new Dictionary<string, double>
        {
            {"call option price", callprice},
            {"standard error estimation", std_error}
        };
    }

    public Dictionary<string, double> EuropeanPut(Int64 N, double T, double mu, double sigma, double s0, double k,
        double r, double[,] price)
    {
        var sum = 0.0;
        for (int i = 0; i < N; i++)
        {
            var payoff = k - price[i, (int) (T * 252)];
            if (payoff > 0) ;
            {
                sum += payoff;
            }
        }

        var putprice = (sum / N) * Math.Exp(-r * T);


        // standard error of the call price
        var diffsum = 0.0;
        for (int i = 0; i < N; i++)
        {
            var payoff = price[i, (int) (T * 252)] - k;
            var eachprice = payoff * Math.Exp(-r * T);
            var diff = Math.Pow((eachprice - putprice), 2);
            diffsum += diff;
        }

        var std = diffsum / (N - 1);
        var std_error = std / Math.Pow(N, 0.5);

        return new Dictionary<string, double>
        {
            {"put option price", putprice},
            {"standard error estimation", std_error}
        };
    }

    public double AsianCall(Int64 N, double T, double mu, double sigma, double s0, double k,
        double r, double[,] price)
    {
        double[,] AveragePrice = new double [N,1];
        var sum = 0.0;
        for (int j = 0; j < N; j++)
        {
            for (int i = 0; i < (int) (T * 252) + 1; i++)
            {
                var price_path = price[j, i];
                sum += price_path;
            }

            AveragePrice[j, 0] = sum / (T * 252+1);
        }

        var sum2 = 0.0;
        for (int i = 0; i < N; i++)
        {
            var payoff = AveragePrice[i, 0] - k;
            if (payoff > 0) ;
            {
                sum2 += payoff;
            }
        }

        var callprice = (sum2 / N) * Math.Exp(-r * T);

        return callprice;

    }
    
    public double AsianPut(Int64 N, double T, double mu, double sigma, double s0, double k,
        double r, double[,] price)
    {
        double[,] AveragePrice = new double [N,1];
        var sum = 0.0;
        for (int j = 0; j < N; j++)
        {
            for (int i = 0; i < (int) (T * 252) + 1; i++)
            {
                var price_path = price[j, i];
                sum += price_path;
            }

            AveragePrice[j, 0] = sum / (T * 252+1);
        }

        var sum2 = 0.0;
        for (int i = 0; i < N; i++)
        {
            var payoff = k - AveragePrice[i, 0];
            if (payoff > 0) ;
            {
                sum2 += payoff;
            }
        }

        var putprice = (sum2 / N) * Math.Exp(-r * T);

        return putprice;

    }
    
    public double DigitalPut(Int64 N, double T, double mu, double sigma, double s0, double k,
        double r, double p, double[,] price)
    {
        var sum = 0.0;
        for (int i = 0; i < N; i++)
        {
            var payoff = k - price[i, (int) (T * 252)];
            if (payoff > 0) ;
            {
                sum += p;
            }
        }
        var optionprice = (sum/N) * Math.Exp(-r * T);

        return optionprice;

    }

    public double DigitalCall(Int64 N, double T, double mu, double sigma, double s0, double k,
        double r, double p, double[,] price)
    {
        var sum = 0.0;
        for (int i = 0; i < N; i++)
        {
            var payoff = price[i, (int) (T * 252)] -k;
            if (payoff > 0) ;
            {
                sum += p;
            }
        }
        var optionprice = (sum/N) * Math.Exp(-r * T);

        return optionprice;

    }
    public double LookbackCall(Int64 N, double T, double mu, double sigma, double s0, double k,
        double r, double[,] price)
    {
        var sum = 0.0;
        for (int i = 0; i < N; i++)
        {
            var pricepath = new double [] {price[i, (int) (T * 252)]};
            var payoff = pricepath.Max() - k;
            if (payoff > 0) ;
            {
                sum += payoff;
            }
        }
        var optionprice = (sum/N) * Math.Exp(-r * T);

        return optionprice;

    }
    
    public double LookbackPut(Int64 N, double T, double mu, double sigma, double s0, double k,
        double r, double[,] price)
    {
        var sum = 0.0;
        for (int i = 0; i < N; i++)
        {
            var pricepath = new double [] {price[i, (int) (T * 252)]};
            var payoff = k - pricepath.Min();
            if (payoff > 0) ;
            {
                sum += payoff;
            }
        }
        var optionprice = (sum/N) * Math.Exp(-r * T);

        return optionprice;

    }
    
    public double RangeOption(Int64 N, double T, double mu, double sigma, double s0, double k,
        double r, double[,] price)
    {
        var sum = 0.0;
        for (int i = 0; i < N; i++)
        {
            var pricepath = new double [] {price[i, (int) (T * 252)]};
            var payoff = pricepath.Max() - pricepath.Min();
            if (payoff > 0) ;
            {
                sum += payoff;
            }
        }
        var optionprice = (sum/N) * Math.Exp(-r * T);

        return optionprice;

    }
    
    public Dictionary<string, double> CallGreeks(Int64 N, double T, double mu, double sigma, double s0, double k,
        double r, string antithetic)
    {
        var deltaS = s0 / 10; //10% change
        var deltaSigma = sigma / 10; //10% change
        //var deltaT = T / 10;
        var deltaR = r / 10;
        var price = StockSimulator.StockPath(N, T, mu, sigma, s0, antithetic, RandomNumberGenerator.Generate(N, T));
        var originalPrice = EuropeanCall(N, T, mu, sigma, s0, k, r, price)["call option price"];
        // Delta
        price = StockSimulator.StockPath(N, T, mu, sigma, s0, antithetic, RandomNumberGenerator.Generate(N, T));
        var priceplusS0 = EuropeanCall(N, T, mu, sigma, s0 + deltaS, k, r, price)["call option price"];
        price = StockSimulator.StockPath(N, T, mu, sigma, s0, antithetic, RandomNumberGenerator.Generate(N, T));
        var priceminusS0 = EuropeanCall(N, T, mu, sigma, s0 - deltaS, k, r, price)["call option price"];
        var delta = (priceplusS0 - priceminusS0) / (2 * deltaS);
        //gamma
        var gamma = (priceplusS0 - 2 * originalPrice + priceminusS0) / Math.Pow(deltaS, 2);
        // vega
        price = StockSimulator.StockPath(N, T, mu, sigma, s0, antithetic, RandomNumberGenerator.Generate(N, T));
        var priceplusSigma = EuropeanCall(N, T, mu, sigma + deltaSigma, s0, k, r, price)["call option price"];
        price = StockSimulator.StockPath(N, T, mu, sigma, s0, antithetic, RandomNumberGenerator.Generate(N, T));
        var priceminusSigma = EuropeanCall(N, T, mu, sigma - deltaSigma, s0, k, r, price)["call option price"];
        var vega = (priceplusSigma - priceminusSigma) / (2 * deltaSigma);
        // Theta
        // //price = StockSimulator.StockPath(N, T, mu, sigma, s0, antithetic, RandomNumberGenerator.Generate(N, T));
       // var priceplusT = EuropeanCall(N, T + deltaT, mu, sigma, s0, k, r, price)["call option price"];
        //var theta = (priceplusT - originalPrice) / deltaT;
        // Rho 
        price = StockSimulator.StockPath(N, T, mu, sigma, s0, antithetic, RandomNumberGenerator.Generate(N, T));
        var priceplusR = EuropeanCall(N, T, mu, sigma, s0, k, r + deltaR, price)["call option price"];
        price = StockSimulator.StockPath(N, T, mu, sigma, s0, antithetic, RandomNumberGenerator.Generate(N, T));
        var priceminusR = EuropeanCall(N, T, mu, sigma, s0, k, r - deltaR, price)["call option price"];
        var rho = (priceplusR - priceminusR) / (2 * deltaR);
        return new Dictionary<string, double>
        {
            {"delta", delta},
            {"gamma", gamma},
            {"vega", vega},
            {"rho", rho},
        };
    }

    public Dictionary<string, double> PutGreeks(long N, double T, double mu, double sigma, double s0, double k,
        double r, string antithetic)
    {
        var deltaS = s0 / 10; //10% change
        var deltaSigma = sigma / 10; //10% change
        var deltaT = T / 10;
        var deltaR = r / 10;
        var price = StockSimulator.StockPath(N, T, mu, sigma, s0, antithetic, RandomNumberGenerator.Generate(N, T));
        var originalPrice = EuropeanPut(N, T, mu, sigma, s0, k, r, price)["put option price"];
        // Delta
        price = StockSimulator.StockPath(N, T, mu, sigma, s0, antithetic, RandomNumberGenerator.Generate(N, T));
        var priceplusS0 = EuropeanPut(N, T, mu, sigma, s0 + deltaS, k, r, price)["put option price"];
        price = StockSimulator.StockPath(N, T, mu, sigma, s0, antithetic, RandomNumberGenerator.Generate(N, T));
        var priceminusS0 = EuropeanPut(N, T, mu, sigma, s0 - deltaS, k, r, price)["put option price"];
        var delta = (priceplusS0 - priceminusS0) / (2 * deltaS);
        //gamma
        var gamma = (priceplusS0 - 2 * originalPrice + priceminusS0) / Math.Pow(deltaS, 2);
        // vega
        price = StockSimulator.StockPath(N, T, mu, sigma, s0, antithetic, RandomNumberGenerator.Generate(N, T));
        var priceplusSigma = EuropeanPut(N, T, mu, sigma + deltaSigma, s0, k, r, price)["put option price"];
        price = StockSimulator.StockPath(N, T, mu, sigma, s0, antithetic, RandomNumberGenerator.Generate(N, T));
        var priceminusSigma = EuropeanPut(N, T, mu, sigma - deltaSigma, s0, k, r, price)["put option price"];
        var vega = (priceplusSigma - priceminusSigma) / (2 * deltaSigma);
        // Theta
        price = StockSimulator.StockPath(N, T, mu, sigma, s0, antithetic, RandomNumberGenerator.Generate(N, T));
        var priceplusT = EuropeanPut(N, T + deltaT, mu, sigma, s0, k, r, price)["put option price"];
        var theta = (priceplusT - originalPrice) / deltaT;
        // Rho
        price = StockSimulator.StockPath(N, T, mu, sigma, s0, antithetic, RandomNumberGenerator.Generate(N, T));
        var priceplusR = EuropeanPut(N, T, mu, sigma, s0, k, r + deltaR, price)["put option price"];
        price = StockSimulator.StockPath(N, T, mu, sigma, s0, antithetic, RandomNumberGenerator.Generate(N, T));
        var priceminusR = EuropeanPut(N, T, mu, sigma, s0, k, r - deltaR, price)["put option price"];
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