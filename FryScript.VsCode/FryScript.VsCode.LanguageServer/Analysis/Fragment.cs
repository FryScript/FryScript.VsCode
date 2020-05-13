namespace FryScript.VsCode.LanguageServer.Analysis
{
    public class Fragment
    {
        public int Line { get; }

        public int Column { get; }

        public string Type { get; }

        public string Value {get;}

        public Fragment(string type, string value, int line, int column)
            => (Type, Value, Line, Column) = (type, value, line, column);
    }
}