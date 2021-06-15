using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HMS.Data.Services;
using HMS.Data.ViewModels.Bill;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HMSAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillsController : ControllerBase
    {
        private IBillService _billService;

        public BillsController(IBillService billService)
        {
            _billService = billService;
        }
        
        /// <summary>
        /// Get Bills by contractId
        /// </summary>
        /// <param name="contractId"></param>
        /// <returns></returns>
        [HttpGet]
        public List<BillDetailViewModel> GetBills(int contractId)
        {
            return _billService.GetByContractID(contractId);
        }

        /// <summary>
        /// Get Bill by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public BillDetailViewModel GetBill(string id)
        {
            return _billService.GetByID(id);
        }
    }
}
