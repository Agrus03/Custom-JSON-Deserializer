using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace ConsoleApp
{
    public enum TokenType
    {
        CurlyOpen,
        CurlyClose,
        SquareOpen,
        SquareClose,
        Colon,
        Comma,
        String,
        Number,
    }

    public class JsonToken
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return $"[{Type}]: {Value}";
        }
    }

    public static class CustomJsonParser
    {
        private static List<JsonToken> Tokenize(string filePath)
        {
            var tokens = new List<JsonToken>();
            using FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using StreamReader sr = new StreamReader(fs, Encoding.UTF8);
            int ch;
            
            while ((ch = sr.Read()) != -1)
            {
                char c = (char)ch;
                if (char.IsWhiteSpace(c)) continue;
                
                switch (c)
                {
                    case '{': tokens.Add(new JsonToken { Type = TokenType.CurlyOpen, Value = "{" }); break;
                    case '}': tokens.Add(new JsonToken { Type = TokenType.CurlyClose, Value = "}" }); break;
                    case '[': tokens.Add(new JsonToken { Type = TokenType.SquareOpen, Value = "[" }); break;
                    case ']': tokens.Add(new JsonToken { Type = TokenType.SquareClose, Value = "]" }); break;
                    case ',': tokens.Add(new JsonToken { Type = TokenType.Comma, Value = "," }); break;
                    case ':': tokens.Add(new JsonToken { Type = TokenType.Colon, Value = ":" }); break;
                    case '"':
                        StringBuilder stringBuilder = new StringBuilder();
                        while ((ch = sr.Read()) != -1 && (char)ch != '"')
                        {
                            stringBuilder.Append((char)ch);
                        }
                        tokens.Add(new JsonToken { Type = TokenType.String, Value = stringBuilder.ToString() });
                        break;
                    default:
                        if (char.IsDigit(c) || c == '-')
                        {
                            StringBuilder numberBuilder = new StringBuilder();
                            numberBuilder.Append(c);
                            while ((ch = sr.Peek()) != -1 && (char.IsDigit((char)ch) || (char)ch == '.'))
                            {
                                numberBuilder.Append((char)sr.Read());
                            }
                            tokens.Add(new JsonToken { Type = TokenType.Number, Value = numberBuilder.ToString() });
                        }
                        break;
                }
            }
            return tokens;
        }

        public static T Deserialize<T>(string filePath) where T : new()
        {
            List<JsonToken> tokens = Tokenize(filePath);
            int position = 0;
            object result = Parse(typeof(T), tokens, ref position);
            return (T)result;
        }

        private static object Parse(Type targetType, List<JsonToken> tokens, ref int pos)
        {
            object instance = Activator.CreateInstance(targetType);
            pos++;

            while (pos < tokens.Count && tokens[pos].Type != TokenType.CurlyClose)
            {
                if (tokens[pos].Type == TokenType.Comma)
                {
                    pos++;
                    continue;
                }

                string keyName = tokens[pos].Value;
                pos += 2;

                PropertyInfo prop = targetType.GetProperty(keyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                
                if (prop != null)
                {
                    JsonToken valueToken = tokens[pos];
                    
                    if (valueToken.Type == TokenType.String)
                    {
                        prop.SetValue(instance, valueToken.Value);
                        pos++;
                    }
                    else if (valueToken.Type == TokenType.Number)
                    {
                        object convertedNum = Convert.ChangeType(valueToken.Value, prop.PropertyType, CultureInfo.InvariantCulture);
                        prop.SetValue(instance, convertedNum);
                        pos++;
                    }
                    else if (valueToken.Type == TokenType.CurlyOpen)
                    {
                        object nestedObj = Parse(prop.PropertyType, tokens, ref pos);
                        prop.SetValue(instance, nestedObj);
                    }
                    else if (valueToken.Type == TokenType.SquareOpen)
                    {
                        Type itemType = prop.PropertyType.GetGenericArguments()[0];
                        IList listInstance = (IList)Activator.CreateInstance(prop.PropertyType);
                        pos++;

                        while (tokens[pos].Type != TokenType.SquareClose)
                        {
                            if (tokens[pos].Type == TokenType.Comma)
                            {
                                pos++;
                                continue;
                            }

                            if (tokens[pos].Type == TokenType.CurlyOpen)
                            {
                                object listItem = Parse(itemType, tokens, ref pos);
                                listInstance.Add(listItem);
                            }
                            else
                            {
                                pos++;
                            }
                        }
                        
                        pos++;
                        prop.SetValue(instance, listInstance);
                    }
                    else
                    {
                        pos++;
                    }
                }
                else
                {
                    pos++;
                }
            }

            pos++;
            return instance;
        }
    }
}