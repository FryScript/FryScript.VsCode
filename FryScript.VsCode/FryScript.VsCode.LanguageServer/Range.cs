namespace FryScript.VsCode.LanguageServer
{
    public class Range
    {
        public Position Start { get; set; } = new Position();

        public Position End { get; set; } = new Position();
    }
}