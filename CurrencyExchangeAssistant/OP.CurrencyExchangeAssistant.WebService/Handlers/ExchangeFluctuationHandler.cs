using MediatR;
using OP.CurrencyExchangeAssistant.WebService.Queries;

namespace OP.CurrencyExchangeAssistant.WebService.Handlers
{
    public class ExchangeFluctuationHandler : IRequestHandler<ExchangeFluctuationQuery, bool>
    {
        public bool Handle(ExchangeFluctuationQuery exchangeFluctuationQuery)
        {
            return exchangeFluctuationQuery.IsThereAReasonToPanicBecauseOfTheExchange();
        }
    }
}