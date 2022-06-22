namespace Backend.GameService.Models;

public class LetterValueType
{
    private LetterValueType(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static LetterValueType Wrong => new("Wrong");
    public static LetterValueType WrongPlacement => new("WrongPlacement");
    public static LetterValueType Correct => new("Correct");
}

public class LetterEvaluation
{
    public LetterEvaluation(string letter, LetterValueType letterValueType, int wordIndex, int round)
    {
        Letter = letter;
        LetterValueType = letterValueType;
        WordIndex = wordIndex;
        Round = round;
    }

    public string Letter { get; set; }
    public int WordIndex { get; set; }
    public LetterValueType LetterValueType { get; set; }
    public int Round { get; set; }
}