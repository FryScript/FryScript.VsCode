using System;
using System.Collections.Concurrent;

namespace FryScript.VsCode.LanguageServer.Analysis
{
    public class SourceManager : ISourceManager
    {
        private readonly ConcurrentDictionary<Uri, SourceInfo> _sources = new ConcurrentDictionary<Uri, SourceInfo>();
        private readonly ISourceAnalyser _sourceAnalyser;

        public SourceManager(ISourceAnalyser sourceAnalyser) => (_sourceAnalyser) = sourceAnalyser;

        public bool Open(Uri uri, string source)
        {
            if(!_sources.ContainsKey(uri))
            {
                var sourceInfo = _sourceAnalyser.GetInfo(uri, source);

                _sources.TryAdd(uri, new SourceInfo());

                return true;
            }

            return false;
        }

        public bool Close(Uri uri)
        {
            if(!_sources.ContainsKey(uri))
                return false;

            _sources.TryRemove(uri, out SourceInfo? sourceInfo);

            return true;
        }

        public void Update(Uri uri, string source)
        {
            if(!_sources.ContainsKey(uri))
                throw new InvalidOperationException($"Uri \"{uri.AbsolutePath}\" must be open before it can be updated");

            var sourceInfo = _sourceAnalyser.GetInfo(uri, source);

            _sources.AddOrUpdate(uri, u => sourceInfo, (u, s) => sourceInfo);
        }
    }
}