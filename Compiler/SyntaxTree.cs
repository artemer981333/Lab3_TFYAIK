using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compiler
{
    class SyntaxTree
    {
        private struct AnalyzeError
        {
            public int errorStart, errorLength;
        }
        private struct AnalyzeInfo
        {
            public string analizedString, correctedString;
            public List<AnalyzeError> errors;
        }
        private abstract class GrammarNTreminal
        {
            public abstract AnalyzeInfo Analyze(string code, int previousSymbols);
        }

        private class IfConstruction : GrammarNTreminal
        {
            private static Regex rightConstruction = new Regex(@"if( )+\(.*\)");
            private static Regex wrongIFConstruction = new Regex(@"(i( )+\(?.*\)?)|(f( )+\(?.*\)?)");
            private static Regex wrongBRACKETSConstruction = new Regex(@"((if)?( )+.*\))|((if)?( )+\(.*)");
            public override AnalyzeInfo Analyze(string code, int previousSymbols)
            {
                AnalyzeInfo analyzeInfo = new AnalyzeInfo();
                analyzeInfo.analizedString = code;
                analyzeInfo.correctedString = code;

                MatchCollection matches = rightConstruction.Matches(code);
                if (matches.Count == 0)
                {
                    MatchCollection matchesWrongIF = wrongIFConstruction.Matches(code);
                    MatchCollection matchesWrongBRACKETS = wrongBRACKETSConstruction.Matches(code);
                    if (matchesWrongIF.Count != 0)
                    {
                        Regex ifCorrecter = new Regex(@"i|f");
                        MatchCollection match = ifCorrecter.Matches(code);
                        analyzeInfo.correctedString.Remove(match[0].Index, match[0].Length);
                        analyzeInfo.correctedString.Insert(match[0].Index, "if");
                        AnalyzeError error = new AnalyzeError();
                        error.errorStart = previousSymbols + match[0].Index;
                        error.errorLength = match[0].Length;
                        analyzeInfo.errors.Add(new AnalyzeError());
                    }
                    if (matchesWrongBRACKETS.Count != 0)
                    {
                        Regex bracketsCorrecter = new Regex(@"i|f");
                        MatchCollection match = bracketsCorrecter.Matches(code);
                        analyzeInfo.correctedString.Remove(match[0].Index, match[0].Length);
                        analyzeInfo.correctedString.Insert(match[0].Index, "if");
                        AnalyzeError error = new AnalyzeError();
                        error.errorStart = previousSymbols + match[0].Index;
                        error.errorLength = match[0].Length;
                        analyzeInfo.errors.Add(new AnalyzeError());
                    }
                }
                return analyzeInfo;
            }
        }

        RichTextBox textBox;
        public SyntaxTree()
        {

        }

        public void AnalyzeText()
        {

        }
    }
}
