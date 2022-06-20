

using System.ComponentModel;

namespace SSMO.Enums
{
    public enum DescriptionEnum
    {
        [Description("Birch Film Faced Plywood")]
        BirchFilmFaced,
        [Description("Birch Plywood")]
            BirchPlywood,
        [Description("Poplar Film Faced Plywood")]
        PoplarFilmFaced,
        [Description("Poplar Plywood")]
        PoplarPlywood,
        [Description("Pine Film Faced Plywood")]
        PineFFPlywood,
        [Description("Pine Plywood")]
        PinePlywood,
        [Description("Twin Film Faced Plywood")]
        TwinFFPlywood,
        [Description("Twin Film Faced Plywood/Birch & Pine")]
        TwinFFPlywoodBirchPine,
        [Description("Twin Film Faced Plywood/Poplar & Pine")]
        TwinFFPlywoodPoplarPine,
        [Description("Combi Film Faced Plywood")]
        CombiFFPlywood,
        [Description("Combi Film Faced Plywood/Birch & Pine")]
        CombiFFPlywoodBirchPine,
        [Description("Combi Film Faced Plywood/Poplar & Pine")]
        CombiFFPlywoodPoplarPine
    }
}
