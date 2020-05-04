using System;
using System.Collections.Concurrent;

namespace FryScript.VsCode.LanguageServer.Analysis
{
    public class SourceAnalyser : ISourceAnalyser
    {
        private readonly ConcurrentDictionary<Uri, object> _sources = new ConcurrentDictionary<Uri, object>();

        public bool TryOpen(Uri uri, out object? obj)
        {
            
            if(!_sources.TryGetValue(uri, out obj))
            {
                obj = new object();
                _sources.TryAdd(uri, obj);

                return true;
            }

            obj = null;
            return false;
        }

        public bool Close(Uri uri)
        {
            if(!_sources.ContainsKey(uri))
                return false;

            _sources.TryRemove(uri, out object? obj);

            return true;
        }
    }
}