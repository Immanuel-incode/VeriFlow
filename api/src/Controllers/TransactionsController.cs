using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Api.Repositories;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly TransactionRepository _repository;

        public TransactionsController(TransactionRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<List<Transaction>> GetAll()
        {
            var transactions = _repository.GetAll();
            return Ok(transactions);
        }

        [HttpGet("{id}")]
        public ActionResult<Transaction> GetById(int id)
        {
            var transaction = _repository.GetById(id);
            if (transaction == null)
                return NotFound();
            return Ok(transaction);
        }

        [HttpGet("fraud")]
        public ActionResult<List<Transaction>> GetFraudOnly()
        {
            var transactions = _repository.GetFraudOnly();
            return Ok(transactions);
        }
    }
}