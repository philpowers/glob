using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Glob
{
    static class Matcher
    {
        public static bool MatchesSegment(this DirectorySegment segment, string pathSegment) =>
            segment.SubSegments.ToLst().MatchesSubSegment(pathSegment);

        public static bool MatchesSubSegment(this Lst<SubSegment> segments, string pathSegment, int index = 0)
        {
            switch (segments)
            {
                case Nil<SubSegment> _:
                    return index == pathSegment.Length;

                case Cons<SubSegment> cons:
                    var (head, tail) = cons;

                    switch (head)
                    {
                        // match zero or more chars
                        case StringWildcard _:
                            return tail.MatchesSubSegment(pathSegment, index) // zero
                                || (index < pathSegment.Length && segments.MatchesSubSegment(pathSegment, index + 1)); // or one+

                        case CharacterWildcard _:
                            return index < pathSegment.Length && tail.MatchesSubSegment(pathSegment, index + 1);

                        case Identifier ident:
                            var len = ident.Value.Length;
                            if (len + index > pathSegment.Length)
                                return false;

                            if (pathSegment.Substring(index, ident.Value.Length) != ident.Value)
                                return false;

                            return tail.MatchesSubSegment(pathSegment, index + len);

                        case LiteralSet literalSet:
                            return literalSet.Literals.Any(lit => tail.Prepend(lit).MatchesSubSegment(pathSegment, index));

                        case CharacterSet set:
                            if (index == pathSegment.Length)
                                return false;

                            var inThere = set.Characters.Value.Contains(pathSegment[index]);
                            return (inThere != set.Inverted) && tail.MatchesSubSegment(pathSegment, index + 1);
                    }
                    return false;
            }

            return false;
        }
    }
}
