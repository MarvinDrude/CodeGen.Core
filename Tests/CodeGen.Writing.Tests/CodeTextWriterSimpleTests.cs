namespace CodeGen.Writing.Tests;

public class CodeTextWriterSimpleTests
{
   [Test]
   public async Task SimpleHeapClassOne()
   {
      var generated = CreateClassString();
      var result = 
      """
      public class Test
      {
         public string StrTest { get; set; }

         public Test()
         {
            StrTest = "Test";
         }
      }
      
      """.Replace("\r\n", "\n");

      await Assert.That(result).IsEqualTo(generated);
      return;
      
      static string CreateClassString()
      {
         var writer = new CodeTextWriter(
            stackalloc char[16], stackalloc char[128]);
         const string str = "Test";

         writer.WriteLineInterpolated($"public class {str}");
         writer.OpenBody();
         
         writer.WriteLine("public string StrTest { get; set; }");
         writer.WriteLine();
         
         writer.WriteLineInterpolated($"public {str}()");
         writer.OpenBody();
         writer.WriteLine($"StrTest = \"Test\";");
         writer.CloseBody();
         
         writer.CloseBody();

         return writer.ToString();
      }
   }
}