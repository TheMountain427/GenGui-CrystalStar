using System.Text.RegularExpressions;
using GenGui_CrystalStar.Code.DatabaseModels;

namespace GenGui_CrystalStar.Services;


public class TagMaker
{

    private List<int>? _commaIndexes { get; set; }
    private int _commaCount { get; set; }
    private string _line { get; set; } = "";
    private string _commaTag { get; set; } = "";
    private string _cleanTag { get; set; } = "";
    private bool _isMultiTag { get; set; }
    private string _cleanTagUnderscore { get; set; } = "";
    private string _commaTagUnderscore { get; set; } = "";
    private int _linenumber { get; set; }
    private string? _blockName { get; set; }
    private BlockFlag _blockFlag { get; set; }

    public Tags MakeTag(string inputLine, int inputLineNum)
    {
        if (ValidateInput(inputLine) == false)
        {
            return null!;
        }

        else
        {
            _line = EscapeParenthesis(inputLine.Trim());
            _linenumber = inputLineNum;
            // to do: undo whatever I was doing here
            CleanSpaces();
            CountAndFindCommas();
            SetCommaStyle();
            SetCommaUnderscoreStyle();
            SetCleanStyle();
            SetCleanUnderscoreStyle();

            var tag = new Tags
            {
                Line = _line,
                LineNumber = _linenumber,
                CommaTag = _commaTag,
                CleanTag = _cleanTag,
                CleanTagUnderscore = _cleanTagUnderscore,
                CommaTagUnderscore = _commaTagUnderscore,
                IsMultiTag = _isMultiTag
            };

            return tag;
        }

    }

    private bool ValidateInput(string inputLine)
    {
        if (inputLine.Where(chr => chr == '{' || chr == '}').Count() > 0)
        {
            Console.WriteLine($"{inputLine} is invalid. {{,}} are protected characters");
            return false;
        }

        if (FindCharIndexes(inputLine, '[').Count != FindCharIndexes(inputLine, ']').Count
            || FindCharIndexes(inputLine, '(').Count != FindCharIndexes(inputLine, ')').Count)
        {
            Console.WriteLine($"{inputLine} is invalid. Cannot have an odd amount of (,),[, or ]");
            return false;
        }

        return true;
    }

    private void CountAndFindCommas()
    {
        var indexes = _line.Select((chr, indx) => chr == ',' ? indx : -1 ).Where(i => i != -1).ToList();

        if (indexes.Count() == 0)
        {
            _commaIndexes = [];
            _commaCount = 0;
            _isMultiTag = false;
        }
        else
        {
            _commaIndexes = indexes;
            _commaCount = indexes.Count;
            _isMultiTag = indexes.Count > 1 || _commaIndexes.Last() != _line.Length - 1 ? true : false;
        }
    }

    private void SetCommaStyle()
    {
        if (_commaCount == 0)
        {
            _commaTag = _line + ',';
        }
        else if (_commaCount >= 1)
        {
            _commaTag = _commaIndexes!.Last() == _line.Length - 1 ? _line : _line + ',';
        }
    }

    private void SetCommaUnderscoreStyle()
    {
        if (_commaCount == 0)
        {
            _commaTagUnderscore = _line.Replace(" "," _") + ',';
        }
        else if (_commaCount >= 1)
        {
            _commaTagUnderscore = _commaIndexes!.Last() == _line.Length - 1 ? Regex.Replace(_line, @"\b\s\b", "_")
                                                                            : Regex.Replace(_line, @"\b\s\b", "_") + ',';
        }
    }

    private void SetCleanStyle()
    {
        if (_commaCount == 0)
        {
            _cleanTag = _line;
        }
        else if (_commaCount == 1)
        {
            _cleanTag = _isMultiTag ? _line : _line.TrimEnd(',');
        }
        else
        {
            _cleanTag =  _commaIndexes!.Last() == _line.Length - 1 ? _line.TrimEnd(',') : _line;
        }
    }

