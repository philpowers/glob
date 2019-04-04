﻿using System.Text.RegularExpressions;

namespace Glob
{
    public class Glob
    {
        private readonly GlobOptions _options;
        public string Pattern { get; private set; }

        private GlobNode _root;
        private Regex _regex;

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

            if (_regex != null)
                return;

            var parser = new Parser(this.Pattern);
            _root = parser.Parse();

            var regexPattern = GlobToRegexVisitor.Process(_root);

            // Post-process to handle matching files located in top-level directory by Directory Wildcard
            regexPattern = regexPattern.Replace(".*[/\\\\]", "(.*[/\\\\])?");

            RegexOptions regexOptions = RegexOptions.Singleline;
            if (_options.HasFlag(GlobOptions.Compiled))
                regexOptions |= RegexOptions.Compiled;
            if (_options.HasFlag(GlobOptions.IgnoreCase))
                regexOptions |= RegexOptions.IgnoreCase;
            
            _regex = new Regex(regexPattern, regexOptions);
        }

        public bool IsMatch(string input)
        {
            this.Compile();

            return _regex.IsMatch(input);
        }

        public static bool IsMatch(string input, string pattern)
        {
            return new Glob(pattern).IsMatch(input);
        }
    }
}
