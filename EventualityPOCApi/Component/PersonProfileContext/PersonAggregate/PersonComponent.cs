using EventualityPOC.PersonProfileContext.PersonAggregate.Application;
using EventualityPOCApi.Gateway.BridgeHttp.Channel;
using EventualityPOCApi.Shared.Framework;
using EventualityPOCApi.Shared.Xapi;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
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
                .Where(ew => ew.EventType == Verb.PersonCreationRequested)
                .Subscribe(ew => HandleStatement(ew, PersonApplicationService.PersonCreationRequested));
            _perceptionChannel.Observable()
                .Where(ew => ew.EventType == Verb.PersonRequested)
                .Subscribe(ew => HandleStatement(ew, PersonApplicationService.PersonRequested));
            _perceptionChannel.Observable()
                .Where(ew => ew.EventType == Verb.PersonUpdateRequested)
                .Subscribe(ew => HandleStatement(ew, PersonApplicationService.PersonUpdateRequested));
        }
        #endregion

        #region private
        private void HandleStatement(StatementWrapper statementWrapper, Func<JObject, IPersonRepository, JObject> handler)
        {
            try
            {
                var returnJObject = handler(statementWrapper.Data, _personRepository);
                var returnStatementWrapper = new StatementWrapper(statementWrapper.Subject, returnJObject);
                _decisionChannel.NextAsync(returnStatementWrapper);
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
