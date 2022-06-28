using Backend.Shared.Models;

namespace Backend.GameService.Models.Dto;

public record SolutionLetterRecord(char Letter, int Index);

public class PlayerLetterHints
{
    private readonly Game _game;
    private readonly IPlayer _player;

    public PlayerLetterHints(Game game, IPlayer player)
    {
        _game = game;
        _player = player;
        RoundNumber = game.CurrentRoundNumber;
    }

    public List<LetterEvaluation> Correct { get; } = new();
    public List<LetterEvaluation> WrongPosition { get; } = new();
    public List<LetterEvaluation> Wrong { get; private set; } = new();
    public int RoundNumber { get; }


    public void CalculatePlayerLetterHints()
    {
        if (_game.Solution is null) throw new NullReferenceException();

        var playerRoundSubmissions = _game.RoundSubmissions.Where(e => e.Player.Id == _player.Id).ToList();
        var allLetterEvaluations = new List<LetterEvaluation>();


        var solutionLetterRecords =
            _game.Solution.Select((letter, idx) => new SolutionLetterRecord(letter, idx)).ToList();
        // Add all evaluations to allLetterEvaluations List
        playerRoundSubmissions.ForEach(submission =>
        {
            submission.LetterEvaluations.ForEach(e => allLetterEvaluations.Add(e));
        });

        // add correct letters 
        allLetterEvaluations.ToList().ForEach(evaluation =>
        {
            var letterIsCorrect = solutionLetterRecords.FirstOrDefault(letter =>
                letter.Letter.ToString().Equals(evaluation.Letter) && letter.Index == evaluation.WordIndex);
            if (letterIsCorrect is null) return;

            Correct.Add(evaluation);
            solutionLetterRecords.Remove(letterIsCorrect);
            allLetterEvaluations.Remove(evaluation);
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

        // Add wrongly guessed letters from all rounds
        Wrong = allLetterEvaluations.Where(e => e.LetterValueType.Value == LetterValueType.Wrong.Value)
            .DistinctBy(t => t.Letter).ToList();
    }
}