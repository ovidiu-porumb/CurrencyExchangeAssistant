using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using MediatR;
using SimpleFeedReader;

// ReSharper disable ArrangeThisQualifier
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace OP.CurrencyExchangeAssistant.WebService.Queries
{
    public class ExchangeFluctuationQuery : IRequest<bool>
    {
        [Required]
        public double FluctuationThreshold { get; set; }

        public bool IsThereAReasonToPanicBecauseOfTheExchange()
        {
            var reportedExchangeRates = DownloadEuroExchangeRatesForToday();

            if (AnyFluctuationOverTheThresholdWasRegistered(reportedExchangeRates.ToList(), FluctuationThreshold))
            {
                return true;
            }

            return false;
        }

        private IEnumerable<FeedItem> DownloadEuroExchangeRatesForToday()
        {
            var reader = new FeedReader();
            var rawExchangeRates = reader.RetrieveFeed(ConfigurationManager.AppSettings["BNRExchangeUrlForEuro"]).ToList();
            var adaptedExchangeRates = AdaptTheExchangeRatesToNumberComparison(rawExchangeRates);

            return adaptedExchangeRates;
        }

        private IEnumerable<FeedItem> AdaptTheExchangeRatesToNumberComparison(List<FeedItem> rawExchangeRates)
        {
            var result = new List<FeedItem>(rawExchangeRates.Count());
            result.AddRange(rawExchangeRates.Select(exchangeRate => new FeedItem
            {
                Date = exchangeRate.Date, 
                Summary = exchangeRate.Summary, 
                Title = exchangeRate.Title,
                Uri = exchangeRate.Uri,
                Content = exchangeRate.Title.Split(' ')[3]
            }));

            return result.OrderBy(i => i.Date);
        }

        private bool AnyFluctuationOverTheThresholdWasRegistered(List<FeedItem> reportedExchangeRates, double fluctuationThreshold)
        {
            var convertedExchangeRates = new List<double>(reportedExchangeRates.Count());
            convertedExchangeRates.AddRange(reportedExchangeRates.Select(exchangeRate => double.Parse(exchangeRate.Content)));

            return convertedExchangeRates.Any(rate => Math.Floor(rate - convertedExchangeRates.First()) > fluctuationThreshold);
        }
    }
}