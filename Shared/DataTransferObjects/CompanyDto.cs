using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    //NB: XmlSerializer cannot easily serialize our positional record type but we have 2 solution to it
    //1. Add [Serialize] attribute
    //2. Modify the rcord with init only properties as follows and changing the Mapping in the AutoMapper

    //[Serializable]
    //public record CompanyDto(Guid Id, string Name, string FullAddress);

    public record CompanyDto
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public string? FullAddress { get; init; }
    }

}
