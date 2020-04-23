using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APBD._1.DTOs.Requests;
using APBD._1.DTOs.Responses;
using APBD._1.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APBD._1.Controllers
{
    [Route("api/prescription")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private IPrescriptionService _dbService;
        public PrescriptionController(IPrescriptionService dbService)
        {
            _dbService = dbService;
        }


        [HttpGet("{lastName")]
        public IActionResult GetPrescriptions(String lastName)  // /api/prescription?LastName=abc
        {
            var pres = _dbService.GetPrescriptions(lastName);
            if (pres != null)
            {
                return Ok(pres);
            }

            return BadRequest("Błędny parametr");
        }


        [HttpPost]
        public IActionResult AddPrescription(AddPrescriptionRequest request)
        {
            var response = new AddPrescriptionResponse();

            if(_dbService.AddPrescription(AddPrescriptionRequest request))
            {
                return Ok("dodano");
            }

            return BadRequest();
        }
    }
}