    private void SetCleanUnderscoreStyle()
    {
        if (_commaCount == 0)
        {
            _cleanTagUnderscore = _line.Replace(" "," _");
        }
        else if (_commaCount == 1)
        {
            _cleanTagUnderscore = _isMultiTag ? Regex.Replace(_line, @"\b\s\b", "_") : Regex.Replace(_line, @"\b\s\b", "_").TrimEnd(',');
        }
        else
        {
            _cleanTagUnderscore =  _commaIndexes!.Last() == _line.Length - 1 ? Regex.Replace(_line, @"\b\s\b", "_").TrimEnd(',')
                                                                             : Regex.Replace(_line, @"\b\s\b", "_");
        }
    }

    private void CleanSpaces()
    {
        _line = Regex.Replace(_line, @"\s+", " ");
    }

    private string EscapeParenthesis(string inputLine)
    {
        if (inputLine.Any(chr => chr == '(' || chr == ')'))
        {
            string tmpLine = inputLine;
            var openParenthesis = FindCharIndexes(tmpLine, '(');
            var closedParenthesis = FindCharIndexes(tmpLine, ')');

            if (openParenthesis.Count == closedParenthesis.Count)
            {
                // remove bad parenthesis ie: )string(
                var indexesToRemove = new List<int>();
                for (int i = 0; i < openParenthesis.Count - 1; i++)
                {
                    if(openParenthesis[i] > closedParenthesis[i])
                    {
                        indexesToRemove.Add(openParenthesis[i]);
                        openParenthesis.RemoveAt(i);
                        indexesToRemove.Add(closedParenthesis[i]);
                        closedParenthesis.RemoveAt(i);
                    }
                }
                if (indexesToRemove.Count > 0)
                {

                    int i = 0; foreach (int badIndex in indexesToRemove.Order())
                    {
                        tmpLine = tmpLine.Remove(badIndex - i, 1);
                        i++;
                    }
                }

                if (tmpLine.Contains(':'))
                {
                    var splitStrings = new List<string>();
                    if (openParenthesis.Count != 0) // see if we removed all the parenthesis
                    {

                        for (int i = 0; i < openParenthesis.Count; i++)
                        {
                            try
                            {
                                var emptySub = tmpLine.Substring(0,tmpLine.IndexOf('('));
                                if (!string.IsNullOrWhiteSpace(emptySub))
                                {
                                    splitStrings.Add(emptySub);
                                    tmpLine = tmpLine.Replace(emptySub, "");
                                }

                                int substringLength = tmpLine.IndexOf(')') - tmpLine.IndexOf('(') + 1;
                                var sub = tmpLine.Substring(tmpLine.IndexOf('('), substringLength);
                                if (sub.Contains(':'))
                                {
                                    splitStrings.Add(sub);
                                    tmpLine = tmpLine.Replace(sub, "");
                                }
                                else
                                {
                                    tmpLine = tmpLine.Replace(sub, "");
                                    sub = sub.Replace("(", @"\(");
                                    sub = sub.Replace(")", @"\)");
                                    splitStrings.Add(sub);
                                }
                            }
                            catch (System.Exception e)
                            {
                                Console.WriteLine($"{e}");
                                Console.WriteLine("Probably died on something like (ab (cd) ef). Can't handle that atm, but would be a rare case");

                            }
                        }
                        var joinStr = string.Concat(splitStrings);
                        joinStr += tmpLine;
                        tmpLine = joinStr;
                    }
                    else
                    {   // we removed the only )(, so remove the :
                        tmpLine = tmpLine.Replace(":", "");
                    }

                }
                else
                {
                    tmpLine = tmpLine.Replace("(", @"\(");
                    tmpLine = tmpLine.Replace(")", @"\)");
                }

            }
            else
            {
                throw new Exception($"{_line} cannot be escaped. Unclear parenthesis.");
            }

            return tmpLine;
        }
        else
        {
            return inputLine;
        }
    }

    private bool doesCharMatchList(char chr, List<char> charList)
    {
        if (charList is not null)
        {
            foreach (char matchChar in charList)
            {
                if (chr == matchChar)
                {
                    return true;
                }
            }

            return false;
        }

        return false;
    }

    private List<int> FindCharIndexes(string line, char matchChar)
    {
        return line.Select((chr, indx) => chr == matchChar ? indx : -1 ).Where(i => i != -1).ToList();
    }
}
