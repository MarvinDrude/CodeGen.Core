namespace CodeGen.Common.CodeGen.Models.Common;

[Flags]
public enum ClassType
{
   Class = 0,
   RecordClass = 1 << 0,
   Struct = 1 << 1,
   RecordStruct = 1 << 2,
   Interface = 1 << 3,
}

public static class ClassTypeExtensions
{
   public static void FillCharBuffer(this ClassType modifier, scoped in Span<char> buffer)
   {
      ReadOnlySpan<char> target = modifier switch
      {
         ClassType.Class => "class",
         ClassType.RecordClass => "record",
         ClassType.Struct => "struct",
         ClassType.RecordStruct => "record struct",
         ClassType.Interface => "interface",
         _ => string.Empty
      };
         
      target.CopyTo(buffer);
   }
   
   public static int GetCharBufferSize(this ClassType modifier)
   {
      return modifier switch
      {
         ClassType.Class => 5,
         ClassType.RecordClass => 6,
         ClassType.Struct => 6,
         ClassType.RecordStruct => 13,
         ClassType.Interface => 9,
         _ => 0
      };
   }
}