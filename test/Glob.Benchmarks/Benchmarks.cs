using BenchmarkDotNet.Attributes;

namespace Glob.Benchmarks
{
    public class Benchmarks
    {
        private static readonly string Pattern = "p?th/*a[bcd]b[e-g]a[1-4][!wxyz][!a-c][!1-3].*";
        private Glob _compiled;
        private Glob _uncompiled;
        private Lst<Segment> _segments;
        private Lst<string> _pathSegments;

        public Benchmarks()
        {
            this._compiled = new Glob(Pattern, GlobOptions.Compiled);
            this._uncompiled = new Glob(Pattern);


            this._segments = new Parser(Pattern).ParseTree().Segments.ToLst();

            this._pathSegments = "pAth/fooooacbfa2vd4.txt".Split(new[] { System.IO.Path.DirectorySeparatorChar }).ToLst();
        }

        [Benchmark]
        public void ParseGlob()
        {
            var parser = new Parser(Pattern);
            parser.Parse();
        }

        [Benchmark]
        public Glob ParseAndCompileGlob()
        {
            return new Glob(Pattern, GlobOptions.Compiled);
        }

        [Benchmark(Baseline = true)]
        public bool TestMatchForUncompiledGlob()
        {
            return new Glob(Pattern).IsMatch("pAth/fooooacbfa2vd4.txt");
        }

        [Benchmark]
        public object BenchmarkParseToLst()
        {
            return new Parser(Pattern).ParseTree().Segments.ToLst();
        }

        [Benchmark]
        public object BenchmarkParseToTree()
        {
            return new Parser(Pattern).ParseTree();
        }
    }
}
