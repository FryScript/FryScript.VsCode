using FryScript;
using FryScript.Compilation;
using FryScript.Parsing;
using FryScript.ScriptProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer2
{
    public interface IProtocol
    {
        void Execute(IClient connection, JToken content);
    }

    public class Protocol<T> : IProtocol
    {
        private readonly Action<RequestContext<T>> _method;

        public Protocol(Action<RequestContext<T>> method)
        {
            _method = method;
        }

        public void Execute(IClient connection, JToken request)
        {
            var @params = request.ToObject<T>();

            var context = new RequestContext<T>(connection, @params);

            _method(context);
        }
    }

    public class Protocol<TRequest, TResponse> : IProtocol
    {
        Action<TRequest, Client<TResponse>> _action;

        public Protocol(Action<TRequest, Client<TResponse>> action)
        {
            _action = action;
        }

        public void Execute(IClient client, JToken request)
        {
            var @params = request.ToObject<TRequest>();

            _action(@params, new Client<TResponse>(client));
        }
    }

    public class RequestHeaders
    {
        public int ContentLength { get; set; }
    }

    public class HeaderReader
    {
        private static readonly string[] _headerDelimiters = new[] { ": " };
        private readonly StringBuilder _sb = new StringBuilder();
        private readonly List<string> _headerTexts = new List<string>();
        private readonly TextReader _reader;

        public HeaderReader(TextReader reader)
        {
            _reader = reader;
        }

        public RequestHeaders Read()
        {
            while (true)
            {
                while (_reader.Peek() != '\r')
                {
                    _sb.Append((char)_reader.Read());
                }

                _sb.Append((char)_reader.Read());
                _sb.Append((char)_reader.Read());

                var text = _sb.ToString();
                _sb.Clear();

                if (text != "\r\n")
                {
                    _headerTexts.Add(text);
                    continue;
                }

                var headerValues = _headerTexts.Select(h => h.Split(_headerDelimiters, StringSplitOptions.RemoveEmptyEntries)).ToDictionary(k => k[0], v => v[1]);
                _headerTexts.Clear();

                var headers = new RequestHeaders();

                string contentLength;
                if (headerValues.TryGetValue("Content-Length", out contentLength))
                    headers.ContentLength = int.Parse(contentLength);

                return headers;
            }
        }
    }

    public class ContentReader
    {
        private readonly TextReader _reader;
        private char[] _buffer = new char[1024];

        public ContentReader(TextReader reader)
        {
            _reader = reader;
        }

        public string Read(int length)
        {
            if (length >= _buffer.Length)
                Array.Resize(ref _buffer, length * 2);

            _reader.ReadBlock(_buffer, 0, length);

            return new string(_buffer, 0, length);
        }
    }

    public class RequestReader
    {
        private readonly HeaderReader _headerReader;
        private readonly ContentReader _contentReader;

        public RequestReader(HeaderReader headerReader, ContentReader contentReader)
        {
            _headerReader = headerReader;
            _contentReader = contentReader;
        }

        public RequestReader()
            : this(new HeaderReader(Console.In), new ContentReader(Console.In))
        {
        }

        public string Read()
        {
            var headers = _headerReader.Read();
            var content = _contentReader.Read(headers.ContentLength);

            return content;
        }
    }

    public class RequestHandler
    {
        private readonly RequestReader _reader;

        public RequestHandler(RequestReader reader)
        {
            _reader = reader;
        }

        public RequestHandler()
            : this(new RequestReader())
        {
        }

        public void Handle(ResponseHandler responseHandler, ProtocolMethods protocolMethods)
        {
            var json = _reader.Read();

            var obj = JsonConvert.DeserializeObject<JObject>(json);

            var id = obj.Value<int>("id");
            var methodName = obj.Value<string>("method");

            var protocol = protocolMethods.GetMethod(methodName);

            var connection = new Client(id, responseHandler);

            protocol.Execute(connection, obj["params"]);
        }
    }

    public class ProtocolMethods
    {
        private ScriptEngine _se;
        private readonly Dictionary<string, IProtocol> _protocols = new Dictionary<string, IProtocol>();

        private class ProtocolMethodAttribute : Attribute
        {
            private readonly string _name;

            public string Name => _name;

            public ProtocolMethodAttribute(string name)
            {
                _name = name;
            }
        }

        public ProtocolMethods()
        {
            _protocols["initialize"] = new Protocol<InitializeParams, object>(new Action<InitializeParams, Client<object>>(Initialize));
            _protocols["textDocument/didChange"] = new Protocol<DidChangeTextDocumentParams, object>(new Action<DidChangeTextDocumentParams, Client<object>>(TextDocumentDidChange));
            _protocols["textDocument/completion"] = new Protocol<CompletionParams, CompletionItem[]>(new Action<CompletionParams, Client<CompletionItem[]>>(Completion));
        }

        public class ProtocolVoid
        {
        }

        public virtual void Initialize(InitializeParams @params, Client<object> client)
        {
            _se = new ScriptEngine(new ScriptCompiler(new ScriptParser()), ".fry", new List<IScriptProvider> {
                new DirectoryScriptProvider(@params.RootPath)
            });

            client.Response(new
            {
                capabilities = new
                {
                    textDocumentSync = 1,
                    completionProvider = new
                    {
                        resolveProvider = true,
                        triggerCharacters = new[]
                        {
                            "."
                        }
                    }
                }
            });
        }

        public virtual void TextDocumentDidChange(DidChangeTextDocumentParams @params, Client<object> client)
        {
            var script = @params.ContentChanges.First().Text;

            if (script != null)
            {
                try
                {
                    _se.Eval(@params.TextDocument.Uri, script);
                    client.Diagnostics(new PublishDiagnosticsParams
                    {
                        Uri = @params.TextDocument.Uri,
                        Diagnostics = new Diagnostic[0]
                    });
                }
                catch (FryScriptException ex)
                {
                    client.Diagnostics(new PublishDiagnosticsParams
                    {
                        Uri = @params.TextDocument.Uri,
                        Diagnostics = new[]
                        {
                            new Diagnostic
                            {
                                Severity = DiagnosticSeverity.Error,
                                Source = "Fry Script",
                                Message = ex.Message,
                                Range = new Range
                                {
                                    Start = new Position
                                    {
                                        Line = ex.Line ?? 0,
                                        Character = ex.Column ?? 0
                                    },
                                    End = new Position
                                    {
                                        Line = ex.Line ?? 0,
                                        Character = ex.Column + 1 ?? 0
                                    },
                                }
                            }
                        }
                    });
                }
                catch(Exception ex)
                {
                    client.ShowMessage("Super fatal error!");
                }
                client.Response(null);
            }
        }

        public virtual void Completion(CompletionParams @params, Client<CompletionItem[]> client)
        {
            client.Response(new[]
            {
                new CompletionItem
                {
                    Label = "Mr"
                },
                new CompletionItem
                {
                    Label = "McGoo"
                }
            });
        }

        public virtual void Initialize(ProtocolVoid @params, Client<ProtocolVoid> client)
        {
            client.ShowMessage("Server initialized correctly");
        }

        [ProtocolMethod("initialized")]
        public virtual void Initialized(RequestContext<object> context)
        {
            context.Connection.ShowMessage("Server was initialized!");
        }

        [ProtocolMethod("shutdown")]
        public virtual void Shutdown(RequestContext<object> context)
        {
            context.Connection.Response(null);
        }

        [ProtocolMethod("exit")]
        public virtual void Exit(RequestContext<object> context)
        {
            context.Connection.ShowMessage("The language server is shutting down!");

            Environment.Exit(0);
        }

        [ProtocolMethod("textDocument/didChange")]
        public virtual void TextDocumentDidChange(RequestContext<DidChangeTextDocumentParams> context)
        {
            context.Connection.ShowMessage("Change!");
        }

        public IProtocol GetMethod(string name)
        {
            IProtocol protocol;
            if (!_protocols.TryGetValue(name, out protocol))
            {
                var methodInfo = GetType().GetTypeInfo().DeclaredMethods.FirstOrDefault(m => m.GetCustomAttribute<ProtocolMethodAttribute>()?.Name == name);

                var protocolType = methodInfo?.GetParameters().SingleOrDefault()?.ParameterType.GetTypeInfo().GetGenericArguments()?.FirstOrDefault();

                if (protocolType == null)
                {
                    return _protocols[name] = new StubProtocol();
                }

                var requestType = typeof(RequestContext<>).MakeGenericType(protocolType);
                var actionType = typeof(Action<>).MakeGenericType(requestType);
                var del = Delegate.CreateDelegate(actionType, this, methodInfo);

                _protocols[name] = protocol = Activator.CreateInstance(typeof(Protocol<>).MakeGenericType(protocolType), del) as IProtocol;
            }

            return protocol;
        }
    }

    public class StubProtocol : IProtocol
    {
        public void Execute(IClient connection, JToken content)
        {
        }
    }

    public class InitializeParams
    {
        public string RootPath;
    }

    public class CompletionParams
    {
        public CompletionContext context;
    }

    public class CompletionContext
    {
        public CompletionTriggerKind TriggerKind;

        public string TriggerCharacter;
    }

    public enum CompletionTriggerKind
    {
        Invoked = 1,
        TriggerCharacter = 2,
        TriggerForIncompleteCompletions = 3
    }

    public class CompletionItem
    {
        public string Label;

        public int Kind;

        public string Detail;

        public string Documentation;

        public bool Deprecated;

        public bool Preselect;

        public string SortText;

        public string FilterText;

        public string InsertText;

        public InsertTextFormat InsertTextFormat;

        public TextEdit TextEdit;

        public TextEdit[] AdditionalTextEdits;

        public string[] CommitCharacters;

        public ClientCommand Command;

        public object Data;
    }

    public class ClientCommand
    {
        public string Title;

        public string Command;

        public object[] Arguments;
    }

    public enum InsertTextFormat
    {
        PlainText = 1,
        Snippet = 2
    }

    public class TextEdit
    {
        public Range Range;

        public string NewText;
    }

    public class DidChangeTextDocumentParams
    {
        public VersionedTextDocumentIdentifier TextDocument { get; set; }

        public TextDocumentContentChangeEvent[] ContentChanges { get; set; }
    }

    public class TextDocumentContentChangeEvent
    {
        public Range Range { get; set; }

        public int RangeLength { get; set; }

        public string Text { get; set; }
    }

    public class TextDocumentIdentifier
    {
        public string Uri { get; set; }
    }

    public class Range
    {
        public Position Start { get; set; }

        public Position End { get; set; }
    }

    public class Position
    {
        public int Line { get; set; }

        public int Character { get; set; }
    }

    public class VersionedTextDocumentIdentifier : TextDocumentIdentifier
    {
        public int Version { get; set; }
    }

    public class Diagnostic
    {
        public Range Range;

        public DiagnosticSeverity Severity;

        public int Code;

        public string Source;

        public string Message;

        DiagnosticRelatedInformation[] RelatedInformation;
    }

    public enum DiagnosticSeverity
    {
        Error = 1,
        Warning = 2,
        Information = 3,
        Hint = 4
    }

    public class DiagnosticRelatedInformation
    {
        public Location Location;

        public string Message;
    }

    public class PublishDiagnosticsParams
    {
        public string Uri;

        public Diagnostic[] Diagnostics;
    }

    public class Location
    {
        public string Uri;

        public Range Range;
    }

    public interface IClient
    {
        void Response(object result);

        void Diagnostics(PublishDiagnosticsParams @params);

        void ShowMessage(string message);
    }

    public class Client<T> : IClient
    {
        private IClient _client;

        public Client(IClient client)
        {
            _client = client;
        }

        public void Diagnostics(PublishDiagnosticsParams diagnostics)
        {
            _client.Diagnostics(diagnostics);
        }

        public void Response(T result)
        {
            _client.Response(result);
        }

        void IClient.Response(object result)
        {
            throw new NotImplementedException();
        }

        public void ShowMessage(string message)
        {
            _client.ShowMessage(message);
        }
    }


    public class Client : IClient
    {
        private readonly int _id;
        private readonly ResponseHandler _responseHandler;

        public Client(int id, ResponseHandler responseHandler)
        {
            _id = id;
            _responseHandler = responseHandler;
        }

        public void Diagnostics(PublishDiagnosticsParams diagnostics)
        {
            var response = new
            {
                method = "textDocument/publishDiagnostics",
                @params = diagnostics
            };

            _responseHandler.Push(response);
        }

        public void Response(object result)
        {
            var response = new ResponseMessage(_id, result);

            _responseHandler.Push(response);
        }

        public void ShowMessage(string message)
        {
            var response = new
            {
                method = "window/showMessage",
                @params = new
                {
                    type = 3,
                    message = message
                }
            };

            _responseHandler.Push(response);
        }
    }

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

    public interface IRequestContext
    {
        IClient Connection { get; }

        object Params { get; }
    }

    public class RequestContext<T> : IRequestContext
    {
        private readonly IClient _connection;
        private readonly T _params;


        public IClient Connection => _connection;

        public T Params => _params;

        IClient IRequestContext.Connection => Connection;

        object IRequestContext.Params => Params;

        public RequestContext(IClient connection, T @params)
        {
            _connection = connection;
            _params = @params;
        }
    }

    public class ResponseHandler
    {
        private readonly ResponseWriter _writer;
        private readonly ConcurrentQueue<object> _responses = new ConcurrentQueue<object>();

        public ResponseHandler(ResponseWriter writer)
        {
            _writer = writer;
        }

        public ResponseHandler()
            : this(new ResponseWriter(Console.Out))
        {
        }

        public void Handle()
        {
            object response;

            if (!_responses.TryDequeue(out response))
                return;

            _writer.Write(response);
        }

        public void Push(object response)
        {
            _responses.Enqueue(response);
        }
    }

    public class ResponseWriter
    {
        private readonly TextWriter _writer;

        public ResponseWriter(TextWriter writer)
        {
            _writer = writer;
        }

        public void Write(object response)
        {
            var sb = new StringBuilder();

            var json = JsonConvert.SerializeObject(response, Formatting.None, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            sb.Append($"Content-Length: {json.Length}\r\n");
            sb.Append("\r\n");
            sb.Append(json);

            _writer.Write(sb.ToString());
        }
    }

    public class LanguageServer
    {
        private readonly RequestHandler _requestHandler;
        private readonly ResponseHandler _responseHandler;
        private readonly ProtocolMethods _protocolMethods;

        public LanguageServer(RequestHandler requestHandler, ResponseHandler responseHandler, ProtocolMethods protocolMethods)
        {
            _requestHandler = requestHandler;
            _responseHandler = responseHandler;
            _protocolMethods = protocolMethods;
        }

        public LanguageServer()
            : this(new RequestHandler(), new ResponseHandler(), new ProtocolMethods())
        {
        }

        public LanguageServer(ProtocolMethods protocolMethods)
            : this(new RequestHandler(), new ResponseHandler(), protocolMethods)
        {
        }

        public void Start()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    _responseHandler.Handle();
                }
            });

            while (true)
            {
                _requestHandler.Handle(_responseHandler, _protocolMethods);
            }
        }
    }


    class Program
    {
        // private static List<string> _requestLogs = new List<string>();
        //private static readonly MethodResolver _methodResolver = new MethodResolver(new InitializeMethodHandler(), new ShutdownHandler(), new ExitHandler());
        static void Main(string[] args)
        {
            //Console.WriteLine("Init");
            //var headerReader = new HeaderReader(Console.In);
            //var conentReader = new ContentReader(Console.In);
            //var requestReader = new RequestReader();
            //var requestHandler = new RequestHandler();
            var languageServer = new LanguageServer();

            using (var reader = new StreamReader(Console.OpenStandardInput()))
            {
                while (true)
                {
                    //DebugHook.Hook();

                    languageServer.Start();

                    //requestHandler.Handle();

                    //var headers = new List<string>();

                    //string header;
                    //while ((header = ReadHeader(reader)) != null)
                    //{
                    //    headers.Add(header);
                    //}

                    //var keyedHeaders = headers.Select(h => h.Split(new[] { ": " }, StringSplitOptions.RemoveEmptyEntries)).ToDictionary(k => k[0], v => v[1]);
                    //var contentLength = int.Parse(keyedHeaders["Content-Length"]);

                    //var content = ReadContent(reader, contentLength);

                    //_requestLogs.Add(content);

                    //var headers = headerReader.Read();

                    //var content = conentReader.Read(headers.ContentLength);
                    //var content = ReadContent(Console.In, headers.ContentLength);

                    //#########################################
                    //var requestJson = requestReader.Read();

                    //dynamic request = JsonConvert.DeserializeObject<JObject>(requestJson);


                    //var response = _methodResolver.Resolve(request) as HandlerResult;

                    //if (response.result == MethodHandler.Void)
                    //    continue;

                    //response.id = (int)request?.id?.Value;


                    //WriteResponse(response);


                }
            }
        }

        private static string ReadHeader(StreamReader reader)
        {
            var chars = new List<char>();

            while (reader.Peek() != '\r')
            {
                chars.Add((char)reader.Read());
            }

            // Read trailing \r\n
            if (reader.Peek() == '\r')
            {
                chars.Add((char)reader.Read());
                chars.Add((char)reader.Read());
            }

            var str = new string(chars.ToArray());

            if (str == "\r\n")
                return null;

            return str;
        }

        private static string ReadContent(TextReader reader, int length)
        {
            var buffer = new char[length];
            reader.Read(buffer, 0, length);

            return new string(buffer);
        }

        //private static object WrapResponse(int requestId, object response)
        //{
        //    return new
        //    {
        //        id = requestId,
        //        result = response
        //    };
        //}

        private static void WriteResponse(object response)
        {
            var json = JsonConvert.SerializeObject(response);

            var sb = new StringBuilder();
            sb.AppendLine($"Content-Length: {json.Length}");
            sb.AppendLine();
            sb.Append(json);

            Console.Out.Write(sb.ToString());
        }
    }
}
