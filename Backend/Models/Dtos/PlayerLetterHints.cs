namespace Backend.Models.Dtos;

public record SolutionLetterRecord(char Letter, int Index);

public class PlayerLetterHints
{
    private readonly Game _game;
    private readonly Player _player;
    public List<LetterEvaluation> Correct { get; set; } = new();
    public List<LetterEvaluation> WrongPosition { get; set; } = new();
    public int RoundNumber { get; set; }

    public PlayerLetterHints(Game game, Player player)
    {
        _game = game;
        _player = player;
        RoundNumber = game.CurrentRoundNumber;
    }


    public void CalculatePlayerLetterHints()
    {
        var playerRoundSubmissions = _game.RoundSubmissions.Where(e => e.Player.Id == _player.Id).ToList();
        var allLetterEvaluations = new List<LetterEvaluation>();


        var solutionLetterRecords = _game.Solution.Select((letter, idx) => new SolutionLetterRecord(letter, idx)).ToList();
        // Add all evaluations to allLetterEvaluations List
        playerRoundSubmissions.ForEach(submission =>
        {
            submission.LetterEvaluations.ForEach(e => allLetterEvaluations.Add(e));
        });

        // add correct letters 
        allLetterEvaluations.ForEach(evaluation =>
        {
            var letterIsCorrect = solutionLetterRecords.FirstOrDefault((letter) =>
                letter.Letter.ToString().Equals(evaluation.Letter) && letter.Index == evaluation.WordIndex);
            if (letterIsCorrect is null) return;

            Correct.Add(evaluation);
            solutionLetterRecords.Remove(letterIsCorrect);
        });

        // add wrongly positioned letters
        allLetterEvaluations.ForEach(evaluation =>
        {
            var letterIsWronglyPositioned =
                solutionLetterRecords.FirstOrDefault(letter => letter.Letter.ToString().Equals(evaluation.Letter));

            if (letterIsWronglyPositioned is null) return;
            WrongPosition.Add(evaluation);
            solutionLetterRecords.Remove(letterIsWronglyPositioned);
        });
    }
}