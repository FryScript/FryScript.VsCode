namespace LanguageServer2
{
    public class ResponseMessage
    {
        private readonly int _id;
        private readonly object _result;

        public int Id => _id;

        public object Result => _result;

        public ResponseMessage(int id, object result)
        {
            _id = id;
            _result = result;
        }
    }
}
