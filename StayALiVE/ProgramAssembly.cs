using System;
using System.Reflection;

//-- https://github.com/Ni1kko/StayALiVE

namespace StayALiVE
{
    internal class ProgramAssembly
    {
        internal static ProgramAssembly GetInstance() { return new ProgramAssembly(); }
        internal Assembly _assembly = typeof(Program).Assembly;
        internal Program program = null;
        internal Ui ui = null;

        internal ProgramAssembly()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver;
            program = Program.GetInstance();
        }
        private Assembly AssemblyResolver(object sender, ResolveEventArgs args)
        {
            var askedAssembly = new AssemblyName(args.Name);
            lock (this)
            {
                var stream = _assembly.GetManifestResourceStream($"{Assembly.GetExecutingAssembly().GetName().Name}.Embedded.Assemblies.{askedAssembly.Name}.dll");
                if (stream == null) return null;

                Assembly assembly = null;
                try
                {
                    var assemblyData = new byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    assembly = Assembly.Load(assemblyData);
                }
                catch (Exception e)
                {
                    throw new Exception($"Loading embedded assembly: {askedAssembly.Name}{Environment.NewLine}Has thrown a unhandled exception: {e}");
                }
                return assembly;
            }
        }
    }
}
