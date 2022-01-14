namespace Backend;

public class RoundSubmission
{
    public RoundSubmission(Player player, int round, string submittedWord, DateTime submittedTime)
    {
        Player = player;
        Round = round;
        SubmittedWord = submittedWord;
        SubmittedTime = submittedTime;
    }

    public Player Player { get; }
    public int Round { get; }
    public string SubmittedWord { get; }
    public DateTime SubmittedTime { get; }

    public int CalculateScore()
    {
        return 10;
    }

    public bool IsCorrectWord()
    {
        return false;
    }
}