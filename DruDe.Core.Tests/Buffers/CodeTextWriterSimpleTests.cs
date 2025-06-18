using DruDe.Core.Buffers;

namespace DruDe.Core.Tests.Buffers;

public class CodeTextWriterSimpleTests
{
   [Test]
   public async Task SimpleClassOne()
   {
      var generated = CreateString();
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

      static string CreateString()
      {
         using var writer = new CodeTextWriter(
            stackalloc char[512],
            stackalloc char[128],
            3, ' ');
         
         writer.WriteLine("public class Test");
         writer.OpenBody();
         writer.WriteLine("public string StrTest { get; set; }");
         writer.WriteLine();
         writer.WriteLine("public Test()");
         writer.OpenBody();
         writer.WriteLine("StrTest = \"Test\";");
         writer.CloseBody();
         writer.CloseBody();
         
         return writer.ToString();
      }
   }
}