namespace OldCodeWriter;

public class BaseModule<TSelf>(CodeWriter writer)
   where TSelf : BaseModule<TSelf>
{
   public TSelf OpenBody()
   {
      writer.WriteLine($"{{");
      writer.UpIndent();
      return (TSelf)this;
   }

   public TSelf CloseBody()
   {
      writer.DownIndent();
      writer.WriteLine($"}}");
      return (TSelf)this;
   }
   
   public TSelf UpIndent()
   {
      writer.UpIndent();
      return (TSelf)this;
   }
   
   public TSelf DownIndent()
   {
      writer.DownIndent();
      return (TSelf)this;
   }
   
   public TSelf WriteLine(string line)
   {
      writer.WriteLine(line, false);
      return (TSelf)this;
   }

   public TSelf WriteLine()
   {
      writer.WriteLine();
      return (TSelf)this;
   }
   
   public TSelf Write(string line)
   {
      writer.Write(line, false);
      return (TSelf)this;
   }
}

public sealed class ClassModule(CodeWriter writer) 
   : BaseModule<ClassModule>(writer)
{
   private readonly CodeWriter _writer = writer;

   public ClassModule FirstBaseDeclaration(string baseClassName, bool close = false)
   {
      _writer.UpIndent();
      _writer.Write($" : {baseClassName}");

      return close 
         ? CloseBaseDeclaration() 
         : this;
   }

   public ClassModule NextBaseDeclaration(string baseClassName, bool close = false)
   {
      _writer.WriteLine(",");
      _writer.Write($"  {baseClassName}");
      
      return close 
         ? CloseBaseDeclaration() 
         : this;
   }
   
   public ClassModule CloseBaseDeclaration()
   {
      _writer.WriteLine();
      _writer.DownIndent();
      return this;
   }

   public CodeWriter Done()
   {
      _writer.CancellationToken.ThrowIfCancellationRequested();
      return _writer;
   }
}

public sealed class MethodModule(CodeWriter writer) 
   : BaseModule<MethodModule>(writer)
{
   private readonly CodeWriter _writer = writer;

   public MethodModule OpenHeader(
      string modifiers, string returnType, string name)
   {
      _writer.WriteLine($"{modifiers} {returnType} {name}(");
      return this;
   }

   public MethodModule AddFirstParameter(string type, string name)
   {
      _writer.UpIndent();
      _writer.Write($"{type} {name}");
      return this;
   }

   public MethodModule AddNextParameter(string type, string name)
   {
      _writer.WriteLine(",");
      _writer.Write($"{type} {name}");
      return this;
   }

   public MethodModule CloseHeader(bool semicolon = false)
   {
      _writer.WriteLine($"){(semicolon ? ";" : string.Empty)}");
      if (!semicolon)
      {
         _writer.DownIndent();
      }
      return this;
   }

   public CodeWriter Done()
   {
      _writer.CancellationToken.ThrowIfCancellationRequested();
      return _writer;
   }
}

public sealed class NameSpaceModule(CodeWriter writer)
   : BaseModule<NameSpaceModule>(writer)
{
   private readonly CodeWriter _writer = writer;

   public NameSpaceModule EnableNullable(bool extraLine = true)
   {
      _writer.WriteLine("#nullable enable");
      _writer.WriteLineIf(extraLine);
      return this;
   }
   
   public NameSpaceModule Using(string nameSpace)
   {
      _writer.WriteLine($"using {nameSpace};");
      return this;
   }

   public NameSpaceModule Set(string nameSpace, bool extraLine = true)
   {
      _writer.WriteLine($"namespace {nameSpace};");
      _writer.WriteLineIf(extraLine);
      return this;
   }

   public NameSpaceModule ExtraLine()
   {
      _writer.WriteLine();
      return this;
   }

   public CodeWriter Done()
   {
      _writer.CancellationToken.ThrowIfCancellationRequested();
      return _writer;
   }
}

public sealed class CodeWriter : IDisposable
{
   private const string DefaultIndent = "\t";
   private const string DefaultNewLine = "\n";
   
