
using CodeGen.Writing;

Console.WriteLine("Hello, World!");

var writer = new CodeTextWriter(
   stackalloc char[256], stackalloc char[128]);
  
var x = 20;

writer.UpIndent();
writer.UpIndent();
writer.OpenBody();
writer.WriteInterpolated($"{x} - {x}");
writer.WriteLine();
writer.CloseBodySemicolon();

Console.WriteLine(writer.ToString());