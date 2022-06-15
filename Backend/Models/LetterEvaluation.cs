namespace Backend.Models;

public class LetterValueType
{
    private LetterValueType(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static LetterValueType Wrong => new LetterValueType("Wrong");
    public static LetterValueType WrongPlacement => new LetterValueType("WrongPlacement");
    public static LetterValueType Correct => new LetterValueType("Correct");
}

public class LetterEvaluation
{
    public string Letter { get; set; }
    public int WordIndex { get; set; }
    public LetterValueType LetterValueType { get; set; }
    public int Round { get; set; }

    public LetterEvaluation(string letter, LetterValueType letterValueType, int wordIndex, int round)
    {
        Letter = letter;
        LetterValueType = letterValueType;
        WordIndex = wordIndex;
        Round = round;
    }
}