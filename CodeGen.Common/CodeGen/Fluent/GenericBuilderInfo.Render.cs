namespace CodeGen.Common.CodeGen.Fluent;

public static partial class GenericBuilderInfoExtensions
{
   public static void RenderGenericParameter(this ref CodeBuilder builder, int index)
   {
      var first = true;
      builder.Writer.Write("<");
      
      foreach (var generic in builder.GetTemporaryEnumerator<GenericBuilderInfo>(index))
      {
         if (!first)
         {
            builder.Writer.Write(", ");
         }
         
         builder.Writer.Write(generic.Name.Span);
         first = false;
      }
      
      builder.Writer.Write(">");
   }

   public static void RenderGenericConstraints(this ref CodeBuilder builder, int index)
   {
      var first = true;
      
      foreach (var generic in builder.GetTemporaryEnumerator<GenericBuilderInfo>(index))
      {
         if (first)
         {
            first = false;
            builder.Writer.UpIndent();
         }
         
         builder.Writer.Write("where ");
         builder.Writer.Write(generic.Name.Span);
         builder.Writer.Write(" : ");

         for (var e = 0; e < generic.ConstraintCount; e++)
         {
            
         }
      }
      
      builder.Writer.DownIndent();
   }
}