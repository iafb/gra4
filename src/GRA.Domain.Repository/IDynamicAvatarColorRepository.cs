using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IDynamicAvatarColorRepository : IRepository<DynamicAvatarColor>
    {
        Task<ICollection<DynamicAvatarColor>> GetByLayerAsync(int layerId);
    }
}
