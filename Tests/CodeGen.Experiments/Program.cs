
using CodeGen.Writing;
using CodeGen.Writing.Models.Types;

// Console.WriteLine("Hello, World!");
//
// var writer = new CodeTextWriter(
//    stackalloc char[256], stackalloc char[128]);
//   
// var x = 20;
//
// writer.UpIndent();
// writer.UpIndent();
// writer.OpenBody();
// writer.WriteInterpolated($"{x} - {x}");
// writer.WriteLine();
// writer.CloseBodySemicolon();
//
// Console.WriteLine(writer.ToString());

var header = new ClassHeaderRender();
header = header.WithName("Test")
   .WithBaseType("Base");