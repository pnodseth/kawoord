namespace Backend.Models;



public class CorrectLetterValue
{
    private CorrectLetterValue(string value) { Value = value; }

    public string Value { get; private set; }

    public static CorrectLetterValue Wrong   { get { return new CorrectLetterValue("Wrong"); } }
    public static CorrectLetterValue WrongPlacement   { get { return new CorrectLetterValue("WrongPlacement"); } }
    public static CorrectLetterValue Correct    { get { return new CorrectLetterValue("Correct"); } }
}

public class LetterEvaluation
{


    public string Letter { get; set; }
    public int WordIndex { get; set; }
    public CorrectLetterValue Type { get; set; }
    public int Round { get; set; }
}