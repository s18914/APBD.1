using APBD._1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APBD._1.Services
{
    public interface IPrescriptionService
    {
        public List<Prescription> GetPrescriptions(String sortBy);
    }
}
