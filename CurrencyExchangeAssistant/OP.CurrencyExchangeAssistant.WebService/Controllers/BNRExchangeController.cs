using System.Web.Http;
using MediatR;
using OP.CurrencyExchangeAssistant.WebService.Queries;

// ReSharper disable InconsistentNaming

namespace OP.CurrencyExchangeAssistant.WebService.Controllers
{
    public class BNRExchangeController : ApiController
    {
        private readonly IMediator mediator;

        public BNRExchangeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public IHttpActionResult CheckIfANotableFluctuationHappenedToday([FromUri]ExchangeFluctuationQuery exchangeFluctuationQuery)
        {
            var queryResult = mediator.Send(exchangeFluctuationQuery);
            return Ok(queryResult);
        }
    }
}
