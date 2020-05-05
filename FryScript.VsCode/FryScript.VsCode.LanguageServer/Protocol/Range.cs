namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class Range
    {
        public Position Start { get; set; } = new Position();

        public Position End { get; set; } = new Position();
    }
}