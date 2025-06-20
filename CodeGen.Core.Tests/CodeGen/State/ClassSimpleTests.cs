using CodeGen.Common.CodeGen;

namespace CodeGen.Core.Tests.CodeGen.State;

public class ClassSimpleTests
{
   [Test]
   public async Task ClassHeader()
   {
      var generated = CreateString();
      var result = 
         """
         public class Test<T>
            : BaseTest
            where T : notnull
         {
         """.Replace("\r\n", "\n");

      await Assert.That(result).IsEqualTo(generated);
      return;
      
      static string CreateString()
      {
         var builder = new CodeBuilder(
            stackalloc char[512],
            stackalloc char[128],
            3, ' ');

         builder.Class.Declaration = "public class Test<T>";
         builder.Class.BaseDeclarations = [
            "BaseTest"
         ];
         builder.Class.GenericConstraints = [
            "where T : notnull"
         ];

         builder.Class.RenderDeclaration();
         
         var str = builder.ToString();
         builder.Dispose();

         return str;
      }
   }
}