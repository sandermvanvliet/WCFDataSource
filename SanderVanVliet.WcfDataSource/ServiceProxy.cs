using System;
using System.Reflection;

namespace SanderVanVliet.WcfDataSource
{
    public class ServiceProxy
    {
        public IDisposable Client { get; set; }
        public Type Contract { get; set; }
        public MethodInfo[] Operations { get; set; }
    }
}