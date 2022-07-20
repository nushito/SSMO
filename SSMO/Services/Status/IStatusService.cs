using SSMO.Models;
using System.Collections.Generic;

namespace SSMO.Services.Status
{
    public interface IStatusService
    {
        ICollection<StatusModel> GetAllStatus();
    }
}
