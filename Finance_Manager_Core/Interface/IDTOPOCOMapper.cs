using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_Core.Interface
{
    public interface IDTOPOCOMapper
    {
        public TTarget Map<TSource, TTarget>(TSource source) where TTarget : new();
    }
}