   public CancellationToken CancellationToken { get; }
   
   public NameSpaceModule NameSpace { get; }
   public ClassModule Class { get; }
   public MethodModule Method { get; }

   private string Indent { get; set; } = DefaultIndent;
   private string NewLine { get; set; } = DefaultNewLine;

   private string[] _levelCache;

   public ReadOnlySpan<char> WrittenSpan => Builder.Span;
   
   private ArrayBuilder<char> Builder { get; set; } = new ArrayBuilder<char>();
   private int CurrentLevel { get; set; } = 0;
   private string CurrentLevelString { get; set; } = "";

   public CodeWriter(CancellationToken token = default)
   {
      CancellationToken = token;
      
      _levelCache = new string[6];
      _levelCache[0] = "";

      for (var e = 1; e < _levelCache.Length; e++)
      {
         _levelCache[e] = _levelCache[e - 1] + Indent;
      }

      NameSpace = new NameSpaceModule(this);
      Class = new ClassModule(this);
      Method = new MethodModule(this);
   }

   public void UpIndent()
   {
      CurrentLevel++;
      if (CurrentLevel == _levelCache.Length)
      {
         Array.Resize(ref _levelCache, _levelCache.Length * 2);
      }

      CurrentLevelString = _levelCache[CurrentLevel]
         ??= _levelCache[CurrentLevel - 1] + Indent;
   }

   public void DownIndent()
   {
      CurrentLevel--;
      CurrentLevelString = _levelCache[CurrentLevel];
   }

   public Span<char> Advance(int size)
   {
      AddIndentOnDemand();
      return Builder.Advance(size);
   }

   public void WriteText(string text)
      => WriteText(text.AsSpan());

   public void WriteText(ReadOnlySpan<char> text)
   {
      AddIndentOnDemand();
      Builder.AddRange(text);
   }

   public void Write(string text, bool multiLine = false)
      => Write(text.AsSpan(), multiLine);

   public void Write(ReadOnlySpan<char> content, bool multiLine = false)
   {
      if (!multiLine)
      {
         WriteText(content);
      }
      else
      {
         while (content.Length > 0)
         {
            var newLinePosition = content.IndexOf(NewLine[0]);

            if (newLinePosition >= 0)
            {
               var line = content[..newLinePosition];

               WriteIf(!line.IsEmpty, line);
               WriteLine();

               content = content[(newLinePosition + 1)..];
            }
            else
            {
               WriteText(content);
               break;
            }
         }
      }
   }

   public void WriteIf(bool condition, string content, bool multiLine = false)
      => WriteIf(condition, content.AsSpan(), multiLine);

   public void WriteIf(bool condition, ReadOnlySpan<char> content, bool multiLine = false)
   {
      if (condition)
      {
         Write(content, multiLine);
      }
   }

   public void WriteLine(string content, bool multiLine = false)
      => WriteLine(content.AsSpan(), multiLine);

   public void WriteLine(ReadOnlySpan<char> content, bool multiLine = false)
   {
      Write(content, multiLine);
      WriteLine();
   }

   public void WriteLineIf(bool condition)
   {
      if (condition)
      {
         WriteLine();
      }
   }
   
   public void WriteLineIf(bool condition, string content, bool multiLine = false)
      => WriteLineIf(condition, content.AsSpan(), multiLine);

   public void WriteLineIf(bool condition, ReadOnlySpan<char> content, bool multiLine = false)
   {
      if (condition)
      {
         WriteLine(content, multiLine);
      }
   }

   public void WriteLine()
   {
      Builder.Add(NewLine[0]);
   }

   public override string ToString()
   {
      return Builder.Span.Trim().ToString();
   }

   public void Dispose()
   {
      Builder.Dispose();
   }

   private void AddIndentOnDemand()
   {
      if (Builder.Count == 0 || Builder.Span[^1] == NewLine[0])
      {
         Builder.AddRange(CurrentLevelString.AsSpan());
      }
   }
}