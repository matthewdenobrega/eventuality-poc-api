using EventualityPOCApi.Context.PersonProfileContext.PersonAggregate.Application;
using EventualityPOCApi.Gateway.Channel;
using EventualityPOCApi.Shared.Framework;
using EventualityPOCApi.Shared.Xapi;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace EventualityPOCApi.Gateway.Component.PersonProfileContext.PersonAggregate
{
    public class PersonComponent
    {
        private readonly IDecisionChannel _decisionChannel;
        private readonly ILogger<PersonComponent> _logger;
        private readonly IPerceptionChannel _perceptionChannel;
        private readonly IPersonRepository _personRepository;

        #region Constructor
        public PersonComponent(IDecisionChannel decisionChannel, ILogger<PersonComponent> logger,
            IPerceptionChannel perceptionChannel, IPersonRepository personRepository)
        {
            _decisionChannel = decisionChannel;
            _logger = logger;
            _perceptionChannel = perceptionChannel;
            _personRepository = personRepository;
        }
        #endregion

        #region Public
        public void Configure()
        {
            _perceptionChannel.Observable()
                .Where(sw => sw.EventType == Verb.PersonCreationRequested)
                .Subscribe(sw => HandleStatement(sw, PersonApplicationService.PersonCreationRequested));
            _perceptionChannel.Observable()
                .Where(sw => sw.EventType == Verb.PersonRequested)
                .Subscribe(sw => HandleStatement(sw, PersonApplicationService.PersonRequested));
            _perceptionChannel.Observable()
                .Where(sw => sw.EventType == Verb.PersonUpdateRequested)
                .Subscribe(sw => HandleStatement(sw, PersonApplicationService.PersonUpdateRequested));
        }
        #endregion

        #region private
        private void HandleStatement(StatementWrapper statementWrapper, Func<StatementExtension, IPersonRepository, StatementExtension> handler)
        {
            try
            {
                var decisionStatement = handler(statementWrapper.Data, _personRepository);
                var decisionStatementWrapper = new StatementWrapper(statementWrapper.Subject, decisionStatement);
                _decisionChannel.NextAsync(decisionStatementWrapper);
            }
            catch (Exception exception)
            {
                _logger.LogError("Exception handling statement for person aggregate: " + exception.Message);
                _logger.LogError(exception.StackTrace);
            }
        }
        #endregion
    }
}
