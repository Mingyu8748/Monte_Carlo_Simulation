namespace Monte_Carlo_Simulation;

class StockSimulator
{
    public static double[,] StockPath(Int64 N, double T, double mu, double sigma, double s0, string VarianceReduction,
        double[,] inputRandomNumber)
    {
        var dailymu = mu / 252;
        var dailysigma = sigma / (Math.Pow(252, 0.5));
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
            while (rowpart1 <= (int) N / 2)
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

            var rowpart2 = (int) N / 2;
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

    public static void StockPath(object? obj)
    {
        var o = (Param) obj!;
        StockPath(o.n, o.t, o.mu, o.sigma, o.s0, o.varianceReduction, RandomNumberGenerator.Generate(o.n, o.t));
    }
}

class Param
{
    public Int64 n { get; set; }
    public double t { get; set; }
    public double mu { get; set; }
    public double sigma { get; set; }
    public double s0 { get; set; }
    public string varianceReduction { get; set; }

    public Param(long n, double t, double mu, double sigma, double s0, string varianceReduction)
    {
        this.n = n;
        this.t = t;
        this.mu = mu;
        this.sigma = sigma;
        this.s0 = s0;
        this.varianceReduction = varianceReduction;
    }
}