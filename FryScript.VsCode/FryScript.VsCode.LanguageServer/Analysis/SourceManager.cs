using System;
using System.Collections.Concurrent;
using FryScript.Ast;

namespace FryScript.VsCode.LanguageServer.Analysis
{
    public class SourceManager : ISourceManager
    {
        private readonly ConcurrentDictionary<Uri, ISourceInfo> _sources = new ConcurrentDictionary<Uri, ISourceInfo>();
        private readonly ISourceAnalyser _sourceAnalyser;

        public SourceManager(ISourceAnalyser sourceAnalyser) => (_sourceAnalyser) = sourceAnalyser;

        public bool Open(Uri uri, string source)
        {
            if(!_sources.ContainsKey(uri))
            {
                var sourceInfo = _sourceAnalyser.GetInfo(uri, source);

                _sources.TryAdd(uri, new SourceInfo(uri, new ScriptNode()));

                return true;
            }

            return false;
        }

        public bool Close(Uri uri)
        {
            if(!_sources.ContainsKey(uri))
                return false;

            _sources.TryRemove(uri, out ISourceInfo? sourceInfo);

            return true;
        }

        public ISourceInfo Update(Uri uri, string source)
        {
            if(!_sources.ContainsKey(uri))
                throw new InvalidOperationException($"Uri \"{uri.AbsolutePath}\" must be open before it can be updated");

            var sourceInfo = _sourceAnalyser.GetInfo(uri, source);

            return _sources.AddOrUpdate(uri, u => sourceInfo, (u, s) => AddOrUpdate(s, sourceInfo));
        }

        private ISourceInfo AddOrUpdate(ISourceInfo originalSourceInfo, ISourceInfo newSourceInfo)
        {
            originalSourceInfo.Diagnostics.Clear();
            originalSourceInfo.Diagnostics.AddRange(newSourceInfo.Diagnostics);

            originalSourceInfo.Fragments.Clear();
            originalSourceInfo.Fragments.AddRange(newSourceInfo.Fragments);

            return originalSourceInfo;
        }
    }
}