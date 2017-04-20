using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Glob
{
    public class Glob
    {
        private readonly GlobOptions _options;
        public string Pattern { get; private set; }

        private GlobNode _root;
        private Lst<Segment> _segments;

        public Glob(string pattern, GlobOptions options = GlobOptions.None)
        {
            _options = options;
            this.Pattern = pattern;
            if(options.HasFlag(GlobOptions.Compiled))
            {
                this.Compile();
            }
        }

        private void Compile()
        {
            if(_root != null)
                return;

            if (_segments != null)
                return;

            var parser = new Parser(this.Pattern);
            _root = parser.Parse();
            _segments = parser.ParseTree().Segments.ToLst();
        }

        public bool IsMatch(string input)
        {
            this.Compile();

            var pathSegments = input.Split(new[] { Path.DirectorySeparatorChar }).ToLst();

            return IsMatch(_segments, pathSegments);
        }

        static bool IsMatch(Lst<Segment> pattern, Lst<string> input)
        {
            switch (input)
            {
                case Nil<string> _:
                    return pattern is Nil<Segment>;

                case Cons<string> lst:
                    var (head, tail) = lst;

                    switch (pattern)
                    {
                        case Nil<Segment> _: // we have a path to match but nothing to match against so we are done.
                            return false;

                        case Cons<Segment> cons:
                            var (shead, stail) = cons;

                            switch (shead)
                            {
                                case DirectoryWildcard _:
                                    // return all consuming the wildcard
                                    return IsMatch(stail, tail) || IsMatch(cons, tail);

                                case Root root when head == root.Text:
                                    return IsMatch(stail, tail);

                                case DirectorySegment dir when dir.MatchesSegment(head):
                                    return IsMatch(stail, tail);
                            }

                            break;
                    }

                    return false;
            }

            return false;
        }

        public static bool IsMatch(string input, string pattern)
        {
            return new Glob(pattern).IsMatch(input);
        }
    }
}
