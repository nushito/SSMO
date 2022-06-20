
using System.ComponentModel;

namespace SSMO.Data.Enums
{
    public enum GradeEnum
    {
       
       A,
       B,
       C,
       [Description("I/I")]
       First,
        [Description("I/II")]
        FisrtSecond, 
        [Description("B/BB")]
        BBB,
        [Description("BB/BB")]
        BBBB,
        [Description("BB/CP")]
        BBCP,
        [Description("BB/C")]
        BBC,
        [Description("CP/CP")]
        CPCP,
        [Description("CP/C")]
        CPC,
        [Description("C/C")]
        CC
    };
}
