using Survey.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.HelperClasses
{
    public static class Operator
    {
        private static Operators GreaterThan { get; } = new Operators() { OperatorText = "Greater Than", Id = 1, OperatorSymbol = ">" };
        private static Operators LessThan { get; } = new Operators() { OperatorText = "Less Than", Id = 2, OperatorSymbol = "<" };
        private static Operators EqualTo { get; } = new Operators() { OperatorText = "Equal To", Id = 3, OperatorSymbol = "==" };
        private static Operators Less_Equal { get; } = new Operators() { OperatorText = "Less Or Equal", Id = 4, OperatorSymbol = "<=" };
        private static Operators Greater_Equal { get; } = new Operators() { OperatorText = "Greater Or Equal", Id = 5, OperatorSymbol = ">=" };
        private static List<Operators> Options { get; } = new List<Operators> { EqualTo };
        private static List<Operators> Number { get; } = new List<Operators> { EqualTo, LessThan, Less_Equal, GreaterThan, Greater_Equal };
        public static List<Operators> GetOperators(int id) 
        {
            if (id == 1)
            {
                return Number;
            }
            else if (id == 14 || id == 3)
            {
                return Options;
            }
            else 
            {
                throw new Exception(" This type of question cannot be skipped! ");
            }
        }
        public static Operators getOneOperator(int id)
        {
            Operators skip = new Operators();
            switch (id)
            {
                case 1: skip = GreaterThan; break;
                case 2: skip = LessThan; break;
                case 3: skip = EqualTo; break;
                case 4: skip = Less_Equal; break;
                case 5: skip = Greater_Equal; break;
                default: break;

            }
            return skip;
        }
        public static Operators getOneOperator(string op) 
        {
            var oper = Number.FirstOrDefault(x => x.OperatorSymbol.ToLower().Equals(op.ToLower()));
            return oper;
        }
    }
}
