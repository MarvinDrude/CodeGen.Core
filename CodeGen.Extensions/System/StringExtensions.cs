namespace CodeGen.Extensions.System;

public static class StringExtensions
{
   extension(string str)
   {
      public string FirstCharToLower()
      {
         if (string.IsNullOrEmpty(str) || char.IsLower(str[0]))
         {
            return str;
         }
      
         return char.ToLowerInvariant(str[0]) + str[1..];
      }
   
      public string FirstCharToUpper()
      {
         if (string.IsNullOrEmpty(str) || char.IsUpper(str[0]))
         {
            return str;
         }
      
         return char.ToUpperInvariant(str[0]) + str[1..];
      }
   }
}