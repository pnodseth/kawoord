namespace Backend.Models;

public class CorrectLetterValue
{
    private CorrectLetterValue(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static CorrectLetterValue Wrong => new CorrectLetterValue("Wrong");
    public static CorrectLetterValue WrongPlacement => new CorrectLetterValue("WrongPlacement");
    public static CorrectLetterValue Correct => new CorrectLetterValue("Correct");
}

public class LetterEvaluation
{
    public string Letter { get; set; }
    public int WordIndex { get; set; }
    public CorrectLetterValue LetterValueType { get; set; }
    public int Round { get; set; }
}