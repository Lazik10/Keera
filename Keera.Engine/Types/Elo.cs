using Keera.Engine.Game;

namespace Keera.Engine.Types;

public class Elo
{
    private const double c = 42.4264;
    private const double q = 0.0058565;
    private const double t = 1;

    public double ScoredPoints { get; set; }

    public double EloRating { get; set; }
    public double ExpectedEloScore { get; set; }

    private double GlickoRating_1 { get; set; }
    private double ExpectedScoreGlicko_1 { get; set; }

    private double RatingDeviation;

    public Elo()
    {
        ScoredPoints = 0;

        EloRating = 1500;
        ExpectedEloScore = 0;

        GlickoRating_1 = 1500;
        ExpectedScoreGlicko_1 = 0;
        RatingDeviation = Math.Sqrt(350 * 350 + (c * c * t));
}

    private static int K_Value(Player player)
    {
        return player.TotalGamesPlayed < 30 || (player.Rating.EloRating < 2300 && player.Age < 18) ? 40 : player.Rating.EloRating < 2400 ? 20 : 10;
    }

    private static double G_Value(Player opponent)
    {
        return 1 / Math.Sqrt(1 + (3 * q * q * opponent.Rating.RatingDeviation * opponent.Rating.RatingDeviation / (Math.PI * Math.PI)));
    }

    public static void Update(Player winner, float winnerResult, Player loser, float loserResult)
    {
        UpdateExpectedScore(winner, loser); // Probably update when game starts instead of when game ends - possible disconnections and so on
        SetScorePoints(winner, winnerResult, loser, loserResult);
        ComputeNewRating(winner, loser);
    }

    private static void UpdateExpectedScore(Player winner, Player loser)
    {
        if (winner == null || loser == null)
            return;

        winner.Rating.ExpectedEloScore = 1 / (1 + Math.Pow(10, (loser.Rating.EloRating - winner.Rating.EloRating) / 400));
        loser.Rating.ExpectedEloScore = 1 / (1 + Math.Pow(10, (winner.Rating.EloRating - loser.Rating.EloRating) / 400));

        winner.Rating.ExpectedScoreGlicko_1 = 1 / (1 + Math.Pow(10, -G_Value(loser) * (winner.Rating.EloRating - loser.Rating.EloRating) / 400));
        loser.Rating.ExpectedScoreGlicko_1 = 1 / (1 + Math.Pow(10, -G_Value(winner) * (loser.Rating.EloRating - winner.Rating.EloRating) / 400));
    }

    private static void SetScorePoints(Player winner, float winnerResult, Player loser, float loserResult)
    { 
        winner.Rating.ScoredPoints = winnerResult;
        loser.Rating.ScoredPoints = loserResult;
    }

    private static void ComputeNewRating(Player winner, Player loser)
    {
        winner.Rating.EloRating += K_Value(winner) * (winner.Rating.ScoredPoints - winner.Rating.ExpectedEloScore);
        loser.Rating.EloRating += K_Value(loser) * (loser.Rating.ScoredPoints - loser.Rating.ExpectedEloScore);

        double winnerDValue = 1 / (q * q * G_Value(loser) * G_Value(loser) * winner.Rating.ExpectedScoreGlicko_1 * (1 - winner.Rating.ExpectedScoreGlicko_1));
        double winnerRatingDeviationNew = 1 / Math.Sqrt(1 / (winner.Rating.RatingDeviation * winner.Rating.RatingDeviation) + 1 / (winnerDValue * winnerDValue));

        double loserDValue = 1 / (q * q * G_Value(winner) * G_Value(winner) * loser.Rating.ExpectedScoreGlicko_1 * (1 - loser.Rating.ExpectedScoreGlicko_1));
        double loserRatingDeviationNew = 1 / Math.Sqrt(1 / (loser.Rating.RatingDeviation * loser.Rating.RatingDeviation) + 1 / (loserDValue * loserDValue));

        winner.Rating.GlickoRating_1 += (q * winnerRatingDeviationNew * winnerRatingDeviationNew * G_Value(loser) * (winner.Rating.ScoredPoints - winner.Rating.ExpectedScoreGlicko_1));
        loser.Rating.GlickoRating_1 += (q * loserRatingDeviationNew * loserRatingDeviationNew * G_Value(winner) * (loser.Rating.ScoredPoints - loser.Rating.ExpectedScoreGlicko_1));

        Console.WriteLine($"{winner.Name}'s new Glicko rating is: {winner.Rating.GlickoRating_1} and Elo rating: {winner.Rating.EloRating}");
        Console.WriteLine($"{loser.Name}'s new Glicko rating is: {loser.Rating.GlickoRating_1} and Elo rating: {loser.Rating.EloRating}");

        // Update RatingDeviation value to new one for future game
        winner.Rating.RatingDeviation = winnerRatingDeviationNew;
        loser.Rating.RatingDeviation = loserRatingDeviationNew;
}
}
