namespace Backend.Models;

public class CorrectPlayerLetter
{
    public CorrectPlayerLetter(string letter, int solutionWordIndex, CorrectLetterEnum type, int round)
    {
        Letter = letter;
        SolutionWordIndex = solutionWordIndex;
        Type = type;
        Round = round;
    }

    public string Letter { get; }
    public int SolutionWordIndex { get; }
    public CorrectLetterEnum Type { get; }
    public int Round { get; }
}