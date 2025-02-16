
namespace StockReport.Core;
    public static class Extension
    {
        public static string? SqlDbconnectionString { get; set; }

        public static string MethodName(this object obj, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            return memberName;
        }

        public static string ClassName(this object obj)
        {
            return obj.GetType().Name;
        }

    
}